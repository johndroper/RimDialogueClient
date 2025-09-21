using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestIdeology : DialogueRequestTwoPawn<DialogueData>
  {
    const string InitiatorIdeologyPlaceholder = "INITIATOR_ideology";
    const string RecipientIdeologyPlaceholder = "RECIPIENT_ideology";

    public DialogueRequestIdeology(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    public override string Action => "IdeologyChitchat";

    public override Rule[] Rules => [
      new Rule_String(InitiatorIdeologyPlaceholder, this.Initiator.Ideo.name),
      new Rule_String(RecipientIdeologyPlaceholder, this.Recipient.Ideo.name)
    ];
  }
}
