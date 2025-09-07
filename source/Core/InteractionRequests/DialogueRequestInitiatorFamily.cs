#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestInitiatorFamily : DialogueRequestTarget<DialogueDataFamily>
  {
    const string RelationPlaceholder = "relation";
    const string FamilyPlaceholder = "family";

    public DirectPawnRelation Relation { get; set; }

    //public static new DialogueRequestInitiatorFamily BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestInitiatorFamily(entry);
    //}

    public DialogueRequestInitiatorFamily(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      Relation = this.Initiator.relations.DirectRelations.RandomElement();
    }

    public override async Task BuildData(DialogueDataFamily data)
    {
      await base.BuildData(data);
      data.RelationType = Relation.def.GetGenderSpecificLabel(Relation.otherPawn) ?? string.Empty;
    }

    public override string Action => "FamilyChitchat";

    public override Pawn? Target
    {
      get
      {
        return Relation?.otherPawn;
      }
    }

    public override Rule[] Rules
    {
      get
      {
        if (Relation == null)
          return [
            new Rule_String(RelationPlaceholder,
            "RimDialogue.Family".Translate()),
          new Rule_String(FamilyPlaceholder,
            "RimDialogue.Member".Translate())];
        return [
          new Rule_String(RelationPlaceholder,
            Relation.def.GetGenderSpecificLabel(Relation.otherPawn)),
          new Rule_String(FamilyPlaceholder,
            Relation.otherPawn?.Name?.ToStringShort ?? "RimDialogue.Unknown".Translate())];
      }
    }
  }
}


