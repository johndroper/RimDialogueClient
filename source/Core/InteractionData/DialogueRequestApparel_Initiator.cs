using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Initiator : DialogueRequestApparel<DialogueDataApparel>
  {
    public static DialogueRequestApparel_Initiator BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestApparel_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestApparel_Initiator(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Apparel = this.Initiator.apparel?.WornApparel.RandomElement();
    }
  }
}
