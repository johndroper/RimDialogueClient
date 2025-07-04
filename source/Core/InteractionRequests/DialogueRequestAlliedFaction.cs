using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAlliedFaction : DialogueRequestFaction
  {
    const string FactionPlaceholder = "**faction**";

    public static new DialogueRequestAlliedFaction BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestAlliedFaction(entry);
    }

    public DialogueRequestAlliedFaction(PlayLogEntry_Interaction entry) :
      base(entry)
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
