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
  public class DialogueRequestSkill<DataT> : DialogueRequest<DataT> where DataT : DialogueDataSkill, new()
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
    public static DialogueRequestSkill<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestSkill<DataT>(entry, interactionTemplate);
    }

    public SkillDef SkillDef { get; set; }

    public SkillRecord InitiatorSkillRecord { get; set; }
    public SkillRecord RecipientSkillRecord { get; set; }

    public DialogueRequestSkill(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      SkillDef = SkillDefs.RandomElement();
      InitiatorSkillRecord = Initiator.skills?.GetSkill(SkillDef);
      RecipientSkillRecord = Recipient.skills?.GetSkill(SkillDef);

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
      return this.InteractionTemplate.Replace(Placeholder, InitiatorSkillRecord.def.label);
    }
  }
}
