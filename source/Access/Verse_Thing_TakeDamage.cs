//using HarmonyLib;
//using RimDialogue.Core.InteractionDefs;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Verse;

//namespace RimDialogue.Access
//{
//  [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
//  public static class Verse_Thing_TakeDamage
//  {
//    public static void Prefix(Thing __instance, ref DamageInfo dinfo)
//    {
//      try
//      {
//        if (dinfo.Weapon != null && __instance is Pawn targetPawn && dinfo.Instigator is Pawn attackerPawn && attackerPawn.Faction.HostileTo(targetPawn.Faction))
//        {
//          if (Settings.VerboseLogging.Value) Mod.Log($"{attackerPawn} damaged {targetPawn} with {dinfo.Weapon}");
//          if (Rand.Chance(Settings.HitQuipChance.Value))
//            InteractionUtility.ImitateInteractionWithNoPawn(
//              attackerPawn,
//              new EnemyDamage_InteractionDef(targetPawn, dinfo));
//        }
//      }
//      catch (Exception ex)
//      {
//        Log.Error($"Error in {nameof(Verse_Thing_TakeDamage)}: {ex}");
//      }
//    }
//  }
//}
