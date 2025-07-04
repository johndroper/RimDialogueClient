#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestSkill : DialogueRequestTwoPawn<DialogueDataSkill>
  {
    public static readonly SkillDef[] SkillDefs = new SkillDef[]
    {
      SkillDefOf.Construction,
      SkillDefOf.Plants,
      SkillDefOf.Intellectual,
      SkillDefOf.Mining,
      SkillDefOf.Shooting,
      SkillDefOf.Melee,
      SkillDefOf.Social,
      SkillDefOf.Animals,
      SkillDefOf.Cooking,
      SkillDefOf.Medicine,
      SkillDefOf.Artistic,
      SkillDefOf.Crafting
    };

    const string Placeholder = "skill";
    public static new DialogueRequestSkill BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestSkill(entry);
    }

    public SkillRecord InitiatorSkillRecord { get; set; }
    public SkillRecord RecipientSkillRecord { get; set; }

    public DialogueRequestSkill(PlayLogEntry_Interaction entry) : base(entry)
    {
      InitiatorSkillRecord = Initiator.skills.GetSkill(SkillDef);
      RecipientSkillRecord = Recipient.skills.GetSkill(SkillDef);
    }

    private SkillDef? _skillDef;
    public virtual SkillDef SkillDef
    {
      get
      {
        if (_skillDef == null)
          _skillDef = SkillDefs.RandomElement();
        return _skillDef;
      }
    }

    public override string? Action => "SkillChitchat";

    public override void BuildData(DialogueDataSkill data)
    {
      data.SkillName = SkillDef.label ?? string.Empty;
      data.SkillDescription = SkillDef.description ?? string.Empty;
      data.InitiatorSkillLevel = InitiatorSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      data.RecipientSkillLevel = RecipientSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      data.InitiatorPassion = InitiatorSkillRecord?.passion.ToString().ToLower() ?? string.Empty;
      data.RecipientPassion = RecipientSkillRecord?.passion.ToString().ToLower() ?? string.Empty;
      base.BuildData(data);
    }

    public override Rule[] Rules => [new Rule_String(Placeholder, SkillDef.label)];

  }
}
