using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_NeutralFactionChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var hasNeutralFactions = Find.FactionManager.GetFactions().Any(faction => faction.PlayerRelationKind == FactionRelationKind.Neutral);
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !hasNeutralFactions)
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Neutral Faction ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.FactionChitChatWeight.Value}");
        return Settings.FactionChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_NeutralFactionChitchat: {ex}");
        return 0f;
      }
    }
  }
}
