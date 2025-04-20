using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestInitiatorFamily : DialogueRequestTarget<DialogueDataFamily>
  {
    const string RelationPlaceholder = "**relation**";
    const string FamilyPlaceholder = "**family**";

    public DirectPawnRelation Relation { get; set; }

    public static DialogueRequestInitiatorFamily BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestInitiatorFamily(entry, interactionTemplate);
    }

    public DialogueRequestInitiatorFamily(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Relation = this.Initiator.relations.DirectRelations.RandomElement();
    }

    public override void Build(DialogueDataFamily data)
    {
      base.Build(data);
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

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(RelationPlaceholder, Relation.def.GetGenderSpecificLabel(Relation.otherPawn))
        .Replace(FamilyPlaceholder, Relation.otherPawn.Name.ToStringShort);
    }
  }
}


