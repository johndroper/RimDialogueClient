using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAlliedFaction : DialogueRequestFaction
  {
    const string FactionPlaceholder = "**faction**";

    public static DialogueRequestAlliedFaction BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestAlliedFaction(entry, interactionTemplate);
    }

    public DialogueRequestAlliedFaction(LogEntry entry, string interactionTemplate) :
      base(entry, interactionTemplate)
    {

    }

    private Faction? _faction;
    public override Faction Faction
    {
      get
      {
        _faction ??= Find.FactionManager.GetFactions()
          .Where(faction => faction.PlayerRelationKind == FactionRelationKind.Hostile)
          .RandomElement();
        return _faction;
      }
    }
  }
}
