using HarmonyLib;
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(HediffSet), "AddDirect")]
  public static class HediffSet_AddDirect_Patch
  {
    [HarmonyPostfix]
    public static void Postfix(Hediff hediff, DamageInfo? dinfo, HediffSet __instance)
    {
      if (hediff != null && hediff.pawn != null && hediff.pawn.IsColonist)
        GameComponent_ContextTracker.Instance.Add(hediff);
    }
  }
}
