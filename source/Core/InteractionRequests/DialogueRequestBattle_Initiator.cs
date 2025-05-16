using RimDialogue.Core.InteractionData;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Initiator : DialogueRequestBattle
  {
    public static new DialogueRequestBattle_Initiator BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestBattle_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestBattle_Initiator(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      _battle = Find.BattleLog.Battles
        .Where(battle => battle.Concerns(Initiator))
        .OrderByDescending(battle => battle.CreationTimestamp)
        .Take(5)
        .RandomElement();
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
