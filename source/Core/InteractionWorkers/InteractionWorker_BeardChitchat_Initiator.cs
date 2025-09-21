#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_BeardChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.style == null ||
          initiator.style.beardDef == null ||
          initiator.style.beardDef.defName == "NoBeard" ||
          initiator.style.beardDef.label == "none")
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Beard ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Beard ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_BeardChitchat_Initiator: {ex}");
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
      return new DialogueRequestAppearance_Initiator(entry, intDef, initiator, recipient);
    }
  }
}
