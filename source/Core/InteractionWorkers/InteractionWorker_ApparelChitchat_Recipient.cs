using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ApparelChitchat_Recipient : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var weight = base.RandomSelectionWeight(initiator, recipient);
        if (
          recipient.apparel == null ||
          recipient.apparel.WornApparel == null ||
          !recipient.apparel.WornApparel.Any() ||
          lastUsedTicks > GetMinTime())
          return 0f;

        return Settings.ApparelChitChatWeight.Value * weight;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_ApparelChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}

