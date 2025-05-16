#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestDeadColonist : DialogueRequestDeadPawn
  {
    public static new DialogueRequestDeadColonist BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestDeadColonist(entry, interactionTemplate);
    }

    private readonly PawnDeathRecord _pawnDeathRecord;

    public DialogueRequestDeadColonist(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      _pawnDeathRecord = GameComponent_PawnDeathTracker.Instance.DeadColonists.RandomElement();
    }

    public override PawnDeathRecord Record => _pawnDeathRecord;

  }
}
