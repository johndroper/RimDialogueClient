#nullable enable
using RimDialogue.Access;
using RimDialogue.Context;
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Pool;
using Verse;
using static System.Net.Mime.MediaTypeNames;

namespace RimDialogue.Context
{
  public class ContextData : IDocument, IEquatable<ContextData>
  {
    public ContextData(
      string text,
      string type,
      int tick,
      float weight,
      params Pawn[] pawns)
    {
      Type = type;
      Tick = tick;
      Text = H.RemoveWhiteSpaceAndColor(text);
      Weight = weight;
      Pawns = pawns;
    }
    public string Text { get; set; }
    public string Type { get; set; }
    public int Tick { get; set; }
    public Pawn[] Pawns { get; set; }
    public float Weight { get; set; }
    public int AccessCount => _accessCount;
    private int _accessCount = 0;
    public void Accessed()
    {
      Interlocked.Increment(ref _accessCount);
    }

    public bool Equals(ContextData other)
    {
      return other != null && Text == other.Text;
    }
    public override int GetHashCode()
    {
      return Text.GetHashCode();
    }
  }

  public class ContextDb
  {
    private readonly ReaderWriterLockSlim _lock = new();
    List<ContextData> contexts = [];
    VectorDb<ContextData> vectorDb = new();
    BM25Index<ContextData> bm25Index = new ();

    public async Task Add(ContextData contextData)
    {
      var contextsTask = Task.Run(() =>
      {
        _lock.EnterWriteLock();
        try
        {
          contexts.Add(contextData);
        }
        finally { _lock.ExitWriteLock(); }
      });
      var bm25Task = Task.Run(() => bm25Index.Add(contextData));
      var vectorTask = Task.Run(() => vectorDb.Add(contextData));
      await Task.WhenAll(contextsTask, bm25Task, vectorTask);
    }

    public void Cleanup(int maxContextItems)
    {
      if (contexts.Count > maxContextItems)
      {
        ContextData[] toRemove;
        _lock.EnterWriteLock();
        try
        {
          if (contexts.Count <= maxContextItems)
            return;
          toRemove = contexts
            .OrderBy(c => c.Tick)
            .Take(contexts.Count - maxContextItems)
            .ToArray();
          foreach (var contextToRemove in toRemove)
          {
            contexts.Remove(contextToRemove);
            bm25Index.Remove(contextToRemove);
            vectorDb.Remove(contextToRemove);
          }
        }
        finally { _lock.ExitWriteLock(); }
        if (Settings.VerboseLogging.Value) Mod.Log($"Context cleanup removed {toRemove.Length} items. Now {contexts.Count} items.");
      }
    }

    public IEnumerable<SearchResult<ContextData>> NormalizeScores(IEnumerable<SearchResult<ContextData>> searchResults)
    {
      if (!searchResults.Any())
        return searchResults;
      var maxScore = searchResults.Max(searchResult => searchResult.Score);
      foreach(var searchResult in searchResults)
      {
        searchResult.Score = searchResult.Score / maxScore;
      }
      return searchResults;
    }

    public async Task<ContextData?[]> Search(string query, int maxResults = 10)
    {
      var vectorTask = Task.Run(() => NormalizeScores(vectorDb.Search(query, 0.4f, maxResults)));
      var bm25Task = Task.Run(() => NormalizeScores(bm25Index.Search(query, 0, maxResults)));
      await Task.WhenAll(vectorTask, bm25Task);
      var vectorResults = vectorTask.Result;
      var bm25Results = bm25Task.Result;
      if (!vectorResults.Any() && !bm25Results.Any())
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"No results from context search: '{query}'.");
        return Array.Empty<ContextData?>();
      }

      //if (vectorResults.Any())
      //  NormalizeScores(vectorResults);

      //if (bm25Results.Any())
      //  NormalizeScores(bm25Results);

      //combine vectorResults and bm25Results, sum the score where the contexts are the same
      var results = vectorResults.Concat(bm25Results)
        .GroupBy(r => r.MetaData)
        .Select(g => new SearchResult<ContextData>(g.Key, g.Sum(r => r.Score)))
        .OrderByDescending(searchResult => searchResult.Score);

#if DEBUG
      using (var logWriter = File.CreateText($"D:\\junk\\combined_{Find.TickManager.TicksAbs}.log"))
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
        .OrderBy(metaData => metaData.Tick)
        .ToArray();
    }
  }
}
