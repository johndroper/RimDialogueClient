#nullable enable
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAnimal_Wild : DialogueRequestAnimal
  {

    public static new DialogueRequestAnimal_Wild BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestAnimal_Wild(entry, interactionTemplate);
    }

    public DialogueRequestAnimal_Wild(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Pawn? _animal;
    public override Pawn Animal
    {
      get
      {
        _animal ??= Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(p => p.RaceProps.Animal && p.Faction == null).RandomElement();
        return _animal;
      }
    }
  }
}

