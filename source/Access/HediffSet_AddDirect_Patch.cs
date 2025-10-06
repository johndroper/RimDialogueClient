using HarmonyLib;
using RimDialogue.Context;
using RimDialogue.Core;
using System;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(HediffSet), "AddDirect")]
  public static class HediffSet_AddDirect_Patch
  {
    [HarmonyPostfix]
    public static void Postfix(Hediff hediff, DamageInfo? dinfo, HediffSet __instance)
    {
      try
      {
#if !RW_1_5



        if (
          hediff != null &&
          hediff.pawn != null &&
          (hediff.pawn != null && hediff.pawn.IsColonist) &&
          GameComponent_ContextTracker.Instance != null)
        {
          var context = TemporalContextCatalog.Create(hediff);
          GameComponent_ContextTracker.Instance.Add(context);
        }
          
#endif
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"An error occurred in HediffSet_AddDirect_Patch.\r\n{ex}", 987234534);
      }
    }
  }
}
