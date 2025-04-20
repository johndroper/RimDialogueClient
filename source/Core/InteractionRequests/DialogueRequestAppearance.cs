#nullable enable
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

    public override void Build(DialogueDataAppearance data)
    {
      var style = this.Pawn.style;
      var story = this.Pawn.story;
      data.Pawn = this.Pawn.Name.ToStringShort;
      data.Beard = style.beardDef.label;
      data.BeardCategory = style.beardDef.StyleItemCategory.label;
      data.Hair = story.hairDef.label;
      data.HairColor = H.DescribeHairColor(story.HairColor.r, story.HairColor.g, story.HairColor.b, story.HairColor.a);
      data.HairCategory = story.hairDef.StyleItemCategory.label;
      data.BodyTattoo = style.BodyTattoo?.label ?? "None";
      data.BodyTattooCategory = style.BodyTattoo?.StyleItemCategory.label ?? "None";
      data.FaceTattoo = style.FaceTattoo?.label ?? "None";
      data.FaceTattooCategory = style.FaceTattoo?.StyleItemCategory.label ?? "None";

      base.Build(data);
    }

    public override string? Action => "AppearanceChitchat";

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
