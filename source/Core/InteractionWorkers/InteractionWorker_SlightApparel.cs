using RimWorld;
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
  }
}
