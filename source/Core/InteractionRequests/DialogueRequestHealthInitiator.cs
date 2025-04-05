#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthInitiator : DialogueRequestHealth
  {
    public static DialogueRequestHealthInitiator BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestHealthInitiator(entry, interactionTemplate);
    }

    public DialogueRequestHealthInitiator(LogEntry entry, string interactionTemplate) :
      base(entry, interactionTemplate)
    {

    }

    private Hediff? _hediff;
    public override Hediff Hediff {
      get
      {
        if (_hediff == null)
          _hediff = Initiator.health.hediffSet.hediffs.RandomElement();
        return _hediff;
      }
    }
  }
}
