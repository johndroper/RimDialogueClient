#nullable enable
using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueAlert : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var alerts = (List<Alert>)Reflection.RimWorld_AlertsReadout_ActiveAlerts.GetValue(Find.Alerts);
        if (alerts == null)
        {
          Mod.Warning($"InteractionWorker_AlertChitChat alerts are null.");
          return 0f;
        }
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !alerts.Any())
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Alert ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AlertChitChatWeight.Value}");
        return Settings.AlertChitChatWeight.Value * Settings.TimelyEventWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_AlertChitChat: {ex}");
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
      return new DialogueRequestAlert(entry, intDef, initiator, recipient);
    }
  }
}
