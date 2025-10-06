//#if !RW_1_5
//#nullable enable
//using RimWorld;
//using System.Linq;
//using Verse;

//namespace RimDialogue.Context
//{
//  public class TemporalContextDb : ContextDb<TemporalContextData>
//  {
//    public const int MIN_TICKS_BEFORE_USE = 100;

//    public static float VectorTemporalScore(float[] queryVector, float[] dbVector, TemporalContextData metadata)
//    {
//      var deltaTicks = Find.TickManager.TicksAbs - metadata.Tick;
//      if (deltaTicks < MIN_TICKS_BEFORE_USE || deltaTicks > GenDate.TicksPerYear)
//        return -1f;
//      return VectorScore(queryVector, dbVector, metadata) *
//        (1f - ((float)deltaTicks / GenDate.TicksPerYear));
//    }

//    public static float BM25TemporalScore(double idf, double norm, double qWeight, TemporalContextData metadata)
//    {
//      var deltaTicks = Find.TickManager.TicksAbs - metadata.Tick;
//      if (deltaTicks < MIN_TICKS_BEFORE_USE || deltaTicks > GenDate.TicksPerYear)
//        return -1f;
//      return BM25Score(idf, norm, qWeight, metadata) *
//        (1f - ((float)deltaTicks / GenDate.TicksPerYear));
//    }

//    public TemporalContextDb() : base(
//      [],
//      new VectorDb<TemporalContextData>(VectorTemporalScore),
//      new BM25Index<TemporalContextData>(BM25TemporalScore))
//    {

//    }

//    public override void Cleanup(int maxContextItems)
//    {
//      if (Contexts.Count > maxContextItems)
//      {
//        TemporalContextData[] toRemove;
//        _lock.EnterWriteLock();
//        try
//        {
//          if (Contexts.Count <= maxContextItems)
//            return;
//          toRemove = Contexts
//            .OrderBy(c => c.Tick)
//            .Take(Contexts.Count - maxContextItems)
//            .ToArray();
//          foreach (var contextToRemove in toRemove)
//          {
//            Contexts.Remove(contextToRemove);
//            Bm25Index.Remove(contextToRemove);
//            VectorDb.Remove(contextToRemove);
//          }
//        }
//        finally { _lock.ExitWriteLock(); }
//        if (Settings.VerboseLogging.Value) Mod.Log($"Context cleanup removed {toRemove.Length} items. Now {temporalContexts.Count} items.");
//      }
//    }
//  }
//}
//#endif
