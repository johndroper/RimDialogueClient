#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Recipient : DialogueRequestAppearance
  {
    public static new DialogueRequestAppearance_Recipient BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestAppearance_Recipient(entry, interactionTemplate);
    }

    public DialogueRequestAppearance_Recipient(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public override Pawn Pawn => Recipient;

  }
}
