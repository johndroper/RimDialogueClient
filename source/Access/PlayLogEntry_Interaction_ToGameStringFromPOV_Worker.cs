#nullable enable

using HarmonyLib;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(PlayLogEntry_Interaction), "ToGameStringFromPOV_Worker")]
  public static class PlayLogEntry_Interaction_ToGameStringFromPOV_Worker
  {
    public static void Postfix(PlayLogEntry_Interaction __instance, ref string __result, ref Pawn pov, ref bool forceLog)
    {
      try
      {
        if (Settings.OnlyColonists.Value && !pov.IsColonist)
          return;
        if (Settings.VerboseLogging.Value) Mod.Log($"Original interaction log for log entry {__instance.LogID}: {__result}");
        var dialogueRequest = DialogueRequest.Create(
        ref __instance,
        ref __result,
        (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(__instance));
        __result = dialogueRequest.GetInteraction();
        if (Settings.VerboseLogging.Value) Mod.Log($"New {dialogueRequest.GetType().Name} interaction log for log entry {__instance.LogID}: {__result}");
      }
      catch (Exception ex)
      {
        Mod.Error($"An error occurred in PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.\r\n{ex}");
      }
    }
  }
}
