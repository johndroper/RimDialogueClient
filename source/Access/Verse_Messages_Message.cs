using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch]
  public static class Verse_Messages_Message
  {
    public static List<Message> RecentMessages { get; set; } = [];

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
      RecentMessages.Add(msg);
      if (RecentMessages.Count > 10)
      {
        RecentMessages.RemoveAt(0);
      }
    }
  }
}
