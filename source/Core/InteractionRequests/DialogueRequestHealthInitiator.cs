#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthInitiator : DialogueRequestHealth
  {
    public static new DialogueRequestHealthInitiator BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestHealthInitiator(entry);
    }

    public DialogueRequestHealthInitiator(PlayLogEntry_Interaction entry) :
      base(entry)
    {

    }

    private Hediff? _hediff;
    public override Hediff Hediff
    {
      get
      {
        if (_hediff == null)
          _hediff = Initiator.health.hediffSet.hediffs.RandomElement();
        return _hediff;
      }
    }
  }
}
