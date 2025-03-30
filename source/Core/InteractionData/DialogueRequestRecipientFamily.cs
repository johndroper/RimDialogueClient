using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using static UnityEngine.GraphicsBuffer;

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

    public override void Execute()
    {
      var dialogueData = new DialogueDataFamily();
      Build(dialogueData);
      var tracker = H.GetTracker();
      Send(
        [
          new("chitChatJson", dialogueData),
          new("targetJson", H.MakePawnData(Relation.otherPawn, tracker.GetInstructions(Relation.otherPawn)))
        ],
        "FamilyChitchat");
    }

    public override string GetInteraction()
    {
      //System.NullReferenceException: Object reference not set to an instance of an object
      return this.InteractionTemplate
        .Replace(RelationPlaceholder, Relation.def.GetGenderSpecificLabel(Relation.otherPawn))
        .Replace(FamilyPlaceholder, Relation.otherPawn.Name.ToStringShort);
    }
  }
}


