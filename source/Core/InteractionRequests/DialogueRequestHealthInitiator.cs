#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthInitiator : DialogueRequestHealth
  {
    //public static new DialogueRequestHealthInitiator? BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  var request = new DialogueRequestHealthInitiator(entry);
    //  if (request.Hediff == null)
    //    return null;
    //  return request;
    //}

    public DialogueRequestHealthInitiator(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    private Hediff? _hediff;
    public override Hediff? Hediff
    {
      get
      {
        if (_hediff == null && Initiator.health.hediffSet.hediffs.Any())
          _hediff = Initiator.health.hediffSet.hediffs.RandomElement();
        return _hediff;
      }
    }
  }
}
