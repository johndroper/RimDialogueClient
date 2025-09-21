#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_RoomChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var initiatorRoom = initiator.GetRoom();
        var recipientRoom = recipient.GetRoom();
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.IsOutside() ||
          recipient.IsOutside() ||
          initiatorRoom == null ||
          recipientRoom == null ||
          initiatorRoom != recipientRoom)
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Room ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.RoomChitChatWeight.Value}");
        return Settings.RoomChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_RoomChitchat: {ex}");
        return 0f;
      }
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestRoom(entry, intDef, initiator, recipient);
    }
  }
}
