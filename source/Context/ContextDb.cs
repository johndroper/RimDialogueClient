#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Context
{

  public class ContextDb<T>  where T : ContextData
  {
    public static IEnumerable<SearchResult<T>> NormalizeScores(IEnumerable<SearchResult<T>> searchResults)
    {
      if (!searchResults.Any())
        return searchResults;
      var maxScore = searchResults.Max(searchResult => searchResult.Score);
      foreach (var searchResult in searchResults)
      {
        searchResult.Score = searchResult.Score / maxScore;
      }
      return searchResults;
    }

    protected readonly ReaderWriterLockSlim _lock = new();
    protected List<T> Contexts;
    protected VectorDb<T> VectorDb;
    protected BM25Index<T> Bm25Index;

    public ContextDb(string name, IEnumerable<string>? stopWords)
    {
      Name = name;
      Contexts = new List<T>();
      VectorDb = new VectorDb<T>(name);
      Bm25Index = new BM25Index<T>(name, stopWords: stopWords);
    }

    public string Name { get; }

    public async Task Add(T contextData)
    {
      var contextsTask = Task.Run(() =>
      {
        _lock.EnterWriteLock();
        try
        {
          Contexts.Add(contextData);
        }
        finally { _lock.ExitWriteLock(); }
      });
      var bm25Task = Task.Run(() => Bm25Index.Add(contextData));
      var vectorTask = Task.Run(() => VectorDb.Add(contextData));
      await Task.WhenAll(contextsTask, bm25Task, vectorTask);
    }

    public virtual void Cleanup(int maxContextItems)
    {
      List<T> expired = new List<T>();

      _lock.EnterUpgradeableReadLock();
      try
      {
        foreach (var context in Contexts)
        {
          if (context is IExpirable expirable && expirable.IsExpired)
            expired.Add(context);
        }
        _lock.EnterWriteLock();
        try
        {
          foreach (var expiredContext in expired)
          {
            Contexts.Remove(expiredContext);
            Bm25Index.Remove(expiredContext);
            VectorDb.Remove(expiredContext);
          }
        }
        finally
        {
          _lock.ExitWriteLock();
        }
      }
      finally { _lock.ExitUpgradeableReadLock(); }

      if (Settings.VerboseLogging.Value) Mod.Log($"Context cleanup for {Name} removed {expired.Count} expired items. Now {Contexts.Count} items.");

      if (Contexts.Count > maxContextItems)
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Context cleanup for {Name} too large. Count:{Contexts.Count} Max:{maxContextItems}");
        _lock.EnterUpgradeableReadLock();
        try
        {
          var toRemove = Contexts
            .Take(Contexts.Count - maxContextItems)
            .ToArray();
          _lock.EnterWriteLock();
          try
          {
            foreach (var contextToRemove in toRemove)
            {
              Contexts.Remove(contextToRemove);
              Bm25Index.Remove(contextToRemove);
              VectorDb.Remove(contextToRemove);
            }
          }
          finally { _lock.ExitWriteLock(); }
          if (Settings.VerboseLogging.Value) Mod.Log($"Context cleanup for {Name} removed {toRemove.Length} items. Now {Contexts.Count} items.");
        }
        finally { _lock.ExitUpgradeableReadLock(); }
      }
    }

    public virtual async Task<T?[]> Search(string query, int maxResults = 10, float minVectorScore = 0.55f, float minBm25Score = 4.1f)
    {
      var vectorTask = Task.Run(() => NormalizeScores(VectorDb.Search(query, minVectorScore, maxResults)));
      var bm25Task = Task.Run(() => NormalizeScores(Bm25Index.Search(query, minBm25Score, maxResults)));
      await Task.WhenAll(vectorTask, bm25Task);
      var vectorResults = vectorTask.Result;
      var bm25Results = bm25Task.Result;
      if (!vectorResults.Any() && !bm25Results.Any())
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"No results from context search: '{query}'.");
        return Array.Empty<T?>();
      }

      if (vectorResults.Any())
        NormalizeScores(vectorResults);

      if (bm25Results.Any())
        NormalizeScores(bm25Results);

      //combine vectorResults and bm25Results, sum the score where the contexts are the same
      var results = vectorResults.Concat(bm25Results)
        .GroupBy(r => r.MetaData)
        .Select(g => new SearchResult<T>(g.Key, g.Sum(r => r.Score)))
        .OrderByDescending(searchResult => searchResult.Score);

#if DEBUG
      using (var logWriter = File.CreateText($"{GenFilePaths.SaveDataFolderPath}\\Logs\\combined_{Name}_{query.Substring(0, 10).Clean()}.log"))
      {
        logWriter.WriteLine($"query\t{query}");
        foreach (var item in results)
        {
          logWriter.WriteLine($"{item.Score}\t{item.MetaData.Text}");
        }
      } 
#endif

      return results
        .Select(searchResult => searchResult.MetaData)
        .Take(maxResults)
        .ToArray();
    }
  }

  public class BasicContextDb : ContextDb<BasicContextData>
  {
    public BasicContextDb(string name, IEnumerable<string>? stopWords) : base(name,stopWords)
    {
    }
  }

  public class DynamicContextDb : ContextDb<DynamicContextData>
  {
    public DynamicContextDb(string name, IEnumerable<string>? stopWords) : base(name,stopWords)
    {
    }
  }

}
