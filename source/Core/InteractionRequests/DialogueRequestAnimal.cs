#nullable enable
using RimDialogue.Core.InteractionRequests;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestAnimal : DialogueRequestTwoPawn<DialogueDataAnimal>
  {
    const string Placeholder = "animal";

    public DialogueRequestAnimal(PlayLogEntry_Interaction entry) : base(entry)
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

    public override void BuildData(DialogueDataAnimal data)
    {
      data.AnimalName = Animal.Name?.ToStringShort ?? Animal.LabelNoParenthesis ?? string.Empty;
      data.AnimalId = Animal.ThingID;
      data.AnimalType = Animal.def?.label ?? string.Empty;
      data.AnimalDescription = H.RemoveWhiteSpace(Animal.def?.description);
      data.Predator = Animal.RaceProps?.predator ?? false;
      data.HerdAnimal = Animal.RaceProps?.herdAnimal ?? false;
      data.PackAnimal = Animal.RaceProps?.packAnimal ?? false;
      //data.Wildness = Animal.RaceProps?.wildness ?? -1f;
      data.LifeExpectancy = Animal.RaceProps?.lifeExpectancy ?? -1;
      data.BaseHungerRate = Animal.RaceProps?.baseHungerRate ?? -1f;
      data.BaseBodySize = Animal.RaceProps?.baseBodySize ?? -1f;
      data.BaseHealthScale = Animal.RaceProps?.baseHealthScale ?? -1f;
      data.Trainability = Animal.RaceProps?.trainability?.label ?? string.Empty;

      base.BuildData(data);
    }

    public override string? Action => "AnimalChitchat";

    public override Rule[] Rules => [new Rule_String(Placeholder, Animal.LabelNoParenthesis)];

  }
}

