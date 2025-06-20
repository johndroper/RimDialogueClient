using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_UnsatisfiedNeedChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.needs == null ||
          !initiator.needs.AllNeeds.Where(need => need.CurLevelPercentage < .333f).Any())
          return 0f;

        // if (Settings.VerboseLogging.Value) Mod.Log($"Unsatisfied Need Weight: {initiator.Name} -> {recipient.Name} = {Settings.NeedChitChatWeight.Value}");
        return Settings.NeedChitChatWeight.Value * 5;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_UnsatisfiedNeedChitchat: {ex}");
        return 0f;
      }
    }
  }
}
