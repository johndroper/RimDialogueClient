#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestSkill : DialogueRequest<DialogueDataSkill>
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

    const string Placeholder = "**skill**";
    public static DialogueRequestSkill BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestSkill(entry, interactionTemplate);
    }

    public SkillRecord InitiatorSkillRecord { get; set; }
    public SkillRecord RecipientSkillRecord { get; set; }

    public DialogueRequestSkill(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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

    public override void Build(DialogueDataSkill data)
    {
      data.SkillName = SkillDef.label ?? string.Empty;
      data.SkillDescription = SkillDef.description ?? string.Empty;
      data.InitiatorSkillLevel = InitiatorSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      data.RecipientSkillLevel = RecipientSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      data.InitiatorPassion = InitiatorSkillRecord?.passion.ToString().ToLower() ?? string.Empty;
      data.RecipientPassion = RecipientSkillRecord?.passion.ToString().ToLower() ?? string.Empty;
      base.Build(data);
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, SkillDef.label);
    }
  }
}
