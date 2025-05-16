#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthRecipient : DialogueRequestHealth
  {
    public static new DialogueRequestHealthRecipient BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestHealthRecipient(entry, interactionTemplate);
    }

    public DialogueRequestHealthRecipient(PlayLogEntry_Interaction entry, string interactionTemplate) :
      base(entry, interactionTemplate)
    {

    }

    private Hediff? _hediff;
    public override Hediff Hediff
    {
      get
      {
        if (_hediff == null)
          _hediff = Recipient.health.hediffSet.hediffs.RandomElement();
        return _hediff;
      }
    }
  }
}
