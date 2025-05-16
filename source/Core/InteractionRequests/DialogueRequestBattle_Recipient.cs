using RimDialogue.Core.InteractionData;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Recipient : DialogueRequestBattle
  {
    public static new DialogueRequestBattle_Recipient BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestBattle_Recipient(entry, interactionTemplate);
    }

    public DialogueRequestBattle_Recipient(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      _battle = Find.BattleLog.Battles
        .Where(battle => battle.Concerns(Recipient))
        .OrderByDescending(battle => battle.CreationTimestamp)
        .Take(5)
        .RandomElement();
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
