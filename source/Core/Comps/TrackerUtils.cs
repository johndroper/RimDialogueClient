#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.Comps
{
  public static class TrackerUtils
  {
    public static Pawn? GetPawnById(int? pawnId)
    {
      if (!pawnId.HasValue)
        return null;

      Pawn pawn;
      if (Current.Game.CurrentMap != null)
      {
        pawn = Find.CurrentMap.mapPawns.AllPawns.FirstOrDefault(p => p.thingIDNumber == pawnId);
        if (pawn != null)
          return pawn;
      }

      foreach (Map map in Find.Maps)
      {
        pawn = map.mapPawns.AllPawns.FirstOrDefault(p => p.thingIDNumber == pawnId);
        if (pawn != null) return pawn;
      }

      return Find.WorldPawns.AllPawnsAliveOrDead.FirstOrDefault(p => p.thingIDNumber == pawnId);
    }
  }
}
