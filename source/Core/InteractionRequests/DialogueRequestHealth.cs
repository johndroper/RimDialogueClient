#nullable enable
using System;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestHealth : DialogueRequest<DialogueDataHealth> 
  {
    const string Placeholder = "**hediff**";

    public DialogueRequestHealth(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public abstract Hediff Hediff
    {
      get;
    }

    public override void Execute()
    {
      var dialogueData = new DialogueDataHealth();
      dialogueData.HediffLabel = this.Hediff.LabelCap ?? string.Empty;
      dialogueData.HediffSeverity = this.Hediff.Severity.ToString() ?? string.Empty;
      dialogueData.HediffDescription = this.Hediff.def.description ?? string.Empty;
      dialogueData.HediffSource = this.Hediff.sourceLabel ?? string.Empty;
      dialogueData.HediffPart = this.Hediff.Part?.Label ?? string.Empty;
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "HealthChitchat");
    }

    public override string GetInteraction()
    {
      if (Hediff.Part != null)
        return this.InteractionTemplate.Replace(Placeholder, $"{Hediff.LabelBase} {Hediff.Part.LabelShort}");
      else
        return this.InteractionTemplate.Replace(Placeholder, $"{Hediff.LabelBase}");
    }
  }
}

