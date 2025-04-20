using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_AlliedFactionChitchat : InteractionWorker_Dialogue
  {

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var hasAlliedFactions = Find.FactionManager.GetFactions().Any(faction => faction.PlayerRelationKind == FactionRelationKind.Ally);
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !hasAlliedFactions)
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Allied Faction ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.FactionChitChatWeight.Value}");
        return Settings.FactionChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_AlliedFactionChitchat: {ex}");
        return 0f;
      }
    }
  }
}
