#nullable enable
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestSkill: DialogueRequest<DialogueDataSkill>
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

    public override void Execute()
    {
      var dialogueData = new DialogueDataSkill();
      dialogueData.SkillName = SkillDef.label ?? string.Empty;
      dialogueData.SkillDescription = SkillDef.description ?? string.Empty;
      dialogueData.InitiatorSkillLevel = InitiatorSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      dialogueData.RecipientSkillLevel = RecipientSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      dialogueData.InitiatorPassion = InitiatorSkillRecord?.passion.ToString().ToLower() ?? string.Empty;
      dialogueData.RecipientPassion = RecipientSkillRecord?.passion.ToString().ToLower() ?? string.Empty;
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "SkillChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, SkillDef.label);
    }
  }
}
