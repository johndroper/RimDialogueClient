#if !RW_1_5

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDialogue.Context
{
  public sealed class OnnxSentenceEmbedder : IDisposable
  {
    private readonly InferenceSession _session;
    private readonly BertTokenizer _tokenizer;
    private readonly int _maxSeqLen;

    public OnnxSentenceEmbedder(string onnxPath, string vocabTxtPath, int maxSeqLen = 256, bool lowerCase = true)
    {
      _session = new InferenceSession(onnxPath);

      // Build tokenizer from vocab.txt with options
      var opts = new BertOptions
      {
        LowerCaseBeforeTokenization = lowerCase,
        ApplyBasicTokenization = true
      };
      _tokenizer = BertTokenizer.Create(vocabTxtPath, opts);  // creates a WordPiece-based BERT tokenizer. :contentReference[oaicite:0]{index=0}
      _maxSeqLen = maxSeqLen;
    }

    public float[] Embed(string text)
    {
      // Encode to token IDs (adds [CLS]/[SEP] for us when addSpecialTokens = true)
      // out: normalizedText (unused) and charsConsumed (unused)
      string? _normalizedText;
      int _charsConsumed;
      var ids = _tokenizer.EncodeToIds(
          text,
          _maxSeqLen,               // maxTokenCount
          addSpecialTokens: true,   // add [CLS] ... [SEP]
          out _normalizedText,
          out _charsConsumed,
          considerPreTokenization: true,
          considerNormalization: true
      ); // BertTokenizer.EncodeToIds overload with addSpecialTokens. :contentReference[oaicite:1]{index=1}

      // Attention mask = 1 for real tokens, 0 for padding
      var attention = new long[_maxSeqLen];
      var inputIds = new long[_maxSeqLen];

      // Copy encoded ids, then pad to fixed length with [PAD]
      int n = Math.Min(ids.Count, _maxSeqLen);
      for (int i = 0; i < n; i++) { inputIds[i] = ids[i]; attention[i] = 1; }
      for (int i = n; i < _maxSeqLen; i++) inputIds[i] = _tokenizer.PaddingTokenId; // PAD id from tokenizer. :contentReference[oaicite:2]{index=2}

      // (Optional) token_type_ids: all zeros for single sequence
      var tokenTypes = new long[_maxSeqLen]; // already 0s

      // Build tensors (batch size = 1)
      var idsT = new DenseTensor<long>(new[] { 1, _maxSeqLen });
      var maskT = new DenseTensor<long>(new[] { 1, _maxSeqLen });
      var typesT = new DenseTensor<long>(new[] { 1, _maxSeqLen });
      for (int i = 0; i < _maxSeqLen; i++)
      {
        idsT[0, i] = inputIds[i];
        maskT[0, i] = attention[i];
        typesT[0, i] = tokenTypes[i];
      }

      // Feed inputs; handle common input names
      var inputs = new List<NamedOnnxValue>();
      var names = _session.InputMetadata.Keys.ToArray();
      void AddIfPresent(string name, DenseTensor<long> t)
      {
        if (names.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
          inputs.Add(NamedOnnxValue.CreateFromTensor(name, t));
      }
      AddIfPresent("input_ids", idsT);
      AddIfPresent("input_ids:0", idsT);
      AddIfPresent("attention_mask", maskT);
      AddIfPresent("attention_mask:0", maskT);
      AddIfPresent("token_type_ids", typesT);
      AddIfPresent("token_type_ids:0", typesT);

      if (inputs.Count == 0)
        throw new InvalidOperationException("No recognized input names in the ONNX model.");

      using var results = _session.Run(inputs);

      // Pick an output and convert to a sentence vector
      var first = results.First().AsTensor<float>();
      float[] vec;
      if (first.Rank == 2) // [1, hidden] pooled embedding
      {
        int hidden = first.Dimensions[1];
        vec = new float[hidden];
        for (int h = 0; h < hidden; h++) vec[h] = first[0, h];
      }
      else if (first.Rank == 3) // [1, seq, hidden] -> mean-pool by attention
      {
        int seq = first.Dimensions[1], hidden = first.Dimensions[2];
        vec = new float[hidden];
        int valid = 0;
        for (int s = 0; s < seq; s++)
        {
          if (maskT[0, s] == 0) continue;
          valid++;
          for (int h = 0; h < hidden; h++) vec[h] += first[0, s, h];
        }
        if (valid > 0) for (int h = 0; h < hidden; h++) vec[h] /= valid;
      }
      else
      {
        throw new NotSupportedException("Unexpected ONNX output shape.");
      }

      VectorMath.L2NormalizeInPlace(vec);
      return vec;
    }

    public void Dispose() => _session.Dispose();
  }
}
#endif
