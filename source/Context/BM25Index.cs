#nullable enable
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
  public sealed class BM25Index<T> where T : IDocument
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
      double k1 = 1.2,
      double b = 0.75,
      double k3 = 0,
      Func<string, IEnumerable<string>>? tokenizer = null,
      ISet<string>? stopWords = null)
    {
      if (k1 <= 0) throw new ArgumentOutOfRangeException(nameof(k1));
      if (b < 0 || b > 1) throw new ArgumentOutOfRangeException(nameof(b));
      if (k3 < 0) throw new ArgumentOutOfRangeException(nameof(k3));

      K1 = k1; B = b; K3 = k3;
      _tokenizer = tokenizer ?? DefaultTokenizer;
      _stopWords = stopWords ?? EnglishStopWordsSmall;
    }

    public BM25Index(
      IEnumerable<T> docs,
      double k1 = 1.2,
      double b = 0.75,
      double k3 = 0,
      Func<string, IEnumerable<string>>? tokenizer = null,
      ISet<string>? stopWords = null)
        : this(k1, b, k3, tokenizer, stopWords)
    {
      if (docs == null) throw new ArgumentNullException(nameof(docs));
      foreach (var d in docs) AddInternal(d);
      Recompute();
    }

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
      try {
        _avgDocLength = _docLengths.Average(kvp => kvp.Value);
      } 
      finally { _lock.ExitReadLock(); }
    }

    public SearchResult<T>[] Search(string query, float minScore, int topK)
    {
      if (query == null) throw new ArgumentNullException(nameof(query));
      if (docCount == 0) return Array.Empty<SearchResult<T>>();

      var qTokens = FilterTokens(_tokenizer(query)).ToArray();
      if (!qTokens.Any()) return Array.Empty<SearchResult<T>>();

      // Query term frequencies (qtf)
      var qtf = new Dictionary<string, int>(StringComparer.Ordinal);
      foreach (var t in qTokens)
      {
        int c;
        if (!qtf.TryGetValue(t, out c)) qtf[t] = 1; else qtf[t] = c + 1;
      }

      int N = docCount;
      var scores = new Dictionary<T, float>();
      var now = Find.TickManager.TicksAbs;

      _lock.EnterReadLock();
      try
      {
        foreach (var kv in qtf)
        {
          int qFreq = kv.Value;
          if (!_postings.TryGetValue(kv.Key, out Dictionary<T, int> documentFrequencies))
            continue;
          int df = documentFrequencies.Count;
          double idf = Math.Log((N - df + 0.5) / (df + 0.5));
          double qWeight = (K3 > 0) ? ((K3 + 1) * qFreq) / (K3 + qFreq) : 1.0;
          foreach (var dp in documentFrequencies)
          {
            T metadata = dp.Key;
            var deltaTicks = now - metadata.Tick;
            if (deltaTicks < 100 || deltaTicks > GenDate.TicksPerYear)
              continue;
            int frequency = dp.Value;
            int documentLength = _docLengths[metadata];
            if (documentLength == 0) continue;
            double norm = frequency * (K1 + 1.0) / (frequency + K1 * (1.0 - B + B * documentLength / Math.Max(_avgDocLength, 1e-9)));
            double contrib = idf * norm * qWeight * metadata.Weight;
            float cur;
            if (!scores.TryGetValue(metadata, out cur)) scores[metadata] = (float)contrib;
            else scores[metadata] = (float)(cur + contrib);
          }
        }
      }
      finally { _lock.ExitReadLock(); }

#if DEBUG
      using (var logStream = File.Create($"D:\\junk\\bm25_{Find.TickManager.TicksAbs}.log"))
      using (var logWriter = new StreamWriter(logStream))
      {
        logWriter.WriteLine($"begin bm25 {now}: '{query}'");
        foreach (var score in scores)
        {
          logWriter.WriteLine($"bm25 {now}: '{score.Key.Text}' = {score.Value}");
        }
        if (scores.Any())
        {
          var maxScore = scores.Max(r => r.Value);
          var avgScore = scores.Average(r => r.Value);
          logWriter.WriteLine($"end bm25 {now}: {scores.Count()} results, max score: {maxScore}, avg score: {avgScore}");
        }
      } 
#endif

      //re-rank scores by recency

      return scores
        .Select(kv => new SearchResult<T>(
          kv.Key,
          (1f - ((float)(now - kv.Key.Tick) / GenDate.TicksPerYear)) * kv.Value))
        .Where(kv => kv.Score >= minScore)
        .OrderByDescending(kvp => kvp.Score)
        .Take(topK)
        .ToArray();
    }

    // ---------- Helpers ----------
    private IEnumerable<string> FilterTokens(IEnumerable<string> tokens)
    {
      foreach (var t in tokens)
      {
        if (_stopWords == null || !_stopWords.Contains(t)) yield return t;
      }
    }

    private static readonly Regex TokenRe = new Regex(@"[A-Za-z0-9_]+", RegexOptions.Compiled);
    private static IEnumerable<string> DefaultTokenizer(string text)
    {
      if (string.IsNullOrEmpty(text)) yield break;
      foreach (Match m in TokenRe.Matches(text))
        yield return m.Value.ToLowerInvariant();
    }

    // ---------- Convenience ----------
    public int DocumentCount { get { return docCount; } }
    public double AverageDocumentLength { get { return _avgDocLength; } }

    public static HashSet<string> EnglishStopWordsSmall = new HashSet<string>(new[]
    {
      "a","an","and","are","as","at","be","by","for","from","has","he","in",
      "is","it","its","of","on","that","the","to","was","were","will","with"
    }, StringComparer.OrdinalIgnoreCase);
  }
}
