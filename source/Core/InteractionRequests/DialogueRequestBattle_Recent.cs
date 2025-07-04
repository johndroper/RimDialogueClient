using RimDialogue.Core.InteractionData;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Recent : DialogueRequestBattle
  {
    public static new DialogueRequestBattle_Recent BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestBattle_Recent(entry);
    }

    public DialogueRequestBattle_Recent(PlayLogEntry_Interaction entry) : base(entry)
    {
      _battle = Find.BattleLog.Battles
        .OrderByDescending(battle => battle.CreationTimestamp)
        .RandomElementByWeight(battle => battle.Importance * (1 / AgeDays(battle)));
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
