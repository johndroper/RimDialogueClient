using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRecipientFamily : DialogueRequest<DialogueDataFamily>
  {
    const string RelationPlaceholder = "**relation**";
    const string FamilyPlaceholder = "**family**";

    public DirectPawnRelation Relation { get; set; }

    public static DialogueRequestRecipientFamily BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestRecipientFamily(entry, interactionTemplate);
    }

    public DialogueRequestRecipientFamily(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Relation = this.Recipient.relations.DirectRelations.RandomElement();
    }

    public override void Build(DialogueDataFamily data)
    {
      base.Build(data);
      data.RelationType = Relation.def.GetGenderSpecificLabel(Relation.otherPawn) ?? string.Empty;
    }

    public override string Action => "FamilyChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(RelationPlaceholder, Relation.def.GetGenderSpecificLabel(Relation.otherPawn))
        .Replace(FamilyPlaceholder, Relation.otherPawn.Name.ToStringShort);
    }
  }
}


