#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestDeadColonist : DialogueRequestDeadPawn
  {
    public static new DialogueRequestDeadColonist BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestDeadColonist(entry);
    }

    private readonly PawnDeathRecord _pawnDeathRecord;

    public DialogueRequestDeadColonist(PlayLogEntry_Interaction entry) : base(entry)
    {
      _pawnDeathRecord = GameComponent_PawnDeathTracker.Instance.DeadColonists.RandomElement();
    }

    public override PawnDeathRecord Record => _pawnDeathRecord;

  }
}
