#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAnimal_Colony : DialogueRequestAnimal
  {
    //public static new DialogueRequestAnimal_Colony BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestAnimal_Colony(entry);
    //}

    public DialogueRequestAnimal_Colony(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
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
