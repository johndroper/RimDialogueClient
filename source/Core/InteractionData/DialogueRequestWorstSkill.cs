using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWorstSkill<DataT> : DialogueRequest<DataT> where DataT : DialogueDataSkill, new()
  {
    const string Placeholder = "**skill**";
    public static DialogueRequestWorstSkill<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestWorstSkill<DataT>(entry, interactionTemplate);
    }

    public SkillDef SkillDef { get; set; }
    public SkillRecord InitiatorSkillRecord { get; set; }
    public SkillRecord RecipientSkillRecord { get; set; }

    public DialogueRequestWorstSkill(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      var skills = Initiator.skills?.skills;
      var maxLevel = skills.Min(s => s.Level);
      InitiatorSkillRecord = skills.Where(s => s.Level == maxLevel).RandomElement();
      SkillDef = InitiatorSkillRecord?.def;
      RecipientSkillRecord = this.Recipient?.skills?.GetSkill(SkillDef);
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
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
