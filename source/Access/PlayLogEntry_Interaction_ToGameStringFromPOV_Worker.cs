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
        if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Original interaction: '{__result}'");
        var dialogueRequest = DialogueRequest.Create(
        __instance,
        __result,
        (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(__instance));
        __result = dialogueRequest.GetInteraction();
        if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - New {dialogueRequest.GetType().Name} interaction: '{__result}'");
      }
      catch (Exception ex)
      {
        Mod.Error($"Entry {__instance.LogID} - An error occurred in PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.\r\n{ex}");
      }
    }
  }
}
