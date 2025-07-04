using RimDialogue.Core.InteractionRequests;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestIdeology<DataT> : DialogueRequestTwoPawn<DataT> where DataT : DialogueData, new()
  {
    const string InitiatorIdeologyPlaceholder = "INITIATOR_ideology";
    const string RecipientIdeologyPlaceholder = "RECIPIENT_ideology";

    public static new DialogueRequestIdeology<DataT> BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestIdeology<DataT>(entry);
    }

    public DialogueRequestIdeology(PlayLogEntry_Interaction entry) : base(entry)
    {

    }

    public override string Action => "IdeologyChitchat";

    public override Rule[] Rules => [
      new Rule_String(InitiatorIdeologyPlaceholder, this.Initiator.Ideo.name),
      new Rule_String(RecipientIdeologyPlaceholder, this.Recipient.Ideo.name)
    ];
  }
}
