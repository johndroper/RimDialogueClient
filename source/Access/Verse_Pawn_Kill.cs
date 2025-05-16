//#nullable enable
//using HarmonyLib;
//using RimDialogue;
//using RimDialogue.Core;
//using RimDialogue.Core.InteractionDefs;
//using RimWorld;
//using Verse;

//[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
//public static class Patch_Pawn_Kill
//{
//  public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff? exactCulprit = null)
//  {
//    GameComponent_PawnDeathTracker.Instance?.RecordDeath(__instance, dinfo, exactCulprit);

//    if (dinfo != null)
//    {
//      var attacker = dinfo?.Instigator as Pawn;
//      if (attacker != null && dinfo.HasValue && attacker.Faction.HostileTo(__instance.Faction) && Rand.Chance(Settings.HitQuipChance.Value))
//      {
//        InteractionUtility.ImitateInteractionWithNoPawn(
//          attacker,
//          new EnemyKill_InteractionDef(__instance, dinfo.Value));
//      }
//    }
//  }
//}
