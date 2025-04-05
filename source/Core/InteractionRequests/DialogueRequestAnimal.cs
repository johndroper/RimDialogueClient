#nullable enable
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestAnimal : DialogueRequest<DialogueDataAnimal>
  {
    const string Placeholder = "**animal**";

    public DialogueRequestAnimal(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Pawn? _animal;
    public virtual Pawn Animal
    {
      get
      {
        _animal ??= Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(p => p.RaceProps.Animal && p.Faction == null).RandomElement();
        return _animal;
      }
    }

    public override void Execute()
    {
      var dialogueData = new DialogueDataAnimal();
      dialogueData.AnimalName = Animal.Name?.ToStringShort ?? Animal.LabelNoParenthesis ?? string.Empty;
      dialogueData.AnimalId = Animal.ThingID;
      dialogueData.AnimalType = Animal.def?.label ?? string.Empty;
      dialogueData.AnimalDescription = H.RemoveWhiteSpace(Animal.def?.description);
      dialogueData.Predator = Animal.RaceProps?.predator ?? false;
      dialogueData.HerdAnimal = Animal.RaceProps?.herdAnimal ?? false;
      dialogueData.PackAnimal = Animal.RaceProps?.packAnimal ?? false;
      dialogueData.Wildness = Animal.RaceProps?.wildness ?? -1f;
      dialogueData.LifeExpectancy = Animal.RaceProps?.lifeExpectancy ?? -1;
      dialogueData.BaseHungerRate = Animal.RaceProps?.baseHungerRate ?? -1f;
      dialogueData.BaseBodySize = Animal.RaceProps?.baseBodySize ?? -1f;
      dialogueData.BaseHealthScale = Animal.RaceProps?.baseHealthScale ?? -1f;
      dialogueData.Trainability = Animal.RaceProps?.trainability?.label ?? string.Empty;

      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "AnimalChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Animal.LabelNoParenthesis);
    }
  }
}

