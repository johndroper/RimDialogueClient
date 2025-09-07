using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonyAnimal : DialogueRequestTarget<DialogueTargetData>
  {
    const string animalPlaceholder = "animal";

    //public static new DialogueRequestColonyAnimal BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestColonyAnimal(entry);
    //}

    public override Pawn Target => _target;

    private Pawn _target;

    public DialogueRequestColonyAnimal(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      _target = Find.CurrentMap.mapPawns.SpawnedColonyAnimals.RandomElement();
    }

    public override string Action => "ColonistChitchat";

    public override Rule[] Rules => [new Rule_String(animalPlaceholder, this._target.Name.ToStringShort)];

  }
}
