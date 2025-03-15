using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_Dialogue : InteractionWorker
  {

    private static DateTime? LastConversation = null;

    public static bool IsEnabled
    {
      get
      {
        if (Find.TickManager.CurTimeSpeed > (TimeSpeed)Settings.MaxSpeed.Value)
        {
          Mod.LogV($"Game speed is too high.");
          return false;
        }
        if (Settings.MinTimeBetweenConversations.Value > 0 && LastConversation.HasValue && DateTime.Now - LastConversation.Value < TimeSpan.FromSeconds(Settings.MinTimeBetweenConversations.Value))
        {
          Mod.LogV($"Too soon since last conversation.");
          return false;
        }
        LastConversation = DateTime.Now;
        return true;
      }
    }

    public static int GetMinTime()
    {
      return Find.TickManager.TicksAbs - 2500 * Settings.ChitChatMinHours.Value;
    }
  }
}
