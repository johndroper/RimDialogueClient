#nullable enable
using RimDialogue.Core.InteractionRequests;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestHealth : DialogueRequestTwoPawn<DialogueDataHealth>
  {
    public DialogueRequestHealth(PlayLogEntry_Interaction entry) : base(entry)
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

    public override Rule[] Rules => [
      new Rule_String("hediff", Hediff.Part != null ? $"{Hediff.LabelBase} {Hediff.Part.LabelShort}" : $"{Hediff.LabelBase}"),
      new Rule_String("severity", this.Hediff.Severity.ToString()),
      new Rule_String("source", this.Hediff.sourceLabel ?? string.Empty),
      new Rule_String("part", this.Hediff.Part?.Label ?? string.Empty)
    ];
  }
}

