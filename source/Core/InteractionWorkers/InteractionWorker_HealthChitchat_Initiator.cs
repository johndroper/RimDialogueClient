using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_HealthChitchat_Initiator : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var hediffs = initiator.health.hediffSet?.hediffs;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          hediffs == null ||
          !hediffs.Any() ||
          lastUsedTicks > GetMinTime())
        {
          Mod.LogV($"Health ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        Mod.LogV($"Health ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.HealthChitChatWeight.Value}");
        return Settings.HealthChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_HealthChitchat: {ex}");
        return 0f;
      }
    }
  }
}
