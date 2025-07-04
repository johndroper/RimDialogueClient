using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueBattle : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        !initiator.IsColonist ||
        !recipient.IsColonist ||
        !Find.BattleLog.Battles.Any())
        return 0f;
      // if (Settings.VerboseLogging.Value) Mod.Log($"Battle ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.BattleChitChatWeight.Value}");
      return Settings.BattleChitChatWeight.Value;
    }
  }
}


