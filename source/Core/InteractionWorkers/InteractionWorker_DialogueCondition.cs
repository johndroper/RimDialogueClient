using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueCondition : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !Find.CurrentMap.GameConditionManager.ActiveConditions.Any())
        {
          return 0f;
        }
        Mod.LogV($"Condition ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.GameConditionChitChatWeight.Value}");
        return Settings.GameConditionChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
        return 0f;
      }
    }
  }
}
