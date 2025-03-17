//using Bubbles.Core;
//using HarmonyLib;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Verse;

//namespace RimDialogue.Access.Incidents
//{
//  [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
//  public static class RimWorld_IncidentWorker_RaidEnemy_TryExecuteWorker
//  {
//    public static bool Prefix(IncidentParms parms)
//    {
//      Mod.LogV($"IncidentWorker_RaidEnemy_TryExecuteWorker.");

//      string text = string.Format(parms.raidArrivalMode.textEnemy, parms.faction.def.pawnsPlural, parms.faction.Name.ApplyTag(parms.faction)).CapitalizeFirst();
//      text += "\n\n";
//      text += parms.raidStrategy.arrivalTextEnemy;
//      Pawn pawn = pawns.Find((Pawn x) => x.Faction.leader == x);
//      if (pawn != null)
//      {
//        text += "\n\n";
//        text += "EnemyRaidLeaderPresent".Translate(pawn.Faction.def.pawnsPlural, pawn.LabelShort, pawn.Named("LEADER")).Resolve();
//      }

//      if (parms.raidAgeRestriction != null && !parms.raidAgeRestriction.arrivalTextExtra.NullOrEmpty())
//      {
//        text += "\n\n";
//        text += parms.raidAgeRestriction.arrivalTextExtra.Formatted(parms.faction.def.pawnsPlural.Named("PAWNSPLURAL")).Resolve();
//      }

//      return true;
//    }


//  }
//}
