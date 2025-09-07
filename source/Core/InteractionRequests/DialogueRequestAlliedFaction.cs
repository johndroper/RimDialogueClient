#nullable enable
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAlliedFaction : DialogueRequestFaction
  {
    //public static new DialogueRequestAlliedFaction BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestAlliedFaction(entry);
    //}

    public DialogueRequestAlliedFaction(
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
          .Where(faction => faction.PlayerRelationKind == FactionRelationKind.Hostile)
          .RandomElement();
        return _faction;
      }
    }
  }
}
