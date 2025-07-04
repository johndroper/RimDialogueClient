using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRecipientFamily : DialogueRequestTwoPawn<DialogueDataFamily>
  {
    const string RelationPlaceholder = "relation";
    const string FamilyPlaceholder = "family";

    public DirectPawnRelation Relation { get; set; }

    public static new DialogueRequestRecipientFamily BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestRecipientFamily(entry);
    }

    public DialogueRequestRecipientFamily(PlayLogEntry_Interaction entry) : base(entry)
    {
      Relation = this.Recipient.relations.DirectRelations.RandomElement();
    }

    public override void BuildData(DialogueDataFamily data)
    {
      base.BuildData(data);
      data.RelationType = Relation.def.GetGenderSpecificLabel(Relation.otherPawn) ?? string.Empty;
    }

    public override string Action => "FamilyChitchat";

    public override Rule[] Rules => [
      new Rule_String(RelationPlaceholder, Relation.def.GetGenderSpecificLabel(Relation.otherPawn)),
      new Rule_String(FamilyPlaceholder, Relation.otherPawn.Name.ToStringShort)
    ];
  }
}


