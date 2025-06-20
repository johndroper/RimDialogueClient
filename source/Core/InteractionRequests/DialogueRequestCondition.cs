using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestCondition : DialogueRequestTwoPawn<DialogueDataCondition>
  {
    const string GameConditionPlaceholder = "**game_condition**";

    static Dictionary<int, DialogueRequestCondition> recentConditions = new();

    public static new DialogueRequestCondition BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestCondition(entry, interactionTemplate);
    }

    public GameCondition GameCondition { get; set; }
    public string Interaction { get; set; }
    public string Explanation { get; set; }
    public string Duration { get; set; }

    public DialogueRequestCondition(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for condition {entry.LogID} with template {interactionTemplate}.");
      if (!Find.CurrentMap.GameConditionManager.ActiveConditions.Any())
      {
        Interaction = "a very normal day.";
        return;
      }
      GameCondition = Find.CurrentMap.GameConditionManager.ActiveConditions.RandomElement();
      Explanation = H.RemoveWhiteSpaceAndColor(GameCondition.def.description ?? GameCondition.def.letterText);
      Duration = GameCondition.TicksPassed.ToStringTicksToPeriod();
      switch (GameCondition)
      {
        case GameCondition_Aurora:
          Interaction = "an aurora is lighting up the sky";
          break;
        case GameCondition_ClimateCycle:
          Interaction = $"a climate cycle has been affecting the weather";
          break;
        case GameCondition_ColdSnap:
          Interaction = $"a cold snap has brought freezing temperatures";
          break;
        case GameCondition_DisableElectricity:
          Interaction = $"a solar flare has been blasting the planet";
          break;
        case GameCondition_Flashstorm:
          Interaction = $"a flashstorm has been raging";
          break;
        case GameCondition_HeatWave:
          Interaction = $"a heat wave has been bringing scorching temperatures";
          break;
        case GameCondition_NoSunlight:
          Interaction = $"a solar eclipse has been blocking the sun";
          break;
        case GameCondition_Planetkiller planetKiller:
          Interaction = $"a planet killer is coming in {planetKiller.TicksLeft.ToStringTicksToPeriod()}";
          Explanation += " If we don't escape the planet in time, we will all die";
          break;
        case GameCondition_PsychicEmanation psychicEmanation:
          Interaction = $"a {GameCondition.def.label} has been affecting all {psychicEmanation.gender} in the area";
          break;
        case GameCondition_PsychicSuppression psychicSuppression:
          Interaction = $"a {GameCondition.def.label} has been affecting all {psychicSuppression.gender} in the area";
          break;
        case GameCondition_SmokeSpewer:
          Interaction = $"a smoke spewer has been spewing toxic smoke";
          break;
        case GameCondition_TemperatureOffset:
          Interaction = $"a temperature offset has been affecting the area";
          break;
        case GameCondition_ToxicFallout:
          Interaction = $"toxic fallout has been contaminating the area";
          break;
        case GameCondition_UnnaturalDarkness:
          Interaction = "an unnatural darkness has been covering the area";
          break;
        case GameCondition_UnnaturalHeat:
          Interaction = "an unnatural heat has been affecting the area";
          break;
        case GameCondition_VolcanicWinter:
          Interaction = $"a volcanic winter has been affecting the area";
          break;
        default:
          Interaction = $"a {GameCondition.Label} is affecting the area";
          break;
      }
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(GameConditionPlaceholder, Interaction);
    }

    public override void BuildData(DialogueDataCondition data)
    {
      data.Explanation = Explanation;
      data.LabelCap = GameCondition.LabelCap;
      data.TooltipString = H.RemoveWhiteSpaceAndColor(GameCondition.TooltipString);
      data.Duration = Duration;
      data.DurationTicks = GameCondition.TicksPassed;
      data.Permanent = GameCondition.Permanent;
      base.BuildData(data);
    }

    public override string Action => null;

  }
}
