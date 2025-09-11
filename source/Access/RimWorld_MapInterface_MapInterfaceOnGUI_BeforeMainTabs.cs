using HarmonyLib;
using RimDialogue.Core;
using RimWorld;
using System;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(MapInterface), nameof(MapInterface.MapInterfaceOnGUI_BeforeMainTabs))]
  public static class RimWorld_MapInterface_MapInterfaceOnGUI_BeforeMainTabs
  {
    private static void Postfix()
    {
      try
      {
        if (Settings.DialogueMessageInterface.Value == 2 && GameComponent_ConversationTracker.Instance.DialogueMessagesFixed != null)
          GameComponent_ConversationTracker.Instance.DialogueMessagesFixed.DoGUI();
      }
      catch (Exception exception)
      {
        Mod.ErrorOnce($"Deactivated because draw failed with error: [{exception.Source}: {exception.Message}]\n\nTrace:\n{exception.StackTrace}", 3531243);
        Settings.Activated = false;
      }
    }
  }
}
