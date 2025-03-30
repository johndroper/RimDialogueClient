#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestAlert<DataT> : DialogueRequest<DataT> where DataT : DialogueDataAlert, new()
  {
    const string AlertPlaceholder = "**alert**";

    public static DialogueRequestAlert<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestAlert<DataT>(entry, interactionTemplate);
    }

    public Pawn? Target { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;

    public PawnData? targetData;

    public DialogueRequestAlert(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for alert {entry.LogID} with template {interactionTemplate}.");
      var alerts = (List<Alert>)Reflection.RimWorld_AlertsReadout_ActiveAlerts.GetValue(Find.Alerts);
      var alert = alerts.RandomElement();
      switch (alert)
      {
        case Alert_AbandonedBaby abandonedBabyAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AbandonedBaby");
          var abandonedBabies = (List<Pawn>)Reflection.RimWorld_Alert_AbandonedBaby_tmpAbandonedBabiesList.GetValue(abandonedBabyAlert);
          var abandonedBaby = abandonedBabies.RandomElement();
          Target = abandonedBaby;
          Subject = $"the baby {abandonedBaby.Name.ToStringShort} has been abandoned";
          if (abandonedBaby.IsOutside())
            Explanation = $"{abandonedBaby.Name.ToStringShort} is outside.";
          else
          {
            var room = abandonedBaby.GetRoom();
            Explanation = $"The baby {abandonedBaby.Name.ToStringShort} is in the {room.GetRoomRoleLabel()}.";
          }
          ;
          break;
        case Alert_AnimalFilth animalFilthAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalFilth");
          var animalFilthTargets = (List<GlobalTargetInfo>)Reflection.RimWorld_Alert_AnimalFilth_Targets.GetValue(animalFilthAlert);
          var animalFilthTarget = animalFilthTargets.RandomElement();
          var animalFilthPawn = animalFilthTarget.Pawn;
          Target = animalFilthPawn;
          Subject = $"the animal {animalFilthPawn.Name.ToStringShort} is making a mess inside";
          break;
        case Alert_AnimalPenNeeded animalPenAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalPenNeeded");
          var animalPenTargets = (List<GlobalTargetInfo>)Reflection.RimWorld_Alert_AnimalPenNeeded_Targets.GetValue(animalPenAlert);
          var animalPenTarget = animalPenTargets.RandomElement();
          var animalPenPawn = animalPenTarget.Pawn;
          Target = animalPenPawn;
          Subject = $"the animal '{animalPenPawn.Label}' needs a pen";
          break;
        case Alert_AnimalPenNotEnclosed animalPenNotEnclosedAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalPenNotEnclosed");
          Subject = $"an animal pen is not enclosed";
          Explanation = $"If the animal pen is not fixed the animals inside may escape.";
          break;
        case Alert_AnimalRoaming animalRoamingAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_AnimalRoaming");
          var animalRoamingTargets = (List<GlobalTargetInfo>)Reflection.RimWorld_Alert_AnimalRoaming_Targets.GetValue(animalRoamingAlert);
          var animalRoamingTarget = animalRoamingTargets.RandomElement();
          var animalRoamingPawn = animalRoamingTarget.Pawn;
          Target = animalRoamingPawn;
          Subject = $"the animal '{animalRoamingPawn.Label}' is not in a pen and may run away";
          break;
        case Alert_Boredom boredomAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_Boredom");
          var boredomPawns = (List<Pawn>)Reflection.RimWorld_Alert_Boredom_BoredPawnsResult.GetValue(boredomAlert);
          var boredomPawn = boredomPawns.RandomElement();
          Target = boredomPawn;
          Subject = $"{boredomPawn.Name.ToStringShort} is bored.";
          break;
        case Alert_BrawlerHasRangedWeapon brawlerAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_BrawlerHasRangedWeapon");
          var brawlerPawns = (List<Pawn>)Reflection.RimWorld_Alert_BrawlerHasRangedWeapon_BrawlersWithRangedWeaponResult.GetValue(brawlerAlert);
          var brawlerPawn = brawlerPawns.RandomElement();
          Target = brawlerPawn;
          Subject = $"{brawlerPawn.Name.ToStringShort} is a brawler and has a ranged weapon";
          Explanation = $"{brawlerPawn.Name.ToStringShort} is a melee fighter and is equipped with ranged weapon.  This will make {brawlerPawn.Name.ToStringShort} unhappy.";
          break;
        case RimWorld.Alert_ColonistLeftUnburied colonistLeftUnburiedAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistLeftUnburied");
          var colonistLeftUnburiedThings = (List<Thing>)Reflection.RimWorld_Alert_ColonistLeftUnburied_UnburiedColonistCorpsesResult.GetValue(colonistLeftUnburiedAlert);
          var colonistLeftUnburiedCorpse = (Corpse)colonistLeftUnburiedThings.RandomElement();
          var colonistLeftUnburiedPawn = colonistLeftUnburiedCorpse.InnerPawn;
          Target = colonistLeftUnburiedPawn;
          Subject = $"{colonistLeftUnburiedPawn.Name.ToStringShort}'s corpse has been left unburied";
          Explanation = $"{colonistLeftUnburiedPawn.Name.ToStringShort} has died and their corpse has been left out in the open.";
          break;
        case Alert_ColonistNeedsRescuing colonistNeedsRescuingAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistNeedsRescuing");
          var colonistNeedsRescuingThings = (List<Pawn>)Reflection.RimWorld_Alert_ColonistNeedsRescuing_ColonistsNeedingRescueResult.GetValue(colonistNeedsRescuingAlert);
          var colonistNeedsRescuingPawn = colonistNeedsRescuingThings.RandomElement();
          Target = colonistNeedsRescuingPawn;
          Subject = $"{colonistNeedsRescuingPawn.Name.ToStringShort} is down and needs rescuing";
          var colonistNeedsRescuingHediffs = colonistNeedsRescuingPawn.health.hediffSet.hediffs
            .Where(hediff => hediff.Severity > 0)
            .OrderByDescending(hediff => hediff.Severity)
            .ToArray();
          Explanation = $"{colonistNeedsRescuingPawn.Name.ToStringShort} has the following injuries: {string.Join(", ", colonistNeedsRescuingHediffs.Select(hediff => hediff.LabelCap))}.";
          break;
        case Alert_ColonistNeedsTend colonistNeedsTendAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistNeedsTend");
          var colonistNeedsTendThings = (List<Pawn>)Reflection.RimWorld_Alert_ColonistNeedsTend_NeedingColonistsResult.GetValue(colonistNeedsTendAlert);
          var colonistNeedsTendPawn = colonistNeedsTendThings.RandomElement();
          Target = colonistNeedsTendPawn;
          Subject = $"{colonistNeedsTendPawn.Name.ToStringShort} needs tending";
          var colonistNeedsTendHediffs = colonistNeedsTendPawn.health.hediffSet.GetHediffsTendable()
            .OrderByDescending(hediff => hediff.Severity)
            .ToArray();
          Explanation = $"{colonistNeedsTendPawn.Name.ToStringShort} has the following injuries that need medical attention: {string.Join(", ", colonistNeedsTendHediffs.Select(hediff => hediff.LabelCap))}.";
          break;
        case Alert_ColonistsIdle idleAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_ColonistsIdle");
          var idlePawns = (List<Pawn>)Reflection.RimWorld_Alert_ColonistsIdle_IdleColonistsResult.GetValue(idleAlert);
          var idlePawn = idlePawns.RandomElement();
          Target = idlePawn;
          Subject = $"{idlePawn.Name.ToStringShort} does not have any work to do";
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
          Explanation = $"Types of work {idlePawn.Name.ToStringShort} is assigned to but has no work to do: {string.Join(", ", workPriorities)}";
          break;
        case Alert_DateRitualComing dateRitualComingAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_DateRitualComing");
          Precept_Ritual ritual = Faction.OfPlayer.ideos.AllIdeos
            .SelectMany(ideo => ideo.PreceptsListForReading.OfType<Precept_Ritual>())
            .Where(ritual => !ritual.isAnytime)
            .RandomElement();
          Subject = $"the ritual '{ritual.LabelCap}' is coming soon";
          Explanation = ritual.Description;
          break;
        case Alert_FireInHomeArea fireInHomeAreaAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_FireInHomeArea");
          Subject = $"a fire has started in the home area";
          break;
        case Alert_Heatstroke heatstrokeAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_Heatstroke");
          StringBuilder sb = new StringBuilder();
          var heatstrokePawns = (List<Pawn>)Reflection.RimWorld_Alert_Heatstroke_HeatstrokePawnsResult.GetValue(heatstrokeAlert);
          var heatstrokePawn = heatstrokePawns.RandomElement();
          Target = heatstrokePawn;
          Subject = $"{heatstrokePawn.Name.ToStringShort} is suffering from heatstroke";
          var heatstrokeHediff = heatstrokePawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Heatstroke, mustBeVisible: true);
          sb.AppendLine($"{heatstrokePawn.Name.ToStringShort}'s heatstroke is in the {heatstrokeHediff.CurStage.label} stage.");
          if (heatstrokePawn.IsOutside())
            sb.AppendLine($"The current temperature outside is {H.TemperatureFeel(Find.CurrentMap.mapTemperature.OutdoorTemp)} ");
          else
          {
            var room = heatstrokePawn.GetRoom();
            sb.AppendLine($"The current temperature of the room {heatstrokePawn.Name.ToStringShort} is in is {H.TemperatureFeel(room.Temperature)}");
          }
          sb.AppendLine($"{heatstrokePawn.Name.ToStringShort}'s comfortable temperature range is {heatstrokePawn.ComfortableTemperatureRange()}");
          sb.AppendLine($"{heatstrokePawn.Name.ToStringShort}'s heatstroke is {(heatstrokeHediff.IsCurrentlyLifeThreatening ? "life threatening" : "not life threatening")}.");
          Explanation = sb.ToString();
          break;
        case Alert_Hypothermia hypothermiaAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_Hypothermia");
          var hypothermiaPawns = (List<Pawn>)Reflection.RimWorld_Alert_Hypothermia_HypothermiaPawnsResult.GetValue(hypothermiaAlert);
          var hypothermiaPawn = hypothermiaPawns.RandomElement();
          Target = hypothermiaPawn;
          Subject = $"{hypothermiaPawn.Name.ToStringShort} is suffering from hypothermia";
          var hypothermiaHediff = hypothermiaPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia, mustBeVisible: true);
          StringBuilder hypothermiaSb = new StringBuilder();
          hypothermiaSb.AppendLine($"{hypothermiaPawn.Name.ToStringShort}'s hypothermia is in the {hypothermiaHediff.CurStage.label} stage.");
          if (hypothermiaPawn.IsOutside())
            hypothermiaSb.AppendLine($"The current temperature outside is {H.TemperatureFeel(Find.CurrentMap.mapTemperature.OutdoorTemp)} ");
          else
          {
            var room = hypothermiaPawn.GetRoom();
            hypothermiaSb.AppendLine($"The current temperature of the room {hypothermiaPawn.Name.ToStringShort} is in is {H.TemperatureFeel(room.Temperature)}");
          }
          hypothermiaSb.AppendLine($"{hypothermiaPawn.Name.ToStringShort}'s comfortable temperature range is {hypothermiaPawn.ComfortableTemperatureRange()}");
          hypothermiaSb.AppendLine($"{hypothermiaPawn.Name.ToStringShort}'s hypothermia is {(hypothermiaHediff.IsCurrentlyLifeThreatening ? "life threatening" : "not life threatening")}.");
          Explanation = hypothermiaSb.ToString();
          break;
        case Alert_HypothermicAnimals hypothermicAnimalsAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_HypothermicAnimals");
          var hypothermicAnimals = (List<Pawn>)Reflection.RimWorld_Alert_HypothermicAnimals_HypothermicAnimalsResult.GetValue(hypothermicAnimalsAlert);
          var hypothermicAnimal = hypothermicAnimals.RandomElement();
          Target = hypothermicAnimal;
          Subject = $"{hypothermicAnimal.Name.ToStringShort} is suffering from hypothermia";
          var hypothermicAnimalHediff = hypothermicAnimal.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia, mustBeVisible: true);
          StringBuilder hypothermicAnimalSb = new StringBuilder();
          hypothermicAnimalSb.AppendLine($"{hypothermicAnimal.Name.ToStringShort}'s hypothermia is in the {hypothermicAnimalHediff.CurStage.label} stage");
          hypothermicAnimalSb.AppendLine($"The current temperature outside is {H.TemperatureFeel(Find.CurrentMap.mapTemperature.OutdoorTemp)} ");
          hypothermicAnimalSb.AppendLine($"{hypothermicAnimal.Name.ToStringShort}'s comfortable temperature range is {hypothermicAnimal.ComfortableTemperatureRange()}");
          hypothermicAnimalSb.AppendLine($"{hypothermicAnimal.Name.ToStringShort}'s hypothermia is {(hypothermicAnimalHediff.IsCurrentlyLifeThreatening ? "life threatening" : "not life threatening")}.");
          Explanation = hypothermicAnimalSb.ToString();
          break;
        case Alert_IdeoBuildingDisrespected ideoBuildingDisrespectedAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_IdeoBuildingDisrespected");
          var demand = (IdeoBuildingPresenceDemand)Reflection.RimWorld_Alert_IdeoBuildingDisrespected_Demand.GetValue(ideoBuildingDisrespectedAlert);
          Subject = $"the temple of {demand.parent.ideo.name} has been disrespected";
          Explanation = $"The temple of {demand.parent.ideo.name} requires {demand.RoomRequirementsInfo.ToLineList(" - ")}.";
          break;
        case Alert_IdeoBuildingMissing ideoBuildingMissingAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_IdeoBuildingMissing");
          var demand2 = (IdeoBuildingPresenceDemand)Reflection.RimWorld_Alert_IdeoBuildingMissing_Demand.GetValue(ideoBuildingMissingAlert);
          Subject = $"we need a temple of {demand2.parent.ideo.name}.";
          break;
        case Alert_LowFood lowFoodAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_LowFood");
          Subject = $"the colony is running low on food";
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
          sbLowFoodAlert.AppendLine($"The following animals are available for hunting: {string.Join(", ", huntableAnimals)}");
          sbLowFoodAlert.AppendLine($"The following food sources are available for harvesting: {string.Join(", ", harvestableFood)}");
          Explanation = sbLowFoodAlert.ToString();
          break;
        case Alert_LowMedicine lowMedicineAlert:
          Subject = $"the colony is running low on medicine";
          break;
        case Alert_MajorOrExtremeBreakRisk majorBreakRiskAlert:
          var majorBreakRiskPawns = (List<Pawn>)Reflection.RimWorld_Alert_BreakRiskAlertUtility_PawnsAtRiskMajorResult.GetValue(null);
          var majorBreakRiskPawn = majorBreakRiskPawns.RandomElement();
          Subject = $"{majorBreakRiskPawn.Name.ToStringShort} is at risk of a serious metal break";
          Target = majorBreakRiskPawn;
          List<Thought> allMoodThoughts2 = [];
          majorBreakRiskPawn.needs.mood.thoughts.GetAllMoodThoughts(allMoodThoughts2);
          var negativeThoughts2 = allMoodThoughts2.Where(thought => thought.MoodOffset() < 0).OrderByDescending(thought => thought.MoodOffset()).ToArray();
          if (negativeThoughts2.Any())
            Explanation = $"'{negativeThoughts2[0].LabelCap}' is affecting {majorBreakRiskPawn.Name.ToStringShort} the most. {negativeThoughts2[0].Description}";
          else
            Explanation = $"{majorBreakRiskPawn.Name.ToStringShort} is feeling very depressed.";
          break;
        case Alert_MinorBreakRisk minorBreakRiskAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_MinorBreakRisk");
          var minorBreakRiskPawns = (List<Pawn>)Reflection.RimWorld_Alert_BreakRiskAlertUtility_PawnsAtRiskMinorResult.GetValue(null);
          var minorBreakRiskPawn = minorBreakRiskPawns.RandomElement();
          Subject = $"{minorBreakRiskPawn.Name.ToStringShort} is at risk of a metal break";
          Target = minorBreakRiskPawn;
          List<Thought> allMoodThoughts = [];
          minorBreakRiskPawn.needs.mood.thoughts.GetAllMoodThoughts(allMoodThoughts);
          var negativeThoughts = allMoodThoughts.Where(thought => thought.MoodOffset() < 0).OrderByDescending(thought => thought.MoodOffset()).ToArray();
          if (negativeThoughts.Any())
            Explanation = $"'{negativeThoughts[0].LabelCap}' is affecting {minorBreakRiskPawn.Name.ToStringShort} the most. {negativeThoughts[0].Description}";
          else
            Explanation = $"{minorBreakRiskPawn.Name.ToStringShort} is feeling down.";
          break;
        case Alert_NeedColonistBeds needColonistBedsAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedColonistBeds");
          Subject = $"we need more beds for our colonists";
          break;
        case Alert_NeedDefenses needDefensesAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedDefenses");
          Subject = $"we need more defenses.";
          Explanation = "The colony is vulnerable to attack";
          break;
        case Alert_NeedDoctor needDoctorAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedDoctor");
          Subject = $"we a doctor.";
          Explanation = "The colony does have any doctors";
          break;
        case Alert_NeedMealSource needMealSourceAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedMealSource");
          Subject = $"we need some way to cook a meal";
          Explanation = "The colony does not have any way to cook food.";
          break;
        case Alert_NeedResearchBench needResearchBenchAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedResearchBench");
          Subject = $"we need a research bench";
          Explanation = "The colony does not have any way to perform research.";
          break;
        case Alert_NeedResearchProject needResearchProjectAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedResearchProject");
          Subject = $"we need a research project";
          Explanation = "The colony does not have any research projects.";
          break;
        case Alert_NeedWarmClothes needWarmClothesAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_NeedWarmClothes");
          Subject = $"we need warm clothes";
          Explanation = "The colony does not have enough warm clothes and it will soon get cold.";
          break;
        case Alert_QuestExpiresSoon questExpiresSoonAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_QuestExpiresSoon");
          var quest = (Quest)Reflection.RimWorld_Alert_QuestExpiresSoon_QuestExpiring.GetValue(questExpiresSoonAlert);
          Subject = $"the quest '{quest.name}' is about to expire";
          Explanation = quest.description;
          break;
        case Alert_StarvationAnimals starvationAnimalsAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_StarvationAnimals");
          var starvationAnimals = (List<Pawn>)Reflection.RimWorld_Alert_StarvationAnimals_StarvingAnimalsResult.GetValue(starvationAnimalsAlert);
          var starvationAnimal = starvationAnimals.RandomElement();
          Target = starvationAnimal;
          Subject = $"the animal {starvationAnimal.Name.ToStringShort} is starving";
          break;
        case Alert_StarvationColonists starvationColonistsAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_StarvationColonists");
          var starvationColonists = (List<Pawn>)Reflection.RimWorld_Alert_StarvationColonists_StarvingColonistsResult.GetValue(starvationColonistsAlert);
          var starvationColonist = starvationColonists.RandomElement();
          Target = starvationColonist;
          Subject = $"{starvationColonist.Name.ToStringShort} is starving";
          break;
        case Alert_HunterLacksRangedWeapon hunterAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_HunterLacksRangedWeapon");
          var hunters = (List<Pawn>)Reflection.RimWorld_Alert_HunterLacksRangedWeapon_HuntersWithoutRangedWeaponResult.GetValue(hunterAlert);
          var hunter = hunters.RandomElement();
          Target = hunter;
          Subject = $"our hunter {hunter.Name.ToStringShort} does not have a ranged weapon";
          var weapon = hunter.equipment.Primary;
          Explanation = weapon == null ? $"{hunter.Name.ToStringShort} has no weapon equipped." : $"{hunter.Name.ToStringShort} has a {weapon.Label} equipped.";
          break;
        case Alert_UnhappyNudity unhappyNudityAlert:
          if (Settings.VerboseLogging.Value) Mod.Log($"Alert_UnhappyNudity");
          var unhappyNudityPawns = (List<Pawn>)Reflection.RimWorld_Alert_UnhappyNudity_AffectedPawnsResult.GetValue(unhappyNudityAlert);
          var unhappyNudityPawn = unhappyNudityPawns.RandomElement();
          Target = unhappyNudityPawn;
          Subject = $"{unhappyNudityPawn.Name.ToStringShort} is unhappy about being naked";
          break;
        default:
          if (alert != null)
            Subject = Subject.Replace(AlertPlaceholder, alert.Label);
          else
          {
            Mod.Warning($"No alerts to choose from for log entry {entry.LogID}.");
            Subject = Subject.Replace(AlertPlaceholder, "an alert");
          }
          break;
      }

      var tracker = H.GetTracker();
      if (Target is not null)
        targetData = H.MakePawnData(Target, tracker.GetInstructions(Target));
      else
        targetData = null;
    }

    public override void Build(DataT data)
    {
      data.Explanation = Explanation;
      base.Build(data);
    }

    public override void Execute()
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Executing dialogue request for alert {Entry.LogID}.");
      var alertData = new DataT();
      Build(alertData);
      Send(
        [
          new("chitChatJson", alertData),
          new("targetJson", targetData)
        ]);
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(AlertPlaceholder, Subject);
    }
  }
}
