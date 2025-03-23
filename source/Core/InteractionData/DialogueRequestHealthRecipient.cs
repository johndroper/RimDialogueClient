using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthRecipient : DialogueRequestHealth<DialogueDataHealth>
  {
    public static DialogueRequestHealthRecipient BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestHealthRecipient(entry, interactionTemplate);
    }

    public DialogueRequestHealthRecipient(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Hediff = this.Recipient.health.hediffSet.hediffs.RandomElement();
    }
  }
}
