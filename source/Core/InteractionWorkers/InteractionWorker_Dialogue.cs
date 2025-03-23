using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_Dialogue : InteractionWorker
  {

    private static DateTime? LastConversation = null;
    public static int GetMinTime()
    {
      return Find.TickManager.TicksAbs - (int)(41.6667f * Settings.ChitChatMinMinutes.Value);
    }

    public static bool IsEnabled
    {
      get
      {
        if (Find.TickManager.CurTimeSpeed > (TimeSpeed)Settings.MaxSpeed.Value)
        {
          Mod.LogV($"Game speed is too high.");
          return false;
        }
        if (
          Settings.MinTimeBetweenConversations.Value > 0 &&
          LastConversation.HasValue &&
          DateTime.Now - LastConversation.Value < TimeSpan.FromSeconds(Settings.MinTimeBetweenConversations.Value))
        {
          Mod.LogV($"Too soon since last conversation.");
          return false;
        }
        LastConversation = DateTime.Now;
        return true;
      }
    }

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        recipient.Inhumanized())
        return 0f;

      return 1f;
    }
  }
}
