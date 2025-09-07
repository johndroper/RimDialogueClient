using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(LogEntry), nameof(LogEntry.ExposeData))]
  public static class Patch_LogEntry_ExposeData_Postfix
  {
    public static void Postfix(LogEntry __instance)
    {
      if (Scribe.mode == LoadSaveMode.PostLoadInit && !GameComponent_ConversationTracker.ExecutedLogEntries.Contains(__instance.LogID))
      {
        GameComponent_ConversationTracker.ExecutedLogEntries.Add(__instance.LogID);
        //if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - loaded.");
      }
      
    }
  }
}
