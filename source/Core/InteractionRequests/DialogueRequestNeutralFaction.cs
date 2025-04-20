#nullable enable
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestNeutralFaction : DialogueRequestFaction
  {
    public static DialogueRequestNeutralFaction BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestNeutralFaction(entry, interactionTemplate);
    }

    public DialogueRequestNeutralFaction(LogEntry entry, string interactionTemplate) :
      base(entry, interactionTemplate)
    {

    }

    private Faction? _faction;
    public override Faction Faction
    {
      get
      {
        _faction ??= Find.FactionManager.GetFactions()
          .Where(faction => faction.PlayerRelationKind == FactionRelationKind.Neutral)
          .RandomElement();
        return _faction;
      }
    }
  }
}
