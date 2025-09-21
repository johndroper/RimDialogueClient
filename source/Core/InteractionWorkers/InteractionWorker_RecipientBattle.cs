#nullable enable
using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_RecipientBattle : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (
        !IsEnabled ||
        initiator.Inhumanized() ||
        !initiator.IsColonist ||
        !recipient.IsColonist ||
        !Find.BattleLog.Battles.Where(battle => battle.Concerns(recipient)).Any())
      {
        return 0f;
      }
      // if (Settings.VerboseLogging.Value) Mod.Log($"${nameof(InteractionWorker_RecipientBattle)}: {initiator.Name} -> {recipient.Name} = {Settings.BattleChitChatWeight.Value}");
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
      return new DialogueRequestBattle_Recipient(entry, intDef, initiator, recipient);
    }
  }
}


