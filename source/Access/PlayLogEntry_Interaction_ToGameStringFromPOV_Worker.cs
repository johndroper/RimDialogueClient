using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimDialogue.Core;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(PlayLogEntry_Interaction), "ToGameStringFromPOV_Worker")]
  public static class PlayLogEntry_Interaction_ToGameStringFromPOV_Worker
  {
    const string RecentIncidentPlaceholder = "**recent_incident**";
    const string RecentBattlePlaceholder = "**recent_battle**";

    static Dictionary<int, Battle> recentBattles = [];
    public static Battle GetRecentBattle(int logId, ref Pawn pov)
    {
      if (recentBattles.ContainsKey(logId))
        return recentBattles[logId];
      var battle = DataHelper.GetMostRecentBattle(pov);
      if (battle != null)
        recentBattles.Add(logId, battle);
      return battle;
    }

    public static void Postfix(PlayLogEntry_Interaction __instance, ref string __result, ref Pawn pov, ref bool forceLog)
    {
      Mod.LogV($"Original interaction log for log entry {__instance.LogID}: {__result}");
      var interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(__instance);
      switch(interactionDef.defName)
      {
        case "RecentIncidentChitchat":
          var letter = Verse_LetterMaker_MakeLetter.GetLetter(__instance.LogID);
          if (letter != null)
            __result = __result.Replace(RecentIncidentPlaceholder, letter.Label.Replace(".", "").UncapitalizeFirst());
          else
          {
            Mod.LogV($"No recent letters to choose from for log entry {__instance.LogID}.");
            __result = __result.Replace(RecentIncidentPlaceholder, "a recent incident");
          }
          break;
        case "RecentBattleChitchat":
          var battle = GetRecentBattle(__instance.LogID, ref pov);
          if (battle != null)
            __result = __result.Replace(RecentBattlePlaceholder, battle.GetName());
          else
          {
            Mod.LogV($"No recent battles to choose from for log entry {__instance.LogID}.");
            __result = __result.Replace(RecentBattlePlaceholder, "a recent battle");
          }
          break;
        default:
          break;
      }
      Mod.LogV($"Modified interaction log for log entry {__instance.LogID}: {__result}");
    }
  }
}
