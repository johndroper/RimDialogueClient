#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAppearance_Recipient : DialogueRequestAppearance
  {
    public static DialogueRequestAppearance_Recipient BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestAppearance_Recipient(entry, interactionTemplate);
    }

    public DialogueRequestAppearance_Recipient(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public override Pawn Pawn => Recipient;

  }
}
