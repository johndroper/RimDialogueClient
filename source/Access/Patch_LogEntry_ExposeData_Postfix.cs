using HarmonyLib;
using System;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(LogEntry), nameof(LogEntry.ExposeData))]
  public static class Patch_LogEntry_ExposeData_Postfix
  {
    public static void Postfix(LogEntry __instance)
    {
      try
      {
        if (Scribe.mode == LoadSaveMode.PostLoadInit && !GameComponent_ConversationTracker.ExecutedLogEntries.Contains(__instance.LogID))
        {
          GameComponent_ConversationTracker.ExecutedLogEntries.Add(__instance.LogID);
          //if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - loaded.");
        }
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"Entry {__instance.LogID} - An error occurred in Patch_LogEntry_ExposeData_Postfix.\r\n{ex}", 34534532);
      }
    }
  }
}
