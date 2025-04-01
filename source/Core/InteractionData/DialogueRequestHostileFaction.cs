using RimDialogue.Access;
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHostileFaction : DialogueRequestFaction
  {
    public static DialogueRequestHostileFaction BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestHostileFaction(entry, interactionTemplate);
    }

    public DialogueRequestHostileFaction(LogEntry entry, string interactionTemplate) :
      base(entry, interactionTemplate)
    {

    }

    private Faction? _faction;
    public override Faction Faction
    {
      get
      {
        _faction ??= Find.FactionManager.RandomEnemyFaction();
        return _faction;
      }
    }
  }
}
