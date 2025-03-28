using RimDialogue.Access;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueAlert : InteractionWorker_Dialogue
  {

    public static int lastUsedTicks = 0;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var alerts = (List<Alert>)Reflection.RimWorld_AlertsReadout_ActiveAlerts.GetValue(Find.Alerts);
        if (alerts == null)
        {
          Mod.WarningV($"InteractionWorker_AlertChitChat alerts are null.");
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
        Mod.LogV($"Alert ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AlertChitChatWeight.Value}");
        return Settings.AlertChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_AlertChitChat: {ex}");
        return 0f;
      }
    }
  }
}
