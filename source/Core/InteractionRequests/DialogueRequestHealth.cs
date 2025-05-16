#nullable enable
using RimDialogue.Core.InteractionRequests;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestHealth : DialogueRequestTwoPawn<DialogueDataHealth>
  {
    const string Placeholder = "**hediff**";

    public DialogueRequestHealth(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public abstract Hediff Hediff
    {
      get;
    }

    public override void BuildData(DialogueDataHealth data)
    {
      data.HediffLabel = this.Hediff.LabelCap ?? string.Empty;
      data.HediffSeverity = this.Hediff.Severity.ToString() ?? string.Empty;
      data.HediffDescription = this.Hediff.def.description ?? string.Empty;
      data.HediffSource = this.Hediff.sourceLabel ?? string.Empty;
      data.HediffPart = this.Hediff.Part?.Label ?? string.Empty;
      base.BuildData(data);
    }

    public override string? Action => "HealthChitchat";

    public override string GetInteraction()
    {
      if (Hediff.Part != null)
        return this.InteractionTemplate.Replace(Placeholder, $"{Hediff.LabelBase} {Hediff.Part.LabelShort}");
      else
        return this.InteractionTemplate.Replace(Placeholder, $"{Hediff.LabelBase}");
    }
  }
}

