using RimDialogue.Core.InteractionData;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestBattle_Recent : DialogueRequestBattle
  {
    public static DialogueRequestBattle_Recent BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestBattle_Recent(entry, interactionTemplate);
    }

    public DialogueRequestBattle_Recent(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      _battle = Find.BattleLog.Battles
        .OrderByDescending(battle => battle.CreationTimestamp)
        .Take(5)
        .RandomElement();
    }

    private Battle _battle;
    public override Battle Battle => _battle;

  }
}
