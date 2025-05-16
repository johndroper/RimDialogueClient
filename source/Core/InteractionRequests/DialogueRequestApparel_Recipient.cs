#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Recipient : DialogueRequestApparel
  {
    public static new DialogueRequestApparel_Recipient BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestApparel_Recipient(entry, interactionTemplate);
    }

    public DialogueRequestApparel_Recipient(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Apparel? _apparel = null;
    public override Apparel Apparel
    {
      get
      {
        if (_apparel == null)
          _apparel = Recipient.apparel.WornApparel.RandomElement();
        return _apparel;
      }
    }
  }
}

