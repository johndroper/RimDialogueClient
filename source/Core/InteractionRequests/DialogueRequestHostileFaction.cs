using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHostileFaction : DialogueRequestFaction
  {
    public static new DialogueRequestHostileFaction BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestHostileFaction(entry, interactionTemplate);
    }

    public DialogueRequestHostileFaction(PlayLogEntry_Interaction entry, string interactionTemplate) :
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
