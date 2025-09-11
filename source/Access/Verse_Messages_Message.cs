using HarmonyLib;
using RimDialogue.Core;
using System.Reflection;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch]
  public static class Verse_Messages_Message
  {
    public static MethodBase TargetMethod()
    {
      return AccessTools.Method(typeof(Messages), "Message",
          new[] {
            typeof(Message),
            typeof(bool)
          });
    }

    public static void Prefix(Message msg, bool historical = true)
    {
      try
      {
        if (GameComponent_MessageTracker.Instance != null)
          GameComponent_MessageTracker.Instance.AddMessage(msg);
      }
      catch (System.Exception ex)
      {
        Mod.ErrorOnce($"An error occurred in Verse_Messages_Message.\r\n{ex}", 901820232);
      }
    }
  }
}
