#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthRecipient : DialogueRequestHealth
  {
    //public static new DialogueRequestHealthRecipient BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestHealthRecipient(entry);
    //}

    public DialogueRequestHealthRecipient(
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
        if (_hediff == null && Recipient.health.hediffSet.hediffs.Any())
          _hediff = Recipient.health.hediffSet.hediffs.RandomElement();
        return _hediff;
      }
    }
  }
}
