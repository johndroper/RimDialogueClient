using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestIdeology<DataT> : DialogueRequest<DataT> where DataT : DialogueData, new()
  {
    const string InitiatorIdeologyPlaceholder = "**INITIATOR_ideology**";
    const string RecipientIdeologyPlaceholder = "**RECIPIENT_ideology**";

    public static DialogueRequestIdeology<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestIdeology<DataT>(entry, interactionTemplate);
    }

    public DialogueRequestIdeology(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public override string Action => "IdeologyChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(InitiatorIdeologyPlaceholder, this.Initiator.Ideo.name)
        .Replace(RecipientIdeologyPlaceholder, this.Recipient.Ideo.name);
    }
  }
}
