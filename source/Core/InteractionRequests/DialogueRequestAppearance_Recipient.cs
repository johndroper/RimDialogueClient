#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Recipient : DialogueRequestAppearance
  {
    //public static new DialogueRequestAppearance_Recipient BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestAppearance_Recipient(entry);
    //}

    public DialogueRequestAppearance_Recipient(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    public override Pawn Pawn => Recipient;

  }
}
