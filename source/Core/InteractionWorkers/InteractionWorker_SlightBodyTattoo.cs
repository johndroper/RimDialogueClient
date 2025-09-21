#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SlightBodyTattoo : InteractionWorker_Dialogue
  {
    private const float BaseSelectionWeight = 0.02f;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        recipient.Inhumanized() ||
        recipient.style == null ||
        recipient.style.BodyTattoo == null ||
        recipient.style.BodyTattoo.defName == "NoTattoo_Body" ||
        recipient.style.BodyTattoo.label == "none" ||
        (initiator.IsSlave && !recipient.IsSlave))
      {
        return 0f;
      }

      return 0.02f
        * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient)
        * Settings.AppearanceChitChatWeight.Value;
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestAppearance_Recipient(entry, intDef, initiator, recipient);
    }
  }
}
