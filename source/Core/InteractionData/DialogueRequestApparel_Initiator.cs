#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Initiator : DialogueRequestApparel<DialogueDataApparel>
  {
    public static DialogueRequestApparel_Initiator BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestApparel_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestApparel_Initiator(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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
