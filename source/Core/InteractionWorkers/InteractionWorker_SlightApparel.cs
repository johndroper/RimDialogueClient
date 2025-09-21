#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SlightApparel : InteractionWorker_Dialogue
  {
    private const float BaseSelectionWeight = 0.02f;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
          !IsEnabled ||
          (initiator.IsSlave && !recipient.IsSlave) ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          !recipient.apparel.AnyApparel)
        return 0f;

      return 0.02f
        * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient)
        * Settings.ApparelChitChatWeight.Value;
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestApparel_Recipient(entry, intDef, initiator, recipient);
    }
  }
}
