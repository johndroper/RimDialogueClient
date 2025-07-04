#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Recipient : DialogueRequestAppearance
  {
    public static new DialogueRequestAppearance_Recipient BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestAppearance_Recipient(entry);
    }

    public DialogueRequestAppearance_Recipient(PlayLogEntry_Interaction entry) : base(entry)
    {

    }

    public override Pawn Pawn => Recipient;

  }
}
