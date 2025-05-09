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
  }
}
