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
  [HarmonyPatch(typeof(Pawn))]
  [HarmonyPatch(nameof(Pawn.SetFaction))]
  public static class Patch_Pawn_SetFaction_JoinColony
  {
    static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter, out (bool wasColonist, Faction oldFaction) __state)
    {
      __state = (__instance.IsColonist, __instance.Faction);
    }

    static void Postfix(Pawn __instance, Faction newFaction, Pawn recruiter, (bool wasColonist, Faction oldFaction) __state)
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
  }
}
