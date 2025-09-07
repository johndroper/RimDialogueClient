#nullable enable

using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoyalFaction : DialogueRequestFaction
  {
    //public static new DialogueRequestRoyalFaction BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestRoyalFaction(entry);
    //}

    public DialogueRequestRoyalFaction(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
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
