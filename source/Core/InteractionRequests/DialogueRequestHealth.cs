#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestHealth : DialogueRequestTwoPawn<DialogueDataHealth>
  {
    public DialogueRequestHealth(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    public abstract Hediff? Hediff
    {
      get;
    }

    public override async Task BuildData(DialogueDataHealth data)
    {
      await base.BuildData(data);
      if (Hediff == null)
        return;
      data.HediffLabel = this.Hediff.LabelCap ?? string.Empty;
      data.HediffSeverity = this.Hediff.Severity.ToString() ?? string.Empty;
      data.HediffDescription = this.Hediff.def.description ?? string.Empty;
      data.HediffSource = this.Hediff.sourceLabel ?? string.Empty;
      data.HediffPart = this.Hediff.Part?.Label ?? string.Empty;
    }

    public override string? Action => "HealthChitchat";

    public override Rule[] Rules
    {
      get
      {
        if (Hediff == null)
          return [
            new Rule_String("hediff", "RimDialogue.Injury".Translate()),
            new Rule_String("severity", string.Empty),
            new Rule_String("source", string.Empty),
            new Rule_String("part", string.Empty)];

        return [
          new Rule_String("hediff", Hediff.Part != null ? $"{Hediff.LabelBase} {Hediff.Part.LabelShort}" : $"{Hediff.LabelBase}"),
          new Rule_String("severity", this.Hediff.Severity.ToString()),
          new Rule_String("source", this.Hediff.sourceLabel ?? string.Empty),
          new Rule_String("part", this.Hediff.Part?.Label ?? string.Empty)];
      }
    }
  }
}

