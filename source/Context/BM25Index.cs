#nullable enable
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Verse;
using static UnityEngine.Networking.UnityWebRequest;

namespace RimDialogue.Context
{
  public sealed class BM25Index<T> where T : IContext
  {
    private readonly ReaderWriterLockSlim _lock = new();

    // ---------- BM25 parameters ----------
    public double K1 { get; private set; }  // saturation for document term frequency (typical ~1.2)
    public double B { get; private set; }  // length normalization (typical ~0.75)
    public double K3 { get; private set; }  // optional query term boosting; set 0 to ignore (typical 0 or ~500-1000)

    // ---------- Index data ----------
    private int docCount = 0;
    private readonly Dictionary<string, Dictionary<T, int>> _postings = new Dictionary<string, Dictionary<T, int>>(StringComparer.Ordinal);
    private readonly Dictionary<T, int> _docLengths = []; // in tokens
    private double _avgDocLength = 0.0;

    // ---------- Text processing ----------
    private readonly Func<string, IEnumerable<string>> _tokenizer;
    private readonly ISet<string> _stopWords;

    public BM25Index(
      string name,
      double k1 = 1.2,
      double b = 0.75,
      double k3 = 0,
      IEnumerable<string>? stopWords = null,
      Func<string, IEnumerable<string>>? tokenizer = null)
    {
      if (k1 <= 0) throw new ArgumentOutOfRangeException(nameof(k1));
      if (b < 0 || b > 1) throw new ArgumentOutOfRangeException(nameof(b));
      if (k3 < 0) throw new ArgumentOutOfRangeException(nameof(k3));

      Name = name;

      K1 = k1; B = b; K3 = k3;
      _tokenizer = tokenizer ?? DefaultTokenizer;
      if (stopWords != null)
      {
        _stopWords = new HashSet<string>(DefaultStopWords);
        foreach (var w in stopWords)
          _stopWords.Add(w.ToLowerInvariant());
      }
      else
        _stopWords = DefaultStopWords;
    }

    public BM25Index(
      string name,
      IEnumerable<T> docs,
      double k1 = 1.2,
      double b = 0.75,
      double k3 = 0,
      IEnumerable<string>? stopWords = null,
      Func<string, IEnumerable<string>>? tokenizer = null)
        : this(name, k1, b, k3, stopWords, tokenizer)
    {
      if (docs == null) throw new ArgumentNullException(nameof(docs));
      foreach (var d in docs) AddInternal(d);
      Recompute();
    }

    public string Name { get; }

    public void Remove(T metadata)
    {
      if (metadata == null) throw new ArgumentNullException(nameof(metadata));
      if (!_docLengths.TryGetValue(metadata, out int dl)) return;
      _docLengths.Remove(metadata);
      Interlocked.Decrement(ref docCount);
      var tokens = FilterTokens(_tokenizer(metadata.Text))
        .Distinct()
        .ToArray();
      _lock.EnterWriteLock();
      try
      {
        foreach (var token in tokens)
        {
          if (_postings.TryGetValue(token, out Dictionary<T, int> documentFrequencies))
          {
            documentFrequencies.Remove(metadata);
            if (documentFrequencies.Count == 0)
              _postings.Remove(token);
          }
        }
      }
      finally { _lock.ExitWriteLock(); }
    }

    private int changes = 0;
    public void Add(T metadata)
    {
      AddInternal(metadata);
      Interlocked.Increment(ref changes);
      if (changes < 10 || changes % 10 == 0)
        Recompute();
    }

    private void AddInternal(T metadata)
    {
      if (metadata == null) throw new ArgumentNullException(nameof(metadata));
      Interlocked.Increment(ref docCount);

      var tokens = FilterTokens(_tokenizer(metadata.Text)).ToArray();
      var tf = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

      foreach (var t in tokens)
      {
        if (!tf.TryGetValue(t, out int c))
          tf[t] = 1;
        else
          tf[t] = c + 1;
      }
      _lock.EnterWriteLock();
      try
      {
        if (!_docLengths.ContainsKey(metadata))
          _docLengths.Add(metadata, tokens.Length);
        foreach (var kv in tf)
        {
          Dictionary<T, int> documentFrequencies;
          if (!_postings.TryGetValue(kv.Key, out documentFrequencies))
          {
            documentFrequencies = new Dictionary<T, int>();
            _postings[kv.Key] = documentFrequencies;
          }
          documentFrequencies[metadata] = kv.Value;
        }
      }
      finally { _lock.ExitWriteLock(); }
    }

    public void Recompute()
    {
      if (_docLengths.Count == 0) { _avgDocLength = 0; return; }
      _lock.EnterReadLock();
      try
      {
        _avgDocLength = _docLengths.Average(kvp => kvp.Value);
      }
      finally { _lock.ExitReadLock(); }
    }

    public SearchResult<T>[] Search(string query, float minScore, int topK)
    {
      if (query == null) throw new ArgumentNullException(nameof(query));
      if (docCount == 0) return Array.Empty<SearchResult<T>>();

      query = query.RemoveWhiteSpaceAndColor();

      var queryTokens = FilterTokens(_tokenizer(query)).ToArray();
      if (!queryTokens.Any()) return Array.Empty<SearchResult<T>>();

      // Query term frequencies (qtf)
      var queryTermFrequencies = new Dictionary<string, int>(StringComparer.Ordinal);
      foreach (var token in queryTokens)
      {
        int c;
        if (!queryTermFrequencies.TryGetValue(token, out c)) queryTermFrequencies[token] = 1; else queryTermFrequencies[token] = c + 1;
      }

      int N = docCount;
      var scores = new Dictionary<T, float>();
      var now = Find.TickManager.TicksAbs;

#if DEBUG
      using (var logStream = File.Create($"{GenFilePaths.SaveDataFolderPath}\\Logs\\bm25_{Name}_{query.Substring(0, 10).Clean()}.log"))
      using (var logWriter = new StreamWriter(logStream))
      {
#endif
        _lock.EnterReadLock();
        try
        {
#if DEBUG
          logWriter.WriteLine($"{query}");
          logWriter.WriteLine();
#endif
          foreach (var kv in queryTermFrequencies)
          {
            int queryTermFrequency = kv.Value;
            string term = kv.Key;
            if (!_postings.TryGetValue(term, out Dictionary<T, int> documentTermFrequencies))
              continue;
            int df = documentTermFrequencies.Count;
            double idf = Math.Log((N - df + 0.5) / (df + 0.5));
            double qWeight = (K3 > 0) ? ((K3 + 1) * queryTermFrequency) / (K3 + queryTermFrequency) : 1.0;
#if DEBUG
            logWriter.WriteLine($"{term}\t{queryTermFrequency}\t{idf}\t{qWeight}");
#endif
            foreach (var dp in documentTermFrequencies)
            {
              T metadata = dp.Key;
              if (metadata is IExpirable expirable && expirable.IsExpired)
                continue;
              int frequency = dp.Value;
              int documentLength = _docLengths[metadata];
              if (documentLength == 0) continue;
              double norm = frequency * (K1 + 1.0) / (frequency + K1 * (1.0 - B + B * documentLength / Math.Max(_avgDocLength, 1e-9)));
              double scoreContribution = metadata.BM25Score(idf, norm, qWeight);
#if DEBUG
              logWriter.WriteLine($"\t{norm}\t{metadata.Text}");
#endif
              if (!scores.TryGetValue(metadata, out float currentScore))
                scores[metadata] = (float)scoreContribution;
              else
                scores[metadata] = (float)(currentScore + scoreContribution);
            }
          }
        }
        finally { _lock.ExitReadLock(); }

        var results = scores
          .Select(kv => new SearchResult<T>(
            kv.Key,
            kv.Value))
          .OrderByDescending(kvp => kvp.Score)
          .ToArray();

#if DEBUG
        logWriter.WriteLine();
        foreach (var score in results)
        {
          logWriter.WriteLine($"{score.Score}\t{score.MetaData.Text}");
        }
        if (results.Any())
        {
          var maxScore = results.Max(r => r.Score);
          var avgScore = results.Average(r => r.Score);
          logWriter.WriteLine($"end: {scores.Count()} results, max score: {maxScore}, avg score: {avgScore}");
        }
#endif
        return results
          .Where(r => r.Score >= minScore)
          .Take(topK)
          .ToArray();
      }
#if DEBUG
    }
#endif

    // ---------- Helpers ----------
    private IEnumerable<string> FilterTokens(IEnumerable<string> tokens)
    {
      foreach (var t in tokens)
      {
        if (_stopWords == null || !_stopWords.Contains(t)) yield return t;
      }
    }

    private static readonly Regex TokenRe = new Regex(@"\w+(?:\'\w{1,3})?", RegexOptions.Compiled);
    private static IEnumerable<string> DefaultTokenizer(string text)
    {
      if (string.IsNullOrEmpty(text)) yield break;
      foreach (Match m in TokenRe.Matches(text))
        yield return m.Value.ToLowerInvariant();
    }

    // ---------- Convenience ----------
    public int DocumentCount { get { return docCount; } }
    public double AverageDocumentLength { get { return _avgDocLength; } }

    public static HashSet<string> DefaultStopWords =
      new HashSet<string>(
        "RimDialogue.StopWords".Translate()
          .ToString()
          .Trim()
          .Split([','], StringSplitOptions.RemoveEmptyEntries)
          .Select(s => s.Trim().ToLowerInvariant())
      , StringComparer.OrdinalIgnoreCase);
  }
}
