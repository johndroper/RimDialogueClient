using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ApparelChitchat_Recipient : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          recipient.apparel == null ||
          recipient.apparel.WornApparel == null ||
          !recipient.apparel.WornApparel.Any())
          return 0f;

        // if (Settings.VerboseLogging.Value) Mod.Log($"Apparel Weight: {initiator.Name} -> {recipient.Name} = {Settings.ApparelChitChatWeight.Value}");
        return Settings.ApparelChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_ApparelChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}

