#nullable enable
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestNeutralFaction : DialogueRequestFaction
  {
    //public static new DialogueRequestNeutralFaction BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestNeutralFaction(entry);
    //}

    public DialogueRequestNeutralFaction(
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
          .Where(faction => faction.PlayerRelationKind == FactionRelationKind.Neutral)
          .RandomElement();
        return _faction;
      }
    }
  }
}
