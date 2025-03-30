using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SkillChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.skills == null ||
          !initiator.skills.skills.Any())
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Skill ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.SkillChitChatWeight.Value}");
        return Settings.SkillChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_SkillChitchat: {ex}");
        return 0f;
      }
    }
  }
}

