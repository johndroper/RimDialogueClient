using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Initiator : DialogueRequestBattle
  {
    //public static new DialogueRequestBattle_Initiator BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestBattle_Initiator(entry);
    //}

    public DialogueRequestBattle_Initiator(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
       var battles = Find.BattleLog.Battles
        .Where(battle => battle.Concerns(Initiator))
        .OrderByDescending(battle => battle.CreationTimestamp);
      _battle = battles.RandomElementByWeight(
        battle => Math.Max(battle.Importance * (1f / AgeDays(battle)), 1));
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
