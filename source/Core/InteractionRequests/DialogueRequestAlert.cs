#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAlert : DialogueRequestTarget<DialogueDataAlert>
  {
    public static List<Alert> Alerts = (List<Alert>)Reflection.RimWorld_AlertsReadout_ActiveAlerts.GetValue(Find.Alerts);

    public override Pawn? Target
    {
      get
      {
        return _target;
      }
    }

    private Pawn? _target;
    public string Subject { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;

    public PawnData? targetData;

    public DialogueRequestAlert(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      if (Alerts == null || !Alerts.Any())
        throw new System.Exception("Alerts are null or empty.");
      var alert = Alerts.RandomElementByWeight(alert => (float)alert.Priority + 1f);
      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Creating dialogue request for alert '{alert.GetType().Name}'.");
      switch (alert)
      {
        case Alert_AbandonedBaby abandonedBabyAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AbandonedBaby");
          var abandonedBabies = (List<Pawn>)Reflection.RimWorld_Alert_AbandonedBaby_tmpAbandonedBabiesList.GetValue(abandonedBabyAlert);
          var abandonedBaby = abandonedBabies.RandomElement();
          _target = abandonedBaby;
          Subject = string.Format("RimDialogue.Alert_AbandonedBaby".Translate(), abandonedBaby.Name.ToStringShort);
          if (abandonedBaby.IsOutside())
            Explanation = string.Format("RimDialogue.Alert_AbandonedBaby_Outside".Translate(), abandonedBaby.Name.ToStringShort);
          else
          {
            var room = abandonedBaby.GetRoom();
            Explanation = string.Format("RimDialogue.Alert_AbandonedBaby_InRoom".Translate(), abandonedBaby.Name.ToStringShort, room.GetRoomRoleLabel());
          }
          break;
        case Alert_AnimalFilth animalFilthAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalFilth");
          var animalFilthTargets = (List<GlobalTargetInfo>)Reflection.RimWorld_Alert_AnimalFilth_Targets.GetValue(animalFilthAlert);
          var animalFilthTarget = animalFilthTargets.RandomElement();
          var animalFilthPawn = animalFilthTarget.Pawn;
          _target = animalFilthPawn;
          Subject = string.Format("RimDialogue.Alert_AnimalFilth".Translate(), animalFilthPawn.Name.ToStringShort);
          break;
        case Alert_AnimalPenNeeded animalPenAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalPenNeeded");
          var animalPenTargets = (List<GlobalTargetInfo>)Reflection.RimWorld_Alert_AnimalPenNeeded_Targets.GetValue(animalPenAlert);
          var animalPenTarget = animalPenTargets.RandomElement();
          var animalPenPawn = animalPenTarget.Pawn;
          _target = animalPenPawn;
          Subject = string.Format("RimDialogue.Alert_AnimalPenNeeded".Translate(), animalPenPawn.Label);
          break;
        case Alert_AnimalPenNotEnclosed animalPenNotEnclosedAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalPenNotEnclosed");
          Subject = "RimDialogue.Alert_AnimalPenNotEnclosed".Translate();
          Explanation = "RimDialogue.Alert_AnimalPenNotEnclosed_Explanation".Translate();
          break;
        case Alert_AnimalRoaming animalRoamingAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalRoaming");
          var animalRoamingTargets = (List<GlobalTargetInfo>)Reflection.RimWorld_Alert_AnimalRoaming_Targets.GetValue(animalRoamingAlert);
          var animalRoamingTarget = animalRoamingTargets.RandomElement();
          var animalRoamingPawn = animalRoamingTarget.Pawn;
          _target = animalRoamingPawn;
          Subject = string.Format("RimDialogue.Alert_AnimalRoaming".Translate(), animalRoamingPawn.Label);
          break;
        case Alert_Boredom boredomAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_Boredom");
          var boredomPawns = (List<Pawn>)Reflection.RimWorld_Alert_Boredom_BoredPawnsResult.GetValue(boredomAlert);
          var boredomPawn = boredomPawns.RandomElement();
          _target = boredomPawn;
          Subject = string.Format("RimDialogue.Alert_Boredom".Translate(), boredomPawn.Name.ToStringShort);
          break;
        case Alert_BrawlerHasRangedWeapon brawlerAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_BrawlerHasRangedWeapon");
          var brawlerPawns = (List<Pawn>)Reflection.RimWorld_Alert_BrawlerHasRangedWeapon_BrawlersWithRangedWeaponResult.GetValue(brawlerAlert);
          var brawlerPawn = brawlerPawns.RandomElement();
          _target = brawlerPawn;
          Subject = string.Format("RimDialogue.Alert_BrawlerHasRangedWeapon".Translate(), brawlerPawn.Name.ToStringShort);
          Explanation = string.Format("RimDialogue.Alert_BrawlerHasRangedWeapon_Explanation".Translate(), brawlerPawn.Name.ToStringShort, brawlerPawn.Name.ToStringShort);
          break;
        case RimWorld.Alert_ColonistLeftUnburied colonistLeftUnburiedAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistLeftUnburied");
          var colonistLeftUnburiedThings = (List<Thing>)Reflection.RimWorld_Alert_ColonistLeftUnburied_UnburiedColonistCorpsesResult.GetValue(colonistLeftUnburiedAlert);
          var colonistLeftUnburiedCorpse = (Corpse)colonistLeftUnburiedThings.RandomElement();
          var colonistLeftUnburiedPawn = colonistLeftUnburiedCorpse.InnerPawn;
          _target = colonistLeftUnburiedPawn;
          Subject = string.Format("RimDialogue.Alert_ColonistLeftUnburied".Translate(), colonistLeftUnburiedPawn.Name.ToStringShort);
          Explanation = string.Format("RimDialogue.Alert_ColonistLeftUnburied_Explanation".Translate(), colonistLeftUnburiedPawn.Name.ToStringShort);
          break;
        case Alert_ColonistNeedsRescuing colonistNeedsRescuingAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistNeedsRescuing");
          var colonistNeedsRescuingThings = (List<Pawn>)Reflection.RimWorld_Alert_ColonistNeedsRescuing_ColonistsNeedingRescueResult.GetValue(colonistNeedsRescuingAlert);
          var colonistNeedsRescuingPawn = colonistNeedsRescuingThings.RandomElement();
          _target = colonistNeedsRescuingPawn;
          Subject = string.Format("RimDialogue.Alert_ColonistNeedsRescuing".Translate(), colonistNeedsRescuingPawn.Name.ToStringShort);
          var colonistNeedsRescuingHediffs = colonistNeedsRescuingPawn.health.hediffSet.hediffs
            .Where(hediff => hediff.Severity > 0)
            .OrderByDescending(hediff => hediff.Severity)
            .ToArray();
          Explanation = string.Format("RimDialogue.Alert_ColonistNeedsRescuing_Explanation".Translate(), colonistNeedsRescuingPawn.Name.ToStringShort, string.Join(", ", colonistNeedsRescuingHediffs.Select(hediff => hediff.LabelCap)));
          break;
        case Alert_ColonistNeedsTend colonistNeedsTendAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistNeedsTend");
          var colonistNeedsTendThings = (List<Pawn>)Reflection.RimWorld_Alert_ColonistNeedsTend_NeedingColonistsResult.GetValue(colonistNeedsTendAlert);
          var colonistNeedsTendPawn = colonistNeedsTendThings.RandomElement();
          _target = colonistNeedsTendPawn;
          Subject = string.Format("RimDialogue.Alert_ColonistNeedsTend".Translate(), colonistNeedsTendPawn.Name.ToStringShort);
          var colonistNeedsTendHediffs = colonistNeedsTendPawn.health.hediffSet.GetHediffsTendable()
            .OrderByDescending(hediff => hediff.Severity)
            .ToArray();
          Explanation = string.Format("RimDialogue.Alert_ColonistNeedsTend_Explanation".Translate(), colonistNeedsTendPawn.Name.ToStringShort, string.Join(", ", colonistNeedsTendHediffs.Select(hediff => hediff.LabelCap)));
          break;
        case Alert_ColonistsIdle idleAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistsIdle");
          var idlePawns = (List<Pawn>)Reflection.RimWorld_Alert_ColonistsIdle_IdleColonistsResult.GetValue(idleAlert);
          var idlePawn = idlePawns.RandomElement();
          _target = idlePawn;
          Subject = string.Format("RimDialogue.Alert_ColonistsIdle".Translate(), idlePawn.Name.ToStringShort);
          var workPriorities = DefDatabase<WorkTypeDef>.AllDefsListForReading
            .Select(workType => new
            {
              workType,
              priority = idlePawn.workSettings.GetPriority(workType)
            })
            .OrderByDescending(workPriority => workPriority.priority)
            .Take(5)
            .Select(workPriority => workPriority.workType.labelShort)
            .ToArray();
          Explanation = string.Format("RimDialogue.Alert_ColonistsIdle_Explanation".Translate(), idlePawn.Name.ToStringShort, string.Join(", ", workPriorities));
          break;
        case Alert_DateRitualComing dateRitualComingAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_DateRitualComing");
          var rituals = (List<string>)Reflection.RimWorld_Alert_DateRitualComing_RitualEntries.GetValue(dateRitualComingAlert);
          var ritual = rituals.RandomElement();
          Subject = string.Format("RimDialogue.Alert_DateRitualComing".Translate(), ritual);
          break;
        case Alert_FireInHomeArea fireInHomeAreaAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_FireInHomeArea");
          Subject = "RimDialogue.Alert_FireInHomeArea".Translate();
          break;
        case Alert_Heatstroke heatstrokeAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_Heatstroke");
          StringBuilder sb = new StringBuilder();
          var heatstrokePawns = (List<Pawn>)Reflection.RimWorld_Alert_Heatstroke_HeatstrokePawnsResult.GetValue(heatstrokeAlert);
          var heatstrokePawn = heatstrokePawns.RandomElement();
          _target = heatstrokePawn;
          Subject = string.Format("RimDialogue.Alert_Heatstroke".Translate(), heatstrokePawn.Name.ToStringShort);
          var heatstrokeHediff = heatstrokePawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Heatstroke, mustBeVisible: true);
          sb.AppendLine(string.Format("RimDialogue.Alert_Heatstroke_Stage".Translate(), heatstrokePawn.Name.ToStringShort, heatstrokeHediff.CurStage.label));
          if (heatstrokePawn.IsOutside())
            sb.AppendLine(string.Format("RimDialogue.Alert_Heatstroke_Outside".Translate(), H.TemperatureFeel(Find.CurrentMap.mapTemperature.OutdoorTemp)));
          else
          {
            var room = heatstrokePawn.GetRoom();
            sb.AppendLine(string.Format("RimDialogue.Alert_Heatstroke_Room".Translate(), heatstrokePawn.Name.ToStringShort, H.TemperatureFeel(room.Temperature)));
          }
          sb.AppendLine(string.Format("RimDialogue.Alert_Heatstroke_ComfortRange".Translate(), heatstrokePawn.Name.ToStringShort, heatstrokePawn.ComfortableTemperatureRange()));
          sb.AppendLine(string.Format("RimDialogue.Alert_Heatstroke_LifeThreatening".Translate(), heatstrokePawn.Name.ToStringShort, (heatstrokeHediff.IsCurrentlyLifeThreatening ? "RimDialogue.LifeThreatening".Translate() : "RimDialogue.NotLifeThreatening".Translate())));
          Explanation = sb.ToString();
          break;
        case Alert_Hypothermia hypothermiaAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_Hypothermia");
          var hypothermiaPawns = (List<Pawn>)Reflection.RimWorld_Alert_Hypothermia_HypothermiaPawnsResult.GetValue(hypothermiaAlert);
          var hypothermiaPawn = hypothermiaPawns.RandomElement();
          _target = hypothermiaPawn;
          Subject = string.Format("RimDialogue.Alert_Hypothermia".Translate(), hypothermiaPawn.Name.ToStringShort);
          var hypothermiaHediff = hypothermiaPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia, mustBeVisible: true);
          StringBuilder hypothermiaSb = new StringBuilder();
          hypothermiaSb.AppendLine(string.Format("RimDialogue.Alert_Hypothermia_Stage".Translate(), hypothermiaPawn.Name.ToStringShort, hypothermiaHediff.CurStage.label));
          if (hypothermiaPawn.IsOutside())
            hypothermiaSb.AppendLine(string.Format("RimDialogue.Alert_Hypothermia_Outside".Translate(), H.TemperatureFeel(Find.CurrentMap.mapTemperature.OutdoorTemp)));
          else
          {
            var room = hypothermiaPawn.GetRoom();
            hypothermiaSb.AppendLine(string.Format("RimDialogue.Alert_Hypothermia_Room".Translate(), hypothermiaPawn.Name.ToStringShort, H.TemperatureFeel(room.Temperature)));
          }
          hypothermiaSb.AppendLine(string.Format("RimDialogue.Alert_Hypothermia_ComfortRange".Translate(), hypothermiaPawn.Name.ToStringShort, hypothermiaPawn.ComfortableTemperatureRange()));
          hypothermiaSb.AppendLine(string.Format("RimDialogue.Alert_Hypothermia_LifeThreatening".Translate(), hypothermiaPawn.Name.ToStringShort, (hypothermiaHediff.IsCurrentlyLifeThreatening ? "RimDialogue.LifeThreatening".Translate() : "RimDialogue.NotLifeThreatening".Translate())));
          Explanation = hypothermiaSb.ToString();
          break;
        case Alert_HypothermicAnimals hypothermicAnimalsAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_HypothermicAnimals");
          var hypothermicAnimals = (List<Pawn>)Reflection.RimWorld_Alert_HypothermicAnimals_HypothermicAnimalsResult.GetValue(hypothermicAnimalsAlert);
          var hypothermicAnimal = hypothermicAnimals.RandomElement();
          _target = hypothermicAnimal;
          Subject = string.Format("RimDialogue.Alert_HypothermicAnimals".Translate(), hypothermicAnimal.Name?.ToStringShort ?? hypothermicAnimal.def.label);
          var hypothermicAnimalHediff = hypothermicAnimal.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia, mustBeVisible: true);
          StringBuilder hypothermicAnimalSb = new StringBuilder();
          hypothermicAnimalSb.AppendLine(string.Format("RimDialogue.Alert_HypothermicAnimals_Stage".Translate(), hypothermicAnimal.Name?.ToStringShort ?? hypothermicAnimal.def.label, hypothermicAnimalHediff.CurStage.label));
          hypothermicAnimalSb.AppendLine(string.Format("RimDialogue.Alert_HypothermicAnimals_Outside".Translate(), H.TemperatureFeel(Find.CurrentMap.mapTemperature.OutdoorTemp)));
          hypothermicAnimalSb.AppendLine(string.Format("RimDialogue.Alert_HypothermicAnimals_ComfortRange".Translate(), hypothermicAnimal.Name?.ToStringShort ?? hypothermicAnimal.def.label, hypothermicAnimal.ComfortableTemperatureRange()));
          hypothermicAnimalSb.AppendLine(string.Format("RimDialogue.Alert_HypothermicAnimals_LifeThreatening".Translate(), hypothermicAnimal.Name?.ToStringShort ?? hypothermicAnimal.def.label, (hypothermicAnimalHediff.IsCurrentlyLifeThreatening ? "RimDialogue.LifeThreatening".Translate() : "RimDialogue.NotLifeThreatening".Translate())));
          Explanation = hypothermicAnimalSb.ToString();
          break;
        case Alert_IdeoBuildingDisrespected ideoBuildingDisrespectedAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_IdeoBuildingDisrespected");
          var demand = (IdeoBuildingPresenceDemand)Reflection.RimWorld_Alert_IdeoBuildingDisrespected_Demand.GetValue(ideoBuildingDisrespectedAlert);
          Subject = string.Format("RimDialogue.Alert_IdeoBuildingDisrespected".Translate(), demand.parent.ideo.name);
          Explanation = string.Format("RimDialogue.Alert_IdeoBuildingDisrespected_Explanation".Translate(), demand.parent.ideo.name, demand.RoomRequirementsInfo.ToLineList(" - "));
          break;
        case Alert_IdeoBuildingMissing ideoBuildingMissingAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_IdeoBuildingMissing");
          var demand2 = (IdeoBuildingPresenceDemand)Reflection.RimWorld_Alert_IdeoBuildingMissing_Demand.GetValue(ideoBuildingMissingAlert);
          Subject = string.Format("RimDialogue.Alert_IdeoBuildingMissing".Translate(), demand2.parent.ideo.name);
          break;
        case Alert_LowFood lowFoodAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_LowFood");
          Subject = "RimDialogue.Alert_LowFood".Translate();
          var huntableAnimals = Find.CurrentMap.mapPawns.AllPawnsSpawned
            .Where(pawn => !pawn.RaceProps.Humanlike && !pawn.IsColonist)
            .Select(pawn => pawn.def.label)
            .Distinct()
            .ToList();
          var harvestableFood = Find.CurrentMap.listerThings
            .ThingsInGroup(ThingRequestGroup.FoodSource)
            .Where(foodSource => foodSource.def.ingestible != null)
            .Select(foodSource => foodSource.def.label)
            .Distinct()
            .ToArray();
          StringBuilder sbLowFoodAlert = new StringBuilder();
          sbLowFoodAlert.AppendLine(string.Format("RimDialogue.Alert_LowFood_Animals".Translate(), string.Join(", ", huntableAnimals)));
          sbLowFoodAlert.AppendLine(string.Format("RimDialogue.Alert_LowFood_Harvestable".Translate(), string.Join(", ", harvestableFood)));
          Explanation = sbLowFoodAlert.ToString();
          break;
        case Alert_LowMedicine lowMedicineAlert:
          Subject = "RimDialogue.Alert_LowMedicine".Translate();
          break;
        case Alert_MajorOrExtremeBreakRisk majorBreakRiskAlert:
          var majorBreakRiskPawns = (List<Pawn>)Reflection.RimWorld_Alert_BreakRiskAlertUtility_PawnsAtRiskMajorResult.GetValue(null);
          var majorBreakRiskPawn = majorBreakRiskPawns.RandomElement();
          Subject = string.Format("RimDialogue.Alert_MajorBreakRisk".Translate(), majorBreakRiskPawn.Name.ToStringShort);
          _target = majorBreakRiskPawn;
          List<Thought> allMoodThoughts2 = [];
          majorBreakRiskPawn.needs.mood.thoughts.GetAllMoodThoughts(allMoodThoughts2);
          var negativeThoughts2 = allMoodThoughts2.Where(thought => thought.MoodOffset() < 0).ToArray();
          if (negativeThoughts2.Any())
          {
            var negativeThought = allMoodThoughts2.RandomElementByWeight(thought => Math.Abs(thought.MoodOffset()));
            Explanation = string.Format("RimDialogue.Alert_MajorBreakRisk_ThoughtExplanation".Translate(), negativeThought.LabelCap, majorBreakRiskPawn.Name.ToStringShort, negativeThought.Description);
          }
          else
            Explanation = string.Format("RimDialogue.Alert_MajorBreakRisk_GenericExplanation".Translate(), majorBreakRiskPawn.Name.ToStringShort);
          break;
        case Alert_MinorBreakRisk minorBreakRiskAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_MinorBreakRisk");
          var minorBreakRiskPawns = (List<Pawn>)Reflection.RimWorld_Alert_BreakRiskAlertUtility_PawnsAtRiskMinorResult.GetValue(null);
          var minorBreakRiskPawn = minorBreakRiskPawns.RandomElement();
          Subject = string.Format("RimDialogue.Alert_MinorBreakRisk".Translate(), minorBreakRiskPawn.Name.ToStringShort);
          _target = minorBreakRiskPawn;
          List<Thought> allMoodThoughts = [];
          minorBreakRiskPawn.needs.mood.thoughts.GetAllMoodThoughts(allMoodThoughts);
          var negativeThoughts = allMoodThoughts.Where(thought => thought.MoodOffset() < 0).ToArray();
          if (negativeThoughts.Any())
          {
            var negativeThought = negativeThoughts.RandomElementByWeight(thought => Math.Abs(thought.MoodOffset()));
            Explanation = string.Format(
              "RimDialogue.Alert_MinorBreakRisk_ThoughtExplanation".Translate(),
              negativeThought.LabelCap,
              minorBreakRiskPawn.Name.ToStringShort,
              negativeThought.Description);
          }
          else
            Explanation = string.Format(
              "RimDialogue.Alert_MinorBreakRisk_GenericExplanation".Translate(),
              minorBreakRiskPawn.Name.ToStringShort);
          break;
        case Alert_NeedColonistBeds needColonistBedsAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedColonistBeds");
          Subject = "RimDialogue.Alert_NeedColonistBeds".Translate();
          break;
        case Alert_NeedDefenses needDefensesAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedDefenses");
          Subject = "RimDialogue.Alert_NeedDefenses".Translate();
          Explanation = "RimDialogue.Alert_NeedDefenses_Explanation".Translate();
          break;
        case Alert_NeedDoctor needDoctorAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedDoctor");
          Subject = "RimDialogue.Alert_NeedDoctor".Translate();
          Explanation = "RimDialogue.Alert_NeedDoctor_Explanation".Translate();
          break;
        case Alert_NeedJoySources needJoySourcesAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedJoySources");
          Subject = "RimDialogue.Alert_NeedJoySources".Translate();
          Explanation = "RimDialogue.Alert_NeedJoySources_Explanation".Translate();
          break;
        case Alert_NeedMealSource needMealSourceAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedMealSource");
          Subject = "RimDialogue.Alert_NeedMealSource".Translate();
          Explanation = "RimDialogue.Alert_NeedMealSource_Explanation".Translate();
          break;
        case Alert_NeedResearchBench needResearchBenchAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedResearchBench");
          Subject = "RimDialogue.Alert_NeedResearchBench".Translate();
          Explanation = "RimDialogue.Alert_NeedResearchBench_Explanation".Translate();
          break;
        case Alert_NeedResearchProject needResearchProjectAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedResearchProject");
          Subject = "RimDialogue.Alert_NeedResearchProject".Translate();
          Explanation = "RimDialogue.Alert_NeedResearchProject_Explanation".Translate();
          break;
        case Alert_NeedWarmClothes needWarmClothesAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedWarmClothes");
          Subject = "RimDialogue.Alert_NeedWarmClothes".Translate();
          Explanation = "RimDialogue.Alert_NeedWarmClothes_Explanation".Translate();
          break;
        case Alert_QuestExpiresSoon questExpiresSoonAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_QuestExpiresSoon");
          var quest = (Quest)Reflection.RimWorld_Alert_QuestExpiresSoon_QuestExpiring.GetValue(questExpiresSoonAlert);
          Subject = string.Format("RimDialogue.Alert_QuestExpiresSoon".Translate(), quest.name);
          Explanation = quest.description;
          break;
        case Alert_StarvationAnimals starvationAnimalsAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_StarvationAnimals");
          var starvationAnimals = (List<Pawn>)Reflection.RimWorld_Alert_StarvationAnimals_StarvingAnimalsResult.GetValue(starvationAnimalsAlert);
          var starvationAnimal = starvationAnimals.RandomElement();
          _target = starvationAnimal;
          Subject = string.Format("RimDialogue.Alert_StarvationAnimals".Translate(), starvationAnimal.Name.ToStringShort);
          break;
        case Alert_StarvationColonists starvationColonistsAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_StarvationColonists");
          var starvationColonists = (List<Pawn>)Reflection.RimWorld_Alert_StarvationColonists_StarvingColonistsResult.GetValue(starvationColonistsAlert);
          var starvationColonist = starvationColonists.RandomElement();
          _target = starvationColonist;
          Subject = string.Format("RimDialogue.Alert_StarvationColonists".Translate(), starvationColonist.Name.ToStringShort);
          break;
        //case Alert_HunterLacksRangedWeapon hunterAlert:
        //  // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_HunterLacksRangedWeapon");
        //  var hunters = (List<Pawn>)Reflection.RimWorld_Alert_HunterLacksRangedWeapon_HuntersWithoutRangedWeaponResult.GetValue(hunterAlert);
        //  var hunter = hunters.RandomElement();
        //  _target = hunter;
        //  Subject = $"our hunter {hunter.Name.ToStringShort} does not have a ranged weapon";
        //  var weapon = hunter.equipment.Primary;
        //  Explanation = weapon == null ? $"{hunter.Name.ToStringShort} has no weapon equipped." : $"{hunter.Name.ToStringShort} has a {weapon.Label} equipped.";
        //  break;
        case Alert_UnhappyNudity unhappyNudityAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_UnhappyNudity");
          var unhappyNudityPawns = (List<Pawn>)Reflection.RimWorld_Alert_UnhappyNudity_AffectedPawnsResult.GetValue(unhappyNudityAlert);
          var unhappyNudityPawn = unhappyNudityPawns.RandomElement();
          _target = unhappyNudityPawn;
          Subject = string.Format("RimDialogue.Alert_UnhappyNudity".Translate(), unhappyNudityPawn.Name.ToStringShort);
          break;
        case Alert_LifeThreateningHediff lifeThreateningHediffAlert:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Alert_LifeThreateningHediff");
          var sickPawns = (List<Pawn>)Reflection.RimWorld_Alert_LifeThreateningHediff_SickPawnsResult.GetValue(lifeThreateningHediffAlert);
          var sickPawn = sickPawns.RandomElement();
          _target = sickPawn;
          Subject = string.Format("RimDialogue.Alert_LifeThreateningHediff".Translate(), sickPawn.Name.ToStringShort);
          var criticalHediffs = sickPawn.health.hediffSet.hediffs
            .Where(hediff => hediff.IsCurrentlyLifeThreatening)
            .OrderByDescending(hediff => hediff.Severity)
            .ToArray();
          Explanation = string.Format("RimDialogue.Alert_LifeThreateningHediff_Explanation".Translate(), sickPawn.Name.ToStringShort, string.Join(", ", criticalHediffs.Select(hediff => hediff.LabelCap)));
          break;
        default:
          if (alert != null)
          {
            Subject = alert.Label;
            Explanation = alert.GetExplanation();
          }
          else
          {
            Mod.Warning($"Entry {entry.LogID} - No alerts to choose from.");
            Subject = "RimDialogue.Alert_Generic".Translate();
          }
          break;
      }
    }

    public override async Task BuildData(DialogueDataAlert data)
    {
      data.Explanation = Explanation;
      await base.BuildData(data);
    }

    public override string? Action => null;

    public override Rule[] Rules => [new Rule_String("alert", Subject)];

  }
}
