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
      GameComponent_MessageTracker.Instance.AddMessage(msg);
    }
  }
}
