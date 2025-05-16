#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Initiator : DialogueRequestApparel
  {
    public static new DialogueRequestApparel_Initiator BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestApparel_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestApparel_Initiator(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
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
