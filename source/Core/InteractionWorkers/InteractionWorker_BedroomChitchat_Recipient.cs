using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_BedroomChitchat_Recipient : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var bedroom = recipient.ownership?.OwnedRoom;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          initiator.IsOutside() ||
          recipient.IsOutside() ||
          bedroom == null)
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"{nameof(InteractionWorker_BedroomChitchat_Recipient)} Weight: {initiator.Name} -> {recipient.Name} = {Settings.RoomChitChatWeight.Value}");
        return Settings.RoomChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in {nameof(InteractionWorker_BedroomChitchat_Recipient)}: {ex}");
        return 0f;
      }
    }
  }
}
