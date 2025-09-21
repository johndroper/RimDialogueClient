#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_HealthChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var hediffs = initiator.health.hediffSet?.hediffs;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          hediffs == null ||
          !hediffs.Any())
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Health ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.HealthChitChatWeight.Value}");
        return Settings.HealthChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_HealthChitchat: {ex}");
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
      return new DialogueRequestHealthInitiator(entry, intDef, initiator, recipient);
    }
  }
}
