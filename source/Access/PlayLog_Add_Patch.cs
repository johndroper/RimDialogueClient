#nullable enable
using HarmonyLib;
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static System.Net.Mime.MediaTypeNames;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(PlayLog), nameof(PlayLog.Add))]
  public static class PlayLog_Add_Patch
  {
    static void Postfix(LogEntry entry)
    {
      try
      {
#if !RW_1_5
        if (GameComponent_ContextTracker.Instance != null)
          GameComponent_ContextTracker.Instance.Add(entry);
        if (Settings.VerboseLogging.Value) 
          Mod.Log($"Entry {entry.LogID} - Added to context DB.");
#endif
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"Entry {entry.LogID} - An error occurred in PlayLog_Add_Patch.\r\n{ex}", 345974);
      }
    }
  }
}
