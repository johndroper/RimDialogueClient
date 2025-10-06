#if !RW_1_5
using RimDialogue.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Verse;

namespace RimDialogue.Context
{


  public class VectorDb<T> where T : IContext
  {
    static OnnxSentenceEmbedder embedder;

    static VectorDb()
    {
      embedder = new OnnxSentenceEmbedder(
        Path.Combine(Mod.Instance.Content.RootDir, "Common", "Models", "model.onnx"),
        Path.Combine(Mod.Instance.Content.RootDir, "Common", "Models", "vocab.txt"));
    }

    private readonly List<(T metadata, float[] vec)> _items = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public VectorDb(string name)
    {
      Name = name;
    }

    public string Name { get; }

    public VectorDb(IEnumerable<T> docs)
    {
      foreach (var doc in docs)
      {
        var normalizedVec = embedder.Embed(doc.Text);
        _items.Add((doc, normalizedVec));
      }
    }

    public void Add(T metadata)
    {
      var normalizedVec = embedder.Embed(metadata.Text);
      _lock.EnterWriteLock();
      var item = (metadata, normalizedVec);
      try { _items.Add(item); }
      finally { _lock.ExitWriteLock(); }

      if (metadata is IRefreshable refreshable)
      {
        refreshable.OnRefresh += (metadata) =>
        {
          item.normalizedVec = embedder.Embed(metadata.Text);
        };
      }
    }

    public void Remove(T metadata)
    {
      _lock.EnterWriteLock();
      try
      {
        var items = _items.Where(i => i.metadata.Equals(metadata)).ToArray();
        foreach (var item in items)
          _items.Remove(item);
      }
      finally { _lock.ExitWriteLock(); }
    }

    public IEnumerable<SearchResult<T>> Search(string query, float minScore, int k)
    {
      query = query.RemoveWhiteSpaceAndColor();

      var queryVec = embedder.Embed(query);
      var results = new List<SearchResult<T>>();
      int now = Find.TickManager.TicksAbs;

      _lock.EnterReadLock();
      try
      {
#if DEBUG
        using (var logStream = File.Create($"{GenFilePaths.SaveDataFolderPath}\\Logs\\vec_{Name}_{query.Substring(0, 10).Clean()}.log"))
        using (var logWriter = new StreamWriter(logStream))
        {
          logWriter.WriteLine($"query\t{query}'");
#endif
          foreach (var (metadata, vec) in _items)
          {
            if (metadata is IExpirable expirable && expirable.IsExpired)
              continue;

            float score = metadata.VectorScore(queryVec, vec);
#if DEBUG
            logWriter.WriteLine($"{score}\t{metadata.Text}");
#endif
            if (score < minScore)
              continue;
            results.Add(new SearchResult<T>(metadata, score));
          }
#if DEBUG
          if (Settings.VerboseLogging.Value)
          {
            if (results.Any())
            {
              var maxScore = results.Max(r => r.Score);
              var avgScore = results.Average(r => r.Score);
              logWriter.WriteLine($"{results.Count()} results, max score: {maxScore}, avg score: {avgScore}");
            }
            else
              logWriter.WriteLine($"No results above '{minScore}'.");
          }
        }
#endif
      }
      finally { _lock.ExitReadLock(); }
      return results
        .OrderByDescending(r => r.Score)
        .Take(k);
    }

    public int Count
    {
      get
      {
        _lock.EnterReadLock();
        try { return _items.Count; } finally { _lock.ExitReadLock(); }
      }
    }
  }
}
#endif
