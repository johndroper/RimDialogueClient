using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_BedroomChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var initiatorBedroom = initiator.ownership?.OwnedRoom;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          initiator.IsOutside() ||
          recipient.IsOutside() ||
          initiatorBedroom == null)
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"{nameof(InteractionWorker_BedroomChitchat_Initiator)} Weight: {initiator.Name} -> {recipient.Name} = {Settings.RoomChitChatWeight.Value}");
        return Settings.RoomChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in {nameof(InteractionWorker_BedroomChitchat_Initiator)}: {ex}");
        return 0f;
      }
    }
  }
}
