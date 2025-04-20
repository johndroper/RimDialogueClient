using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_InitiatorBattle : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        !initiator.IsColonist ||
        !recipient.IsColonist ||
        !Find.BattleLog.Battles.Where(battle => battle.Concerns(initiator)).Any())
      {
        return 0f;
      }
      if (Settings.VerboseLogging.Value) Mod.Log($"${nameof(InteractionWorker_InitiatorBattle)}: {initiator.Name} -> {recipient.Name} = {Settings.BattleChitChatWeight.Value}");
      return Settings.BattleChitChatWeight.Value;
    }
  }
}


