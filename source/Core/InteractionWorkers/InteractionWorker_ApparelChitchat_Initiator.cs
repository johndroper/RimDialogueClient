using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ApparelChitchat_Initiator : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.apparel == null ||
          initiator.apparel.WornApparel == null ||
          !initiator.apparel.WornApparel.Any() ||
          lastUsedTicks > GetMinTime())
        {
          Mod.LogV($"Initiator Apparel ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        Mod.LogV($"Initiator Apparel ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.ApparelChitChatWeight.Value}");
        return Settings.ApparelChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_ApparelChitchat_Initiator: {ex}");
        return 0f;
      }
    }
  }
}

