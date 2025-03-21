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
  public class DialogueRequestWorstSkill<DataT> : DialogueRequest<DataT> where DataT : DialogueDataSkill, new()
  {
    const string Placeholder = "**skill**";
    public static DialogueRequestWorstSkill<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestWorstSkill<DataT>(entry, interactionTemplate);
    }

    public SkillRecord SkillRecord { get; set; }

    public DialogueRequestWorstSkill(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      var skills = this.Initiator.skills?.skills;
      var maxLevel = skills.Min(s => s.Level);
      this.SkillRecord = skills.Where(s => s.Level == maxLevel).RandomElement();
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
      dialogueData.SkillName = this.SkillRecord?.def?.label ?? "unknown";
      dialogueData.SkillLevel = this.SkillRecord?.LevelDescriptor.ToLower() ?? "unknown";
      dialogueData.SkillDescription = this.SkillRecord?.def.description ?? "unknown";
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "SkillChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, SkillRecord.def.label);
    }
  }
}
