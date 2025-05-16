#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAnimal_Colony : DialogueRequestAnimal
  {
    public static new DialogueRequestAnimal_Colony BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestAnimal_Colony(entry, interactionTemplate);
    }

    public DialogueRequestAnimal_Colony(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Pawn? _animal;
    public override Pawn Animal
    {
      get
      {
        _animal ??= Find.CurrentMap.mapPawns.SpawnedColonyAnimals.RandomElement();
        return _animal;
      }
    }
  }
}
