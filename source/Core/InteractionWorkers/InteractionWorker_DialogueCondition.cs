#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueCondition : InteractionWorker_Dialogue
  {
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
        // if (Settings.VerboseLogging.Value) Mod.Log($"Condition ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.GameConditionChitChatWeight.Value}");
        return Settings.GameConditionChitChatWeight.Value * Settings.TimelyEventWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
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
      return new DialogueRequestCondition(entry, intDef, initiator, recipient);
    }
  }
}
