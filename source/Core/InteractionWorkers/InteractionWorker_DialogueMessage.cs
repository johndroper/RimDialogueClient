using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueMessage : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !Verse_Messages_Message.RecentMessages.Any() ||
          lastUsedTicks > GetMinTime())
        {
          Mod.LogV($"Message ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        Mod.LogV($"Message ChitChat Weight: {initiator.Name} -> {recipient.Name} = 1");
        return Settings.MessageChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
        return 0f;
      }
    }
  }
}
