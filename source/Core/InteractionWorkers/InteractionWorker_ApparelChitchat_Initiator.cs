#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ApparelChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.apparel == null ||
          initiator.apparel.WornApparel == null ||
          !initiator.apparel.WornApparel.Any())
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Apparel ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Apparel ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.ApparelChitChatWeight.Value}");
        return Settings.ApparelChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_ApparelChitchat_Initiator: {ex}");
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
      return new DialogueRequestApparel_Initiator(entry, intDef, initiator, recipient);
    }
  }
}

