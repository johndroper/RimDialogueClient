#nullable enable

using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoyalFaction : DialogueRequestFaction
  {
    public static new DialogueRequestRoyalFaction BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestRoyalFaction(entry, interactionTemplate);
    }

    public DialogueRequestRoyalFaction(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Faction? _faction;
    public override Faction Faction
    {
      get
      {
        _faction ??= Find.FactionManager.GetFactions()
          .Where(faction => faction.def.HasRoyalTitles)
          .RandomElement();
        return _faction;
      }
    }
  }
}
