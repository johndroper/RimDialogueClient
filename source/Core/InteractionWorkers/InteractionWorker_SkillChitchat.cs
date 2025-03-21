using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SkillChitchat : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.skills == null ||
          !initiator.skills.skills.Any() ||
          lastUsedTicks > GetMinTime())
        {
          Mod.LogV($"Skill ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        Mod.LogV($"Skill ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.SkillChitChatWeight.Value}");
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

