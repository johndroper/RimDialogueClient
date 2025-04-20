using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SlightFaceTattoo : InteractionWorker_Dialogue
  {
    private const float BaseSelectionWeight = 0.02f;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        recipient.style == null ||
        recipient.style.FaceTattoo == null ||
        recipient.style.FaceTattoo.defName == "NoTattoo_Face" ||
        recipient.style.FaceTattoo.label == "none" ||
        (initiator.IsSlave && !recipient.IsSlave))
      {
        return 0f;
      }

      return 0.02f
        * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient)
        * Settings.AppearanceChitChatWeight.Value;
    }
  }
}
