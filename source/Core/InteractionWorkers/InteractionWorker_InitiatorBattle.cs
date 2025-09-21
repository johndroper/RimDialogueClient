#nullable enable
using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System;
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
      // if (Settings.VerboseLogging.Value) Mod.Log($"${nameof(InteractionWorker_InitiatorBattle)}: {initiator.Name} -> {recipient.Name} = {Settings.BattleChitChatWeight.Value}");
      return Settings.BattleChitChatWeight.Value;
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestBattle_Initiator(entry, intDef, initiator, recipient);
    }
  }
}


