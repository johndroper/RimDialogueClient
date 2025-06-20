using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_HealthChitchat_Recipient : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var hediffs = recipient.health.hediffSet?.hediffs;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          hediffs == null ||
          !hediffs.Any())
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Recipient Health ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.HealthChitChatWeight.Value}");
        return Settings.HealthChitChatWeight.Value * 5;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_HealthChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}

