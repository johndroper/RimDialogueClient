using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestBestSkill<DataT> : DialogueRequest<DataT> where DataT : DialogueDataSkill, new()
  {
    const string Placeholder = "**skill**";
    public static DialogueRequestBestSkill<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestBestSkill<DataT>(entry, interactionTemplate);
    }

    public SkillDef SkillDef { get; set; }
    public SkillRecord InitiatorSkillRecord { get; set; }
    public SkillRecord RecipientSkillRecord { get; set; }

    public DialogueRequestBestSkill(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      var skills = this.Initiator.skills?.skills;
      var maxLevel = skills.Max(s => s.Level);
      this.InitiatorSkillRecord = skills.Where(s => s.Level == maxLevel).RandomElement();
      SkillDef = this.InitiatorSkillRecord?.def;
      RecipientSkillRecord = this.Recipient?.skills?.GetSkill(SkillDef);
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
      dialogueData.SkillName = this.InitiatorSkillRecord?.def?.label ?? string.Empty;
      dialogueData.SkillDescription = this.InitiatorSkillRecord?.def.description ?? string.Empty;
      dialogueData.InitiatorSkillLevel = this.InitiatorSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
      dialogueData.RecipientSkillLevel = this.RecipientSkillRecord?.LevelDescriptor.ToLower() ?? string.Empty;
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
      return this.InteractionTemplate.Replace(Placeholder, InitiatorSkillRecord.def.label);
    }
  }
}
