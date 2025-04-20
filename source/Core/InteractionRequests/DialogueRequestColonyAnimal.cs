using RimDialogue.Core.InteractionRequests;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonyAnimal<DataT> : DialogueRequestTarget<DataT> where DataT : DialogueTargetData, new()
  {
    const string animalPlaceholder = "**animal**";

    public static DialogueRequestColonyAnimal<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestColonyAnimal<DataT>(entry, interactionTemplate);
    }

    public override Pawn Target => _target;

    private Pawn _target;

    public DialogueRequestColonyAnimal(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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
