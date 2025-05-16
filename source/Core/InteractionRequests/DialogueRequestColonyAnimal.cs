using RimDialogue.Core.InteractionRequests;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonyAnimal : DialogueRequestTarget<DialogueTargetData>
  {
    const string animalPlaceholder = "**animal**";

    public static new DialogueRequestColonyAnimal BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestColonyAnimal(entry, interactionTemplate);
    }

    public override Pawn Target => _target;

    private Pawn _target;

    public DialogueRequestColonyAnimal(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      ;
      _target = Find.CurrentMap.mapPawns.SpawnedColonyAnimals.RandomElement();
    }

    public override string Action => "ColonistChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(animalPlaceholder, this._target.Name.ToStringShort);
    }
  }
}
