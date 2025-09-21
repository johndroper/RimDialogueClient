#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SlightWeapon : InteractionWorker_Dialogue
  {
    private const float BaseSelectionWeight = 0.02f;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        recipient.Inhumanized() ||
        recipient.equipment == null ||
        recipient.equipment.Primary == null ||
        (initiator.IsSlave && !recipient.IsSlave))
      {
        return 0f;
      }

      return 0.02f
        * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient)
        * Settings.WeaponChitChatWeight.Value;
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestWeapon_Recipient(entry, intDef, initiator, recipient);
    }
  }
}
