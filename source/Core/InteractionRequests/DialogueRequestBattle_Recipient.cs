using RimDialogue.Core.InteractionData;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Recipient : DialogueRequestBattle
  {
    public static new DialogueRequestBattle_Recipient BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestBattle_Recipient(entry);
    }

    public DialogueRequestBattle_Recipient(PlayLogEntry_Interaction entry) : base(entry)
    {
      _battle = Find.BattleLog.Battles
        .Where(battle => battle.Concerns(Recipient))
        .OrderByDescending(battle => battle.CreationTimestamp)
        .RandomElementByWeight(battle => battle.Importance * (1 / AgeDays(battle)));
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
