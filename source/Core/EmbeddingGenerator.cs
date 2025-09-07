using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.Generic;
using System.Linq;

public class EmbeddingGenerator
{
  private readonly InferenceSession _session;

  public EmbeddingGenerator(string modelPath)
  {
    _session = new InferenceSession(modelPath);
  }

  public float[] GetEmbedding(long[] tokenIds)
  {
    // Create input tensor
    var inputTensor = new DenseTensor<long>(new[] { tokenIds.Length });
    for (int i = 0; i < tokenIds.Length; i++)
      inputTensor[i] = tokenIds[i];

    var inputs = new List<NamedOnnxValue>
      {
          NamedOnnxValue.CreateFromTensor("input_ids", inputTensor)
      };

    // Run inference
    using var results = _session.Run(inputs);
    var embeddingTensor = results.First().AsEnumerable<float>().ToArray();

    return embeddingTensor;
  }
}
