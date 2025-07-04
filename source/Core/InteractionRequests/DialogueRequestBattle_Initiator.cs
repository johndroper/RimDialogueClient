using RimDialogue.Core.InteractionData;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Initiator : DialogueRequestBattle
  {
    public static new DialogueRequestBattle_Initiator BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestBattle_Initiator(entry);
    }

    public DialogueRequestBattle_Initiator(PlayLogEntry_Interaction entry) : base(entry)
    {
      _battle = Find.BattleLog.Battles
        .Where(battle => battle.Concerns(Initiator))
        .OrderByDescending(battle => battle.CreationTimestamp)
        .RandomElementByWeight(battle => battle.Importance * (1 / AgeDays(battle)));
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
