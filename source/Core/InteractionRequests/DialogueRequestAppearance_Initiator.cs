#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Initiator : DialogueRequestAppearance
  {
    public static new DialogueRequestAppearance_Initiator BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestAppearance_Initiator(entry);
    }

    public DialogueRequestAppearance_Initiator(PlayLogEntry_Interaction entry) : base(entry)
    {

    }

    public override Pawn Pawn => Initiator;

  }
}
