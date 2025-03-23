using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueBattle : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        !initiator.IsColonist ||
        !recipient.IsColonist ||
        !H.GetRecentBattles(Settings.RecentBattleHours.Value).Any() ||
        lastUsedTicks > GetMinTime())
      {
        Mod.LogV($"Battle ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
        return 0f;
      }
      Mod.LogV($"Battle ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.RecentBattleChitChatWeight.Value}");
      return Settings.RecentBattleChitChatWeight.Value;
    }
  }
}


