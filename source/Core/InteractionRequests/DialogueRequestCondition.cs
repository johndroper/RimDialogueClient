#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestCondition : DialogueRequestTwoPawn<DialogueDataCondition>
  {
    static Dictionary<int, DialogueRequestCondition> recentConditions = new();

    //public static new DialogueRequestCondition BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestCondition(entry);
    //}

    public GameCondition? GameCondition { get; set; }
    public string Condition { get; set; }
    public string? Explanation { get; set; }
    public string? Duration { get; set; }

    public DialogueRequestCondition(PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(
        entry,
        interactionDef,
        initiator,
        recipient)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for condition {entry.LogID} with template {interactionTemplate}.");
      if (!Find.CurrentMap.GameConditionManager.ActiveConditions.Any())
      {
        Condition = "a very normal day.";
        return;
      }
      GameCondition = Find.CurrentMap.GameConditionManager.ActiveConditions.RandomElement();
      Explanation = H.RemoveWhiteSpaceAndColor(GameCondition.def.description ?? GameCondition.def.letterText);
      Duration = GameCondition.TicksPassed.ToStringTicksToPeriod();
      switch (GameCondition)
      {
        case GameCondition_Aurora:
          Condition = "an aurora is lighting up the sky";
          break;
        case GameCondition_ClimateCycle:
          Condition = $"a climate cycle has been affecting the weather";
          break;
        case GameCondition_ColdSnap:
          Condition = $"a cold snap has brought freezing temperatures";
          break;
        case GameCondition_DisableElectricity:
          Condition = $"a solar flare has been blasting the planet";
          break;
        case GameCondition_Flashstorm:
          Condition = $"a flashstorm has been raging";
          break;
        case GameCondition_HeatWave:
          Condition = $"a heat wave has been bringing scorching temperatures";
          break;
        case GameCondition_NoSunlight:
          Condition = $"a solar eclipse has been blocking the sun";
          break;
        case GameCondition_Planetkiller planetKiller:
          Condition = $"a planet killer is coming in {planetKiller.TicksLeft.ToStringTicksToPeriod()}";
          Explanation += " If we don't escape the planet in time, we will all die";
          break;
        case GameCondition_PsychicEmanation psychicEmanation:
          Condition = $"a {GameCondition.def.label} has been affecting all {psychicEmanation.gender} in the area";
          break;
        case GameCondition_PsychicSuppression psychicSuppression:
          Condition = $"a {GameCondition.def.label} has been affecting all {psychicSuppression.gender} in the area";
          break;
        case GameCondition_SmokeSpewer:
          Condition = $"a smoke spewer has been spewing toxic smoke";
          break;
        case GameCondition_TemperatureOffset:
          Condition = $"a temperature offset has been affecting the area";
          break;
        case GameCondition_ToxicFallout:
          Condition = $"toxic fallout has been contaminating the area";
          break;
        case GameCondition_UnnaturalDarkness:
          Condition = "an unnatural darkness has been covering the area";
          break;
        case GameCondition_UnnaturalHeat:
          Condition = "an unnatural heat has been affecting the area";
          break;
        case GameCondition_VolcanicWinter:
          Condition = $"a volcanic winter has been affecting the area";
          break;
        default:
          Condition = $"a {GameCondition.Label} is affecting the area";
          break;
      }
    }

    public override Rule[] Rules => 
    [
      new Rule_String("condition", Condition),
      new Rule_String("duration", Duration)
    ];

    public override async Task BuildData(DialogueDataCondition data)
    {
      data.Explanation = Explanation;
      data.LabelCap = GameCondition?.LabelCap ?? "RimDialogue.Unknown".Translate();
      data.TooltipString = H.RemoveWhiteSpaceAndColor(GameCondition?.TooltipString);
      data.Duration = Duration;
      data.DurationTicks = GameCondition?.TicksPassed ?? 0;
      data.Permanent = GameCondition?.Permanent ?? false;
      await base.BuildData(data);
    }

    public override string? Action => null;

  }
}
