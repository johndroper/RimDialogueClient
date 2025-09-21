#nullable enable
using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DeadColonistDeepTalk : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {

      try
      {
        if (!IsEnabled || initiator.Inhumanized() || recipient.Inhumanized())
        {
          return 0f;
        }

        if (GameComponent_PawnDeathTracker.Instance.DeadColonists.Any())
          return Settings.DeadColonistWeight.Value;

        return 0f;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_DeadColonist: {ex}");
        return 0f;
      }
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestDeadColonist(entry, intDef, initiator, recipient);
    }
  }
}
