using HarmonyLib;
using RimDialogue;
using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;
using Mod = RimDialogue.Mod;

[HarmonyPatch(typeof(PlayLogEntry_InteractionSinglePawn), "ToGameStringFromPOV_Worker")]
public static class PlayLogEntry_Interaction_ToGameStringFromPOV_Worker
{
  public static void Postfix(PlayLogEntry_InteractionSinglePawn __instance, ref string __result, ref Pawn pov, ref bool forceLog)
  {
    try
    {
      if (Settings.OnlyColonists.Value && !pov.IsColonist)
        return;

      if (DialogueRequest.TooSoon() || DialogueRequest.TooSoonAll())
        return;

      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - PlayLogEntry_InteractionSinglePawn Original interaction: '{__result}'");
      var dialogueRequest = DialogueRequest.Create(
      __instance,
      (InteractionDef)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_InteractionDef.GetValue(__instance));
      dialogueRequest.Execute(__result);

      //__result = dialogueRequest.GetInteraction();
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - New {dialogueRequest.GetType().Name} interaction: '{__result}'");


    }
    catch (Exception ex)
    {
      Mod.Error($"Entry {__instance.LogID} - An error occurred in PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.\r\n{ex}");
    }
  }
}
