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
  [HarmonyPatch(typeof(Pawn_EquipmentTracker))]
  public static class Patch_Pawn_EquipmentTracker
  {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Pawn_EquipmentTracker.Notify_EquipmentAdded))]
    public static void Prefix_Notify_EquipmentAdded(Pawn_EquipmentTracker __instance, ThingWithComps eq)
    {
      DynamicContextData<ThingWithComps>.CreateContext(eq, __instance.pawn, (ThingWithComps) => DynamicContextData.GetEquipmentText(eq, __instance.pawn));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Pawn_EquipmentTracker.Notify_EquipmentRemoved))]
    public static void Prefix_Notify_EquipmentRemoved(Pawn_EquipmentTracker __instance, ThingWithComps eq)
    {
      DynamicContextData<ThingWithComps>.ExpireContext(eq);
    }
  }
}
