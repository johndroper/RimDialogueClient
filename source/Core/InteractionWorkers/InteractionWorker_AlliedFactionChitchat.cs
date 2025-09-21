#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
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
        //if (Settings.VerboseLogging.Value) Mod.Log($"Allied Faction ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.FactionChitChatWeight.Value}");
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
        return Settings.FactionChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_AlliedFactionChitchat: {ex}");
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
      return new DialogueRequestAlliedFaction(entry, intDef, initiator, recipient);
    }
  }
}


