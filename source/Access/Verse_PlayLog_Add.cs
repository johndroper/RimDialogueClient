using Bubbles.Core;
using HarmonyLib;
using System;
using Verse;

namespace Bubbles.Access
{
  [HarmonyPatch(typeof(PlayLog), nameof(PlayLog.Add))]
  public static class Verse_PlayLog_Add
  {
    private static void Postfix(LogEntry entry)
    {
      try
      {
        Bubbler.Add(entry);
      }
      catch(Exception exception)
      {
        Mod.Error($"Deactivated because add failed with error: [{exception.Source}: {exception.Message}]\n\nTrace:\n{exception.StackTrace}");
        Settings.Activated = false;
      }
    }
  }
}
