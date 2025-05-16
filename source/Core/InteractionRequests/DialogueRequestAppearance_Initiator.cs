#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Initiator : DialogueRequestAppearance
  {
    public static new DialogueRequestAppearance_Initiator BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestAppearance_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestAppearance_Initiator(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public override Pawn Pawn => Initiator;

  }
}
