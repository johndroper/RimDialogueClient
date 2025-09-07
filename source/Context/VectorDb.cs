using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Context
{
  public class VectorDb<T> where T : IDocument
  {
    static OnnxSentenceEmbedder embedder = new OnnxSentenceEmbedder(
      "C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld\\Mods\\RimDialogueClient\\Assemblies\\model.onnx",
      "C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld\\Mods\\RimDialogueClient\\Assemblies\\vocab.txt");

    private readonly List<(T metadata, float[] vec)> _items = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public VectorDb()
    {
    }

    public VectorDb(IEnumerable<T> docs)
    {
      foreach(var doc in docs)
      {
        var normalizedVec = embedder.Embed(doc.Text);
        _items.Add((doc, normalizedVec));
      }
    }

    public void Add(T metadata)
    {
      var normalizedVec = embedder.Embed(metadata.Text);
      _lock.EnterWriteLock();
      try { _items.Add((metadata, normalizedVec)); }
      finally { _lock.ExitWriteLock(); }
    }

    public void Remove(T metadata)
    {
      _lock.EnterWriteLock();
      try
      {
        var items = _items.Where(i => i.metadata.Equals(metadata)).ToArray();
        foreach(var item in items)
          _items.Remove(item);
      }
      finally { _lock.ExitWriteLock(); }
    }

    public IEnumerable<SearchResult<T>> Search(string query, float minScore, int k)
    {
      var queryVec = embedder.Embed(query);
      var results = new List<SearchResult<T>>();
      int now = Find.TickManager.TicksAbs;

      _lock.EnterReadLock();
      try
      {
#if DEBUG
        using (var logStream = File.Create($"D:\\junk\\vec_{Find.TickManager.TicksAbs}.log"))
        using (var logWriter = new StreamWriter(logStream))
        {
          logWriter.WriteLine($"query\t{query}'");
#endif
          foreach (var (metadata, vec) in _items)
          {
            var deltaTicks = now - metadata.Tick;
            if (deltaTicks < 100 || deltaTicks > GenDate.TicksPerYear)
              continue;
            float score = VectorMath.Cosine(queryVec, vec) *
              (1f - ((float)deltaTicks / GenDate.TicksPerYear)) * metadata.Weight;
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
      get {
        _lock.EnterReadLock();
        try { return _items.Count; } finally { _lock.ExitReadLock(); } }
    }
  }
}
