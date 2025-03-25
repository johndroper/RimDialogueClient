using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_UnsatisfiedNeedChitchat : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          initiator.needs == null ||
          !initiator.needs.AllNeeds.Where(need => need.CurLevelPercentage < .333f).Any())
          return 0f;

        Mod.LogV($"Unsatisfied Need Weight: {initiator.Name} -> {recipient.Name} = {Settings.NeedChitChatWeight.Value}");
        return Settings.NeedChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_UnsatisfiedNeedChitchat: {ex}");
        return 0f;
      }
    }
  }
}
