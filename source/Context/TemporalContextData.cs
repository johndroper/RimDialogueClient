using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Context
{
  public class TemporalContextData : BasicContextData, ITemporalContext
  {
    public const int MIN_TICKS_BEFORE_USE = 100;

    public TemporalContextData(
      string text,
      string type,
      int tick,
      float weight,
      params Pawn[] pawns) : base(text, type, weight, pawns)
    {
      Tick = tick;
    }

    public int Tick { get; set; }

    public override float VectorScore(float[] queryVector, float[] dbVector)
    {
      var deltaTicks = Find.TickManager.TicksAbs - Tick;
      if (deltaTicks < MIN_TICKS_BEFORE_USE || deltaTicks > GenDate.TicksPerYear)
        return -1f;
      return base.VectorScore(queryVector, dbVector) *
        (1f - ((float)deltaTicks / GenDate.TicksPerYear));
    }

    public override float BM25Score(double idf, double norm, double qWeight)
    {
      var deltaTicks = Find.TickManager.TicksAbs - Tick;
      if (deltaTicks < MIN_TICKS_BEFORE_USE || deltaTicks > GenDate.TicksPerYear)
        return -1f;
      return base.BM25Score(idf, norm, qWeight) *
        (1f - ((float)deltaTicks / GenDate.TicksPerYear));
    }

    public float RetentionScore()
    {
      var deltaTicks = Find.TickManager.TicksAbs - Tick;
      return Weight * (1f - ((float)deltaTicks / GenDate.TicksPerYear));
    }
  }
}

