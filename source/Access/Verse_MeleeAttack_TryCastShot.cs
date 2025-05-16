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
//  [HarmonyPatch(typeof(Verb_MeleeAttack), "TryCastShot")]
//  public static class Verse_MeleeAttack_TryCastShot
//  {
//    public static void Postfix(Verb_MeleeAttack __instance, ref bool __result)
//    {
//      try
//      {
//        if (__result && __instance.CurrentTarget.Thing is Pawn targetPawn && __instance.tool != null)
//        {
//          Pawn attackerPawn = __instance.CasterPawn;
//          Log.Message($"{attackerPawn} melee attacked {targetPawn}");
//          if (attackerPawn.equipment.Primary != null && attackerPawn.Faction.HostileTo(targetPawn.Faction) && Rand.Chance(Settings.AttackQuipChance.Value))
//            InteractionUtility.ImitateInteractionWithNoPawn(
//              attackerPawn,
//              new MeleeAttack_InteractionDef(targetPawn, __instance, attackerPawn.equipment.Primary.def));
//        }
//      }
//      catch (Exception ex)
//      {
//        Log.Error($"Error in {nameof(Verse_LaunchProjectile_TryCastShot)}: {ex}");
//      }
//    }
//  }
//}
