using HarmonyLib;
using RimDialogue.Context;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(Pawn_ApparelTracker))]
  public static class Patch_Pawn_ApparelTracker
  {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Pawn_ApparelTracker.Notify_ApparelAdded))]
    public static void Prefix_Notify_ApparelAdded(Pawn_ApparelTracker __instance, Apparel apparel)
    {
      DynamicContextData<Apparel>.CreateContext(apparel, __instance.pawn, (apparel) => DynamicContextData.GetApparelText(apparel, __instance.pawn));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Pawn_ApparelTracker.Notify_ApparelRemoved))]
    public static void Prefix_Notify_ApparelRemoved(Apparel apparel)
    {
      DynamicContextData<Apparel>.ExpireContext(apparel);
    }
  }
}
