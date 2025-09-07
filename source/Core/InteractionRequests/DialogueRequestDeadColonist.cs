#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestDeadColonist : DialogueRequestDeadPawn
  {
    //public static new DialogueRequestDeadColonist BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestDeadColonist(entry);
    //}

    private readonly PawnDeathRecord _pawnDeathRecord;

    public DialogueRequestDeadColonist(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      _pawnDeathRecord = GameComponent_PawnDeathTracker.Instance.DeadColonists.RandomElement();
    }

    public override PawnDeathRecord Record => _pawnDeathRecord;

  }
}
