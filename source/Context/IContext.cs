#nullable enable
using System;

namespace RimDialogue.Context
{
  public interface IContext
  {

    public string Text { get; }

    public float Weight { get; }

    public float VectorScore(float[] queryVector, float[] dbVector);

    public float BM25Score(double idf, double norm, double qWeight);

  }
  public interface IAccessible
  {
    public int AccessCount { get; }

    public void Accessed();
  }

  public interface IExpirable
  {
    public event Action<IContext>? OnExpired;
    public bool IsExpired { get; }
    public void Expire();
  }

  public interface IRefreshable : IExpirable
  {
    public event Action<IContext>? OnRefresh;
    public void Refresh();
  }

  public interface ITemporalContext : IContext
  {
    public int Tick { get; }

    public float RetentionScore();

  }
}
