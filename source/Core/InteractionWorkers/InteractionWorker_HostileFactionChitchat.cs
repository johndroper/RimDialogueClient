#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_HostileFactionChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var hasHostileFactions = Find.FactionManager.GetFactions().Any(faction => faction.PlayerRelationKind == FactionRelationKind.Hostile);
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !hasHostileFactions)
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Hostile Faction ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.FactionChitChatWeight.Value}");
        return Settings.FactionChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_HostileFactionChitchat: {ex}");
        return 0f;
      }
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestHostileFaction(entry, intDef, initiator, recipient);
    }
  }
}
