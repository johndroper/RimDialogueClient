#nullable enable

using Bubbles.Core;
using HarmonyLib;
using RimDialogue.Core;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using Verse;
using System.Text.RegularExpressions;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(Bubbler), nameof(Bubbler.Add))]
  public static class Bubbles_Bubbler_Add
  {
    private static readonly Regex ColorTag = new("<\\/?color[^>]*>");
    private static readonly Regex WhiteSpace = new("\\s+");

    public static bool Prefix(LogEntry entry)
    {
      Add(entry);
      return false;
    }
    public static void Add(LogEntry entry)
    {
      var shouldShow = (bool)Reflection.Bubbles_Bubbler_ShouldShow.Invoke(null, null);

      if (!shouldShow) { return; }

      Pawn? initiator, recipient;

      switch (entry)
      {
        case PlayLogEntry_Interaction interaction:
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interaction);
          recipient = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(interaction);
          break;
        case PlayLogEntry_InteractionSinglePawn interaction:
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(interaction);
          recipient = null;
          break;
        default:
          return;
      }

      if (initiator is null || initiator.Map != Find.CurrentMap) { return; }

      var settings_DoNonPlayer = (Bubbles.Configuration.Setting<bool>)Reflection.Bubbles_Settings_DoNonPlayer.GetValue(null);
      var settings_DoAnimals = (Bubbles.Configuration.Setting<bool>)Reflection.Bubbles_Settings_DoAnimals.GetValue(null);
      var settings_DoDrafted = (Bubbles.Configuration.Setting<bool>)Reflection.Bubbles_Settings_DoDrafted.GetValue(null);

      if (!settings_DoNonPlayer.Value && (!initiator.Faction?.IsPlayer ?? true)) { return; }
      if (!settings_DoAnimals.Value && ((initiator.RaceProps?.Animal ?? false) || (recipient?.RaceProps?.Animal ?? false))) { return; }
      if (!settings_DoDrafted.Value && ((initiator.drafter?.Drafted ?? false) || (recipient?.drafter?.Drafted ?? false))) { return; }

      GetDialogue(initiator, recipient, entry);
    }

    public static void Clear()
    {
      dialogueDictionary.Clear();
    }

    public static Battle? GetMostRecentBattle(Pawn? pawn)
    {
      if (pawn == null)
        return null;

      return Find.BattleLog.Battles
        .Where(battle => battle.Concerns(pawn))
        .OrderByDescending(battle => battle.CreationTimestamp)
        .FirstOrDefault();
    }

    public static LogEntry[] GetCombatLogEntries(Pawn? pawn, int count)
    {
      if (pawn == null) { return Array.Empty<LogEntry>(); }

      var battle = GetMostRecentBattle(pawn);
      if (battle == null)
        return [];

      return battle.Entries
          .Where(entry => entry.Concerns(pawn))
          .OrderByDescending(entry => entry.Timestamp)
          .Take(count)
          .OrderBy(entry => entry.Timestamp)
          .ToArray();
    }

    public static string RemoveWhiteSpace(string? input)
    {
      if (input == null) return string.Empty;
      return WhiteSpace.Replace(input, " ");
    }

    public static string RemoveWhiteSpaceAndColor(string? input)
    {
      if (input == null) return string.Empty;
      input = ColorTag.Replace(input, string.Empty);
      return RemoveWhiteSpace(input);
    }

    public static string GetHediffString(Hediff hediff)
    {
      string text = hediff.Label;
      if (hediff.Part != null)
        text = text + " in their " + hediff.Part.Label;
      return text;
    }

    public static string GetBackstory(Pawn? pawn, BackstoryDef? backstory)
    {
      if (pawn == null || backstory == null)
        return string.Empty;

      return backstory.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
    }

    public static Dictionary<Bubble, string> dialogueDictionary = [];

    public static string? GetDialogueText(Bubble bubble)
    {
      if (dialogueDictionary.TryGetValue(bubble, out string text))
      {
        dialogueDictionary.Remove(bubble);
        return text;
      }
      return null;
    }

    public static async void GetDialogue(Pawn initiator, Pawn? recipient, LogEntry entry)
    {
      //Find.CurrentMap.StoryState?.RecentRandomQuests
      //Find.CurrentMap.StoryState?.RecentRandomDecrees;
      //initiator.BodySize
      //initiator.GetBeauty
      //initiator.GetTerrorLevel
      //initiator.interactions
      //initiator.inventory
      //initiator.IsGestating
      //initiator.IsWildMan
      //initiator.style.HasUnwantedBeard
      //Find.CurrentMap.listerThings.
      //recipient?.story.headType
      //recipient?.story.bodyType
      //recipient?.story.CaresAboutOthersAppearance
      //var blahhh = Find.CurrentMap.thingGrid.ThingsAt(initiator.Position);
      //initiator.Faction.GetReportText
      //initiator.Faction.HostileTo
      //initiator.Faction.PlayerRelationKind
      //initiator.Faction.def.Description
      //initiator.Faction.def.LabelCap
      //initiator.mechanitor
      //initiator.IsPrisonerOfColony
      //initiator.IsMutant
      //initiator.Inspired
      //initiator.AmbientTemperature
      //initiator.genes
      //pawn.ageTracker.CurKindLifeStage.label;
      //Find.FactionManager.AllFactionsVisible.Select(faction => faction.def.LabelCap).ToArray();
      //initiator.TraderKind.description
      //initiatorTraderKind = initiator.TraderKind?.label ?? string.Empty,
      //Find.CurrentMap.StoryState.lastRaidFaction
      //Faction.OfPirates;
      //Find.CurrentMap.resourceCounter?.AllCountedAmounts
      //Find.StoryWatcher.statsRecord.colonistsKilled
      //Find.CurrentMap.resourceCounter?.AllCountedAmounts.Where(thing => thing.Key.IsApparel).Sum(thing => thing.Value);
      //var thigncount = Find.CurrentMap.resourceCounter.GetCountIn(ThingRequestGroup.);

      if (Find.TickManager.CurTimeSpeed > (TimeSpeed)Settings.MaxSpeed.Value)
        return;

      try
      {
        var logEntryText = ColorTag.Replace(
          entry.ToGameStringFromPOV(initiator),
          string.Empty);

        List<Thought_Memory> initiatorMoodThoughts = initiator.needs?.mood?.thoughts?.memories?.Memories
          .Where(thoughtMemory => thoughtMemory.MoodOffset() != 0f)
          .ToList() ?? [];
        List<Thought_Memory> recipientMoodThoughts = recipient?.needs?.mood?.thoughts?.memories?.Memories
          .Where(thoughtMemory => thoughtMemory.MoodOffset() != 0f)
          .ToList() ?? [];

        var currentWeather = Find.CurrentMap.weatherManager.CurWeatherPerceived;
        Room room = initiator.GetRoom();

        string? initiatorPersonality = null;
        string? recipientPersonality = null;
        string? initiatorPersonalityDescription = null;
        string? recipientPersonalityDescription = null;
        if (Reflection.IsAssemblyLoaded("SP_Module1"))
        {
          try
          {
            Reflection.GetPersonality(initiator, out initiatorPersonality, out initiatorPersonalityDescription);
            Reflection.GetPersonality(recipient, out recipientPersonality, out recipientPersonalityDescription);
          }
          catch (Exception ex)
          {
            Mod.Warning($"Failed to get personality: {ex.Message}");
          }
        }

        var tracker = Current.Game.GetComponent<GameComponent_ConversationTracker>();
        var additionalInstructions = tracker.GetAdditionalInstructions(null);

        var dialogueData = new DialogueData
        {
          clientId = Settings.ClientId.Value,
          maxWords = Settings.MaxWords.Value,
          specialInstructions = Settings.SpecialInstructions.Value + " " + additionalInstructions,
          interaction = logEntryText,
          scenario = Find.Scenario?.name + " - " + RemoveWhiteSpace(Find.Scenario?.description),
          daysPassedSinceSettle = GenDate.DaysPassedSinceSettle,
          currentWeather = currentWeather.LabelCap + " - " + currentWeather.description,
          outdoorTemp = Find.CurrentMap.mapTemperature.OutdoorTemp,
          hourOfDay = GenLocalDate.HourOfDay(Find.CurrentMap),
          season = GenLocalDate.Season(Find.CurrentMap).ToString(),
          biome = Find.CurrentMap.Biome.label + " - " + RemoveWhiteSpace(Find.CurrentMap.Biome.description),
          recentIncidents = Find.CurrentMap.StoryState?.RecentRandomIncidents.Where(incident => incident?.label != null).Select(incident => incident.label).ToArray() ?? [],
          isOutside = initiator.IsOutside(),
          room = room?.GetRoomRoleLabel() ?? String.Empty,
          roomImpressiveness = room?.GetStat(RoomStatDefOf.Impressiveness) ?? -1f,
          roomCleanliness = room?.GetStat(RoomStatDefOf.Cleanliness) ?? -1f,
          wealthTotal = Find.CurrentMap.wealthWatcher?.WealthTotal ?? -1f,
          foodTotal = Find.CurrentMap.resourceCounter?.TotalHumanEdibleNutrition ?? -1f,
          defensesTotal = Find.CurrentMap.listerBuildings.allBuildingsColonist.Where(building => building.def.building != null && (building.def.building.IsTurret || building.def.building.isTrap) || building.def == ThingDefOf.Sandbags || building.def == ThingDefOf.Barricade).Count(),
          medicineTotal = Find.CurrentMap.resourceCounter?.GetCountIn(ThingRequestGroup.Medicine) ?? -1,
          drugsTotal = Find.CurrentMap.resourceCounter?.GetCountIn(ThingRequestGroup.Drug) ?? -1,
          colonistsCount = Find.CurrentMap.mapPawns?.FreeColonistsSpawnedCount ?? -1,
          prisonersCount = Find.CurrentMap.mapPawns?.PrisonersOfColonyCount ?? -1,
          otherFactions = Find.FactionManager.AllFactionsVisible.Where(faction => faction != Faction.OfPlayer).Select(faction => $"{faction.def.LabelCap.RawText} ({faction.RelationKindWith(Faction.OfPlayer)})").ToArray(),
          initiatorThingID = initiator.ThingID,
          initiatorInstructions = tracker.GetAdditionalInstructions(initiator),
          initiatorFullName = initiator.Name?.ToStringFull ?? String.Empty,
          initiatorPersonality = initiatorPersonality ?? string.Empty,
          initiatorPersonalityDescription = RemoveWhiteSpaceAndColor(initiatorPersonalityDescription),
          initiatorJobReport = initiator.GetJobReport(),
          initiatorCarrying = RemoveWhiteSpaceAndColor(initiator.carryTracker?.CarriedThing?.LabelCap),
          initiatorNickName = initiator.Name?.ToStringShort ?? initiator.Label ?? String.Empty,
          initiatorGender = initiator.GetGenderLabel(),
          initiatorFactionName = initiator.Faction?.Name ?? String.Empty,
          initiatorFactionLabel = RemoveWhiteSpace(initiator.Faction?.def?.LabelCap),
          initiatorFactionDescription = RemoveWhiteSpace(initiator.Faction?.def?.description),
          initiatorDescription = RemoveWhiteSpace(initiator.DescriptionDetailed),
          initiatorRace = initiator.def?.defName ?? String.Empty,
          initiatorIsColonist = initiator.IsColonist,
          initiatorIsPrisoner = initiator.IsPrisoner,
          initiatorIsHostile = initiator.HostileTo(Faction.OfPlayer),
          initiatorRoyaltyTitle = initiator.royalty?.MostSeniorTitle?.Label ?? string.Empty,
          initiatorIsCreepJoiner = initiator.IsCreepJoiner,
          initiatorIsGhoul = initiator.IsGhoul,
          initiatorIsBloodFeeder = initiator.IsBloodfeeder(),
          initiatorIsSlave = initiator.IsSlave,
          initiatorIsAnimal = initiator.IsNonMutantAnimal,
          initiatorIdeologyName = initiator.Ideo?.name ?? string.Empty,
          initiatorIdeologyDescription = RemoveWhiteSpaceAndColor(initiator.Ideo?.description),
          initiatorIdeologyPrecepts = initiator.Ideo?.PreceptsListForReading?.Where(precept => precept.GetType() == typeof(Precept)).Select(precept => precept.Label + " - " + RemoveWhiteSpace(precept.Description)).ToArray() ?? [],
          initiatorAge = initiator.ageTracker?.AgeBiologicalYears ?? -1,
          initiatorHair = initiator.story?.hairDef?.label ?? string.Empty,
          initiatorFaceTattoo = initiator.style?.FaceTattoo?.label ?? string.Empty,
          initiatorBodyTattoo = initiator.style?.BodyTattoo?.label ?? string.Empty,
          initiatorBeard = initiator.style?.beardDef?.label ?? string.Empty,
          initiatorSkills = initiator.skills?.skills.Where(skill => skill.Level >= 5).Select(skill => $"{skill.LevelDescriptor} {skill.def?.label} - {RemoveWhiteSpace(skill.def?.description)}").ToArray() ?? [],
          initiatorTraits = initiator.story?.traits?.allTraits?.Select(trait => trait.Label + " - " + RemoveWhiteSpaceAndColor(trait.CurrentData.description.Formatted(initiator.Named("PAWN")).AdjustedFor(initiator).Resolve())).ToArray() ?? [],
          initiatorChildhood = initiator.story?.Childhood?.title != null ? initiator.story?.Childhood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(initiator, initiator.story?.Childhood)) : string.Empty,
          initiatorAdulthood = initiator.story?.Adulthood?.title != null ? initiator.story?.Adulthood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(initiator, initiator.story?.Adulthood)) : string.Empty,
          initiatorRelations = initiator.relations?.DirectRelations?.Select(relation => $"{relation.otherPawn?.Name?.ToStringFull} ({relation.def?.label})").ToArray() ?? [],
          initiatorApparel = initiator.apparel?.WornApparel?.Select(apparel => apparel.def?.label + " - " + RemoveWhiteSpace(apparel.def?.description)).ToArray() ?? [],
          initiatorWeapons = initiator.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.def.label + " - " + RemoveWhiteSpace(equipment.def.description)).ToArray() ?? [],
          initiatorHediffs = initiator.health.hediffSet?.hediffs?.Select(hediff => GetHediffString(hediff)).ToArray() ?? [],
          initiatorOpinionOfRecipient = recipient == null ? 0 : initiator.relations?.OpinionOf(recipient) ?? 0,
          initiatorMoodThoughts = initiatorMoodThoughts.Select(moodThought => RemoveWhiteSpace(moodThought.Description)).ToArray(),
          initiatorMoodString = initiator.needs?.mood?.MoodString ?? string.Empty,
          initiatorMoodPercentage = initiator.needs?.mood?.CurLevelPercentage ?? -1f,
          initiatorComfortPercentage = initiator.needs?.comfort?.CurLevelPercentage ?? -1f,
          initiatorFoodPercentage = initiator.needs?.food?.CurLevelPercentage ?? -1f,
          initiatorRestPercentage = initiator.needs?.rest?.CurLevelPercentage ?? -1f,
          initiatorJoyPercentage = initiator.needs?.joy?.CurLevelPercentage ?? -1f,
          initiatorBeautyPercentage = initiator.needs?.beauty?.CurLevelPercentage ?? -1f,
          initiatorDrugsDesirePercentage = initiator.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
          initiatorEnergyPercentage = initiator.needs?.energy?.CurLevelPercentage ?? -1f,
          initiatorLastBattle = RemoveWhiteSpaceAndColor(GetMostRecentBattle(initiator)?.GetName()),
          initiatorCombatLog = GetCombatLogEntries(initiator, 10).Select(entry => RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiator))).ToArray(),
          recipientThingID = recipient?.ThingID ?? string.Empty,
          recipientInstructions = tracker.GetAdditionalInstructions(recipient),
          recipientFullName = recipient?.Name?.ToStringFull ?? String.Empty,
          recipientPersonality = recipientPersonality ?? string.Empty,
          recipientPersonalityDescription = RemoveWhiteSpaceAndColor(recipientPersonalityDescription),
          recipientJobReport = recipient?.GetJobReport() ?? string.Empty,
          recipientCarrying = RemoveWhiteSpaceAndColor(recipient?.carryTracker?.CarriedThing?.LabelCap),
          recipientNickName = recipient?.Name?.ToStringShort ?? recipient?.Label ?? String.Empty,
          recipientRoyaltyTitle = recipient?.royalty?.MostSeniorTitle?.Label ?? String.Empty,
          recipientGender = recipient?.GetGenderLabel() ?? string.Empty,
          recipientFactionName = recipient?.Faction?.Name ?? String.Empty,
          recipientFactionLabel = RemoveWhiteSpace(recipient?.Faction?.def?.LabelCap),
          recipientFactionDescription = RemoveWhiteSpace(recipient?.Faction?.def?.description),
          recipientDescription = RemoveWhiteSpace(recipient?.DescriptionDetailed),
          recipientRace = recipient?.def.defName ?? String.Empty,
          recipientIdeologyName = recipient?.Ideo?.name ?? string.Empty,
          recipientIdeologyDescription = RemoveWhiteSpaceAndColor(recipient?.Ideo?.description),
          recipientIdeologyPrecepts = recipient?.Ideo?.PreceptsListForReading?.Where(precept => precept.GetType() == typeof(Precept)).Select(precept => precept.Label + " - " + RemoveWhiteSpace(precept.Description)).ToArray() ?? [],
          recipientAge = recipient?.ageTracker?.AgeBiologicalYears ?? -1,
          recipientHair = recipient?.story?.hairDef?.label ?? string.Empty,
          recipientFaceTattoo = recipient?.style?.FaceTattoo?.label ?? string.Empty,
          recipientBodyTattoo = recipient?.style?.BodyTattoo?.label ?? string.Empty,
          recipientBeard = recipient?.style?.beardDef?.label ?? string.Empty,
          recipientIsColonist = recipient?.IsColonist ?? false,
          recipientIsPrisoner = recipient?.IsPrisoner ?? false,
          recipientIsHostile = recipient?.HostileTo(Faction.OfPlayer) ?? false,
          recipientIsCreepJoiner = recipient?.IsCreepJoiner ?? false,
          recipientIsGhoul = recipient?.IsGhoul ?? false,
          recipientIsBloodfeeder = recipient?.IsBloodfeeder() ?? false,
          recipientIsSlave = recipient?.IsSlave ?? false,
          recipientIsAnimal = recipient?.IsNonMutantAnimal ?? false,
          recipientSkills = recipient?.skills?.skills?.Where(skill => skill.Level >= 5).Select(skill => $"{skill.LevelDescriptor} {skill.def?.label} - {RemoveWhiteSpace(skill.def?.description)}").ToArray() ?? [],
          recipientTraits = recipient?.story?.traits?.allTraits?.Select(trait => trait.Label + " - " + RemoveWhiteSpaceAndColor(trait.CurrentData.description.Formatted(recipient.Named("PAWN")).AdjustedFor(recipient).Resolve())).ToArray() ?? [],
          recipientChildhood = recipient?.story?.Childhood?.title != null ? recipient?.story?.Childhood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(recipient, recipient?.story?.Childhood)) : string.Empty,
          recipientAdulthood = recipient?.story?.Adulthood?.title != null ? recipient?.story?.Adulthood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(recipient, recipient?.story?.Adulthood)) : string.Empty,
          recipientRelations = recipient?.relations?.DirectRelations?.Select(relation => $"{relation.otherPawn?.Name?.ToStringFull} ({relation.def?.label})").ToArray() ?? [],
          recipientApparel = recipient?.apparel?.WornApparel?.Select(apparel => apparel.def?.label + " - " + RemoveWhiteSpace(apparel.def?.description)).ToArray() ?? [],
          recipientWeapons = recipient?.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.def.label + " - " + RemoveWhiteSpace(equipment.def.description)).ToArray() ?? [],
          recipientHediffs = recipient?.health?.hediffSet?.hediffs?.Select(hediff => GetHediffString(hediff)).ToArray() ?? [],
          recipientOpinionOfInitiator = recipient?.relations?.OpinionOf(initiator) ?? 0,
          recipientMoodThoughts = recipientMoodThoughts?.Select(moodThought => RemoveWhiteSpace(moodThought.Description)).ToArray() ?? [],
          recipientMoodString = recipient?.needs?.mood?.MoodString ?? string.Empty,
          recipientMoodPercentage = recipient?.needs?.mood?.CurLevelPercentage ?? -1f,
          recipientComfortPercentage = recipient?.needs?.comfort?.CurLevelPercentage ?? -1f,
          recipientFoodPercentage = recipient?.needs?.food?.CurLevelPercentage ?? -1f,
          recipientRestPercentage = recipient?.needs?.rest?.CurLevelPercentage ?? -1f,
          recipientJoyPercentage = recipient?.needs?.joy?.CurLevelPercentage ?? -1f,
          recipientBeautyPercentage = recipient?.needs?.beauty?.CurLevelPercentage ?? -1f,
          recipientDrugsDesirePercentage = recipient?.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
          recipientEnergyPercentage = recipient?.needs?.energy?.CurLevelPercentage ?? -1f,
          recipientLastBattle = RemoveWhiteSpaceAndColor(GetMostRecentBattle(recipient)?.GetName()),
          recipientCombatLog = GetCombatLogEntries(recipient, 10).Select(entry => RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(recipient))).ToArray(),
        };

        string dialogueDataJson = JsonUtility.ToJson(dialogueData);
        WWWForm form = new WWWForm();
        form.AddField("dialogueDataJSON", dialogueDataJson);
        using (UnityWebRequest request = UnityWebRequest.Post(Settings.ServerUrl.Value, form))
        {
          var asyncOperation = request.SendWebRequest();
          while (!asyncOperation.isDone)
          {
            await Task.Yield();
          }
          if (request.isNetworkError || request.isHttpError)
          {
            throw new Exception($"Network error: {request.error}");
          }
          else
          {
            while (!request.downloadHandler.isDone) { await Task.Yield(); }
            var body = request.downloadHandler.text;
            var dialogueResponse = JsonUtility.FromJson<DialogueResponse>(body);
            if (dialogueResponse.rateLimited)
            {
              Mod.Log("Rate limited.");
              return;
            }
            tracker.AddConversation(initiator, recipient, dialogueResponse.text);

            var bubbleDictionary = Reflection.Bubbles_Bubbler_Dictionary.GetValue(null) as Dictionary<Pawn, List<Bubble>>;
            if (bubbleDictionary == null)
              return;

            var bubble = new Bubble(initiator, entry);
            dialogueDictionary.Add(bubble, dialogueResponse.text ?? "NULL");

            if (!bubbleDictionary.ContainsKey(initiator))
              bubbleDictionary[initiator] = new List<Bubble>();

            bubbleDictionary[initiator].Add(bubble);
          }
        }
      }
      catch (Exception ex)
      {
        Mod.Error($"A http post failed with error: [{ex.Source}: {ex.Message}]\n\nTrace:\n{ex.StackTrace}");
      }
    }
  }
}
