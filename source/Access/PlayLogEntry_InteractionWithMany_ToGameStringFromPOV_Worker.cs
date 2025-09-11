using HarmonyLib;
using RimDialogue;
using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;
using Mod = RimDialogue.Mod;

[HarmonyPatch(typeof(PlayLogEntry_InteractionWithMany), "ToGameStringFromPOV_Worker")]
public static class PlayLogEntry_InteractionWithMany_ToGameStringFromPOV_Worker
{
  public static void Postfix(PlayLogEntry_InteractionWithMany __instance, ref string __result, ref Pawn pov, ref bool forceLog)
  {
    try
    {
      //if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - PlayLogEntry_InteractionWithMany Original interaction: '{__result}'");
      var dialogueRequest = DialogueRequest.Create(
      __instance,
      __result,
      (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(__instance));
      if (dialogueRequest != null)
        dialogueRequest.Execute(__result);
      //if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - New {dialogueRequest.GetType().Name} interaction: '{__result}'");
    }
    catch (Exception ex)
    {
      Mod.ErrorOnce($"Entry {__instance.LogID} - An error occurred in PlayLogEntry_InteractionWithMany_ToGameStringFromPOV_Worker.\r\n{ex}", 6756423);
    }
  }
}
