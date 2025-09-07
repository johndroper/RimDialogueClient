using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHostileFaction : DialogueRequestFaction
  {
    //public static new DialogueRequestHostileFaction BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestHostileFaction(entry);
    //}

    public DialogueRequestHostileFaction(
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
        _faction ??= Find.FactionManager.RandomEnemyFaction();
        return _faction;
      }
    }
  }
}
