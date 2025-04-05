#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestAppearance : DialogueRequest<DialogueDataAppearance>
  {

    public DialogueRequestAppearance(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public abstract Pawn Pawn
    {
      get;
    }

    public override void Execute()
    {
      var style = this.Pawn.style;
      var story = this.Pawn.story;
      var dialogueData = new DialogueDataAppearance();
      dialogueData.Pawn = this.Pawn.Name.ToStringShort;
      dialogueData.Beard = style.beardDef.label;
      dialogueData.BeardCategory = style.beardDef.StyleItemCategory.label;
      dialogueData.Hair = story.hairDef.label;
      dialogueData.HairColor = H.DescribeHairColor(story.HairColor.r, story.HairColor.g, story.HairColor.b, story.HairColor.a);
      dialogueData.HairCategory = story.hairDef.StyleItemCategory.label;
      dialogueData.BodyTattoo = style.BodyTattoo?.label ?? "None";
      dialogueData.BodyTattooCategory = style.BodyTattoo?.StyleItemCategory.label ?? "None";
      dialogueData.FaceTattoo = style.FaceTattoo?.label ?? "None";
      dialogueData.FaceTattooCategory = style.FaceTattoo?.StyleItemCategory.label ?? "None";
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "AppearanceChitchat");
    }

    public override string GetInteraction()
    {
      var style = this.Pawn.style;
      var story = this.Pawn.story;
      return this.InteractionTemplate
        .Replace("**beard**", style.beardDef?.label ?? string.Empty)
        .Replace("**face_tattoo**", style.FaceTattoo?.label ?? string.Empty)
        .Replace("**body_tattoo**", style.BodyTattoo?.label ?? string.Empty)
        .Replace("**hair**", story.hairDef.label ?? string.Empty)
        .Replace("**hair_color**", H.DescribeHairColor(story.HairColor.r, story.HairColor.g, story.HairColor.b, story.HairColor.a));
    }
  }
}
