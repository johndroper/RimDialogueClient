using RimDialogue.Access;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonyAnimal<DataT> : DialogueRequest<DataT> where DataT : DialogueTargetData, new()
  {
    const string animalPlaceholder = "**animal**";

    public static DialogueRequestColonyAnimal<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestColonyAnimal<DataT>(entry, interactionTemplate);
    }

    Pawn Target { get; set; }

    public DialogueRequestColonyAnimal(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {;
      Target = Find.CurrentMap.mapPawns.SpawnedColonyAnimals.RandomElement();
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
      var tracker = H.GetTracker();
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData),
          new("targetJson", H.MakePawnData(Target, tracker.GetInstructions(Target)))
        ],
        "ColonistChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(animalPlaceholder, this.Target.Name.ToStringShort);
    }
  }
}
