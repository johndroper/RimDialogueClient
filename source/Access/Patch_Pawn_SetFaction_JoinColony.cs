#nullable enable
using HarmonyLib;
using RimDialogue.Core;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(Pawn))]
  [HarmonyPatch(nameof(Pawn.SetFaction))]
  public static class Patch_Pawn_SetFaction_JoinColony
  {
    static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter, out (bool wasColonist, Faction? oldFaction) __state)
    {
      try
      {
        __state = (__instance.IsColonist, __instance.Faction);
      }
      catch (Exception ex)
      {
        Mod.Error($"An error occurred in Patch_Pawn_SetFaction_JoinColony.\r\n{ex}");
        __state = (false, null);
      }
    }

    static void Postfix(Pawn __instance, Faction newFaction, Pawn recruiter, (bool wasColonist, Faction? oldFaction) __state)
    {
      try
      {
        if (__instance.RaceProps?.Humanlike == true
            && newFaction == Faction.OfPlayer
            && !__state.wasColonist
            && __instance.IsFreeColonist
            && __instance.HostFaction == null) // not a guest
        {
          var pawnData = __instance.MakeData(null, -1);
          GameComponent_ConversationTracker.Instance.
            GetInstructionsAsync(pawnData);
        }
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"An error occurred in Patch_Pawn_SetFaction_JoinColony.\r\n{ex}", 92383475);
      }
    }
  }
}
