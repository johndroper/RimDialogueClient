#nullable enable
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Verse;

namespace RimDialogue.Context
{
  public abstract class ContextData : IContext, IEquatable<ContextData>
  {

    //TODO
    //active quests
    //battle events
    //public event Action<IContext>? OnRefresh;

    public ContextData(
      string type,
      float weight,
      params Pawn[] pawns)
    {
      Type = type;
      Weight = weight;
      Pawns = pawns;
    }
    public abstract string Text { get; protected set; }
    public string Type { get; set; }
    public Pawn[] Pawns { get; set; }
    public float Weight { get; set; }

    //public int AccessCount => _accessCount;
    //private int _accessCount = 0;
    //public void Accessed()
    //{
    //  Interlocked.Increment(ref _accessCount);
    //}

    //public virtual void Refresh()
    //{
    //  OnRefresh?.Invoke(this);
    //}

    public bool Equals(ContextData other)
    {
      return other != null && Text == other.Text;
    }
    public override int GetHashCode()
    {
      return Text.GetHashCode();
    }

    public virtual float VectorScore(float[] queryVector, float[] dbVector)
    {
      return VectorMath.Cosine(queryVector, dbVector) * Weight;
    }

    public virtual float BM25Score(double idf, double norm, double qWeight)
    {
      return (float)(idf * norm * qWeight * Weight);
    }
  }
}
