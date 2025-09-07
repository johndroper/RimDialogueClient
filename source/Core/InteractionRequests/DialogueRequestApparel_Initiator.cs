#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Initiator : DialogueRequestApparel
  {
    //public static new DialogueRequestApparel_Initiator BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestApparel_Initiator(entry);
    //}

    public DialogueRequestApparel_Initiator(
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
          _apparel = Initiator.apparel.WornApparel.RandomElement();
        return _apparel;
      }
    }
  }
}
