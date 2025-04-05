#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Initiator : DialogueRequestAppearance
  {
    public static DialogueRequestAppearance_Initiator BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestAppearance_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestAppearance_Initiator(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public override Pawn Pawn => Initiator;

  }
}
