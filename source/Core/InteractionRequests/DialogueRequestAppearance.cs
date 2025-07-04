#nullable enable
using RimDialogue.Core.InteractionRequests;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestAppearance : DialogueRequestTwoPawn<DialogueDataAppearance>
  {

    public DialogueRequestAppearance(PlayLogEntry_Interaction entry) : base(entry)
    {

    }

    public abstract Pawn Pawn
    {
      get;
    }

    public override void BuildData(DialogueDataAppearance data)
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

      base.BuildData(data);
    }

    public override string? Action => "AppearanceChitchat";

    public override Rule[] Rules
    {
      get
      {
        var style = this.Pawn.style;
        var story = this.Pawn.story;
        return [
          new Rule_String("beard", style.beardDef?.label ?? string.Empty),
          new Rule_String("face_tattoo", style.FaceTattoo?.label ?? string.Empty),
          new Rule_String("body_tattoo", style.BodyTattoo?.label ?? string.Empty),
          new Rule_String("hair", story.hairDef.label ?? string.Empty),
          new Rule_String("hair_color", H.DescribeHairColor(story.HairColor.r, story.HairColor.g, story.HairColor.b, story.HairColor.a))
        ];
      }
    }
  }
}
