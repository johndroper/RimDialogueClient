#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Recipient : DialogueRequestApparel
  {
    //public static new DialogueRequestApparel_Recipient BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestApparel_Recipient(entry);
    //}

    public DialogueRequestApparel_Recipient(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    private Apparel? _apparel = null;
    public override Apparel Apparel
    {
      get
      {
        if (_apparel == null)
          _apparel = Recipient.apparel.WornApparel.RandomElement();
        return _apparel;
      }
    }
  }
}

