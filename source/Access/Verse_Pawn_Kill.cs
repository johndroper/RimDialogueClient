using HarmonyLib;
using RimDialogue.Core;
using Verse;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
public static class Patch_Pawn_Kill
{
  public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
  {
    GameComponent_PawnDeathTracker.Instance?.RecordDeath(__instance, dinfo, exactCulprit);
  }
}
