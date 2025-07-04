using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestInitiatorFamily : DialogueRequestTarget<DialogueDataFamily>
  {
    const string RelationPlaceholder = "relation";
    const string FamilyPlaceholder = "family";

    public DirectPawnRelation Relation { get; set; }

    public static new DialogueRequestInitiatorFamily BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestInitiatorFamily(entry);
    }

    public DialogueRequestInitiatorFamily(PlayLogEntry_Interaction entry) : base(entry)
    {
      Relation = this.Initiator.relations.DirectRelations.RandomElement();
    }

    public override void BuildData(DialogueDataFamily data)
    {
      base.BuildData(data);
      data.RelationType = Relation.def.GetGenderSpecificLabel(Relation.otherPawn) ?? string.Empty;
    }

    public override string Action => "FamilyChitchat";

    public override Pawn Target
    {
      get
      {
        return Relation.otherPawn;
      }
    }

    public override Rule[] Rules => [
      new Rule_String(RelationPlaceholder, Relation.def.GetGenderSpecificLabel(Relation.otherPawn) ?? string.Empty),
      new Rule_String(FamilyPlaceholder, Relation.otherPawn.Name.ToStringShort)
    ];
  }
}


