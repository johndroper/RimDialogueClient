#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAnimal_Colony : DialogueRequestAnimal
  {
    public static DialogueRequestAnimal_Colony BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestAnimal_Colony(entry, interactionTemplate);
    }

    public DialogueRequestAnimal_Colony(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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
