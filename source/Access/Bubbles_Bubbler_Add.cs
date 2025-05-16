#nullable enable

using Bubbles.Core;
using HarmonyLib;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(Bubbler), nameof(Bubbler.Add))]
  public static class Bubbles_Bubbler_Add
  {

    public static Dictionary<Bubble, string> dialogueDictionary = [];

    public static bool Prefix(LogEntry entry)
    {
      if (Settings.ShowInteractionBubbles.Value)
        return true;

      Pawn? initiator;
      switch (entry)
      {
        case PlayLogEntry_Interaction interaction:
          if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Interaction type is a 'PlayLogEntry_Interaction'");
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interaction);
          break;
        case PlayLogEntry_InteractionSinglePawn interaction:
          if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Interaction type is 'PlayLogEntry_InteractionSinglePawn'");
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(interaction);
          break;
        default:
          return false;
      }
      var logEntryText = H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiator));
      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Interaction text is '{logEntryText}'.");
      return false;
    }

    public static void Clear()
    {
      dialogueDictionary.Clear();
    }
    public static string? GetDialogueText(Bubble bubble)
    {
      if (dialogueDictionary.TryGetValue(bubble, out string text))
      {
        dialogueDictionary.Remove(bubble);
        return text;
      }
      return null;
    }

    private static Dictionary<Pawn, List<Bubble>>? bubbleDictionary = Reflection.Bubbles_Bubbler_Dictionary.GetValue(null) as Dictionary<Pawn, List<Bubble>>;

    public static void AddBubble(Pawn initiator, LogEntry entry, string bubbleText)
    {
      if (bubbleDictionary == null)
        return;
      var bubble = new Bubble(initiator, entry);
      dialogueDictionary.Add(bubble, bubbleText);
      if (!bubbleDictionary.ContainsKey(initiator))
      {
        bubbleDictionary[initiator] = new List<Bubble>();
        if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - New bubble dictionary created for pawn {initiator.thingIDNumber}.");
      }
      bubbleDictionary[initiator].Add(bubble);
      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Bubble added for pawn {initiator.thingIDNumber}.");
    }

    //public static async void GetDialogue(Pawn initiator, Pawn? recipient, string logEntryText, LogEntry entry, InteractionDef? interactionDef)
    //{

    //  #region future stuff
    //  //Find.CurrentMap.StoryState?.RecentRandomQuests
    //  //Find.CurrentMap.StoryState?.RecentRandomDecrees;
    //  //initiator.BodySize
    //  //initiator.GetBeauty
    //  //initiator.GetTerrorLevel
    //  //initiator.interactions
    //  //initiator.inventory
    //  //initiator.IsGestating
    //  //initiator.IsWildMan
    //  //initiator.style.HasUnwantedBeard
    //  //Find.CurrentMap.listerThings.
    //  //recipient?.story.headType
    //  //recipient?.story.bodyType
    //  //recipient?.story.CaresAboutOthersAppearance
    //  //var blahhh = Find.CurrentMap.thingGrid.ThingsAt(initiator.Position);
    //  //initiator.Faction.GetReportText
    //  //initiator.Faction.HostileTo
    //  //initiator.Faction.PlayerRelationKind
    //  //initiator.Faction.def.Description
    //  //initiator.Faction.def.LabelCap
    //  //initiator.mechanitor
    //  //initiator.IsPrisonerOfColony
    //  //initiator.IsMutant
    //  //initiator.Inspired
    //  //initiator.AmbientTemperature
    //  //initiator.genes
    //  //pawn.ageTracker.CurKindLifeStage.label;
    //  //Find.FactionManager.AllFactionsVisible.Select(faction => faction.def.LabelCap).ToArray();
    //  //initiator.TraderKind.description
    //  //initiatorTraderKind = initiator.TraderKind?.label ?? string.Empty,
    //  //Find.CurrentMap.StoryState.lastRaidFaction
    //  //Faction.OfPirates;
    //  //Find.CurrentMap.resourceCounter?.AllCountedAmounts
    //  //Find.StoryWatcher.statsRecord.colonistsKilled
    //  //Find.CurrentMap.resourceCounter?.AllCountedAmounts.Where(thing => thing.Key.IsApparel).Sum(thing => thing.Value);
    //  //var thigncount = Find.CurrentMap.resourceCounter.GetCountIn(ThingRequestGroup.);

    //  //initiator.interactions.InteractionsTrackerTick
    //  #endregion


    //  if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Getting dialogue.");

    //  try
    //  {
    //    List<Thought_Memory> initiatorMoodThoughts = initiator.needs?.mood?.thoughts?.memories?.Memories
    //      .Where(thoughtMemory => thoughtMemory.MoodOffset() != 0f)
    //      .ToList() ?? [];
    //    List<Thought_Memory> recipientMoodThoughts = recipient?.needs?.mood?.thoughts?.memories?.Memories
    //      .Where(thoughtMemory => thoughtMemory.MoodOffset() != 0f)
    //      .ToList() ?? [];


    //    if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Text: '{logEntryText}'.");

    //    var currentWeather = Find.CurrentMap.weatherManager.CurWeatherPerceived;
    //    Room room = initiator.GetRoom();

    //    string? initiatorPersonality = null;
    //    string? recipientPersonality = null;
    //    string? initiatorPersonalityDescription = null;
    //    string? recipientPersonalityDescription = null;
    //    if (Reflection.IsAssemblyLoaded("SP_Module1"))
    //    {
    //      try
    //      {
    //        if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Trying to get personality.");
    //        Reflection.GetPersonality(initiator, out initiatorPersonality, out initiatorPersonalityDescription);
    //        Reflection.GetPersonality(recipient, out recipientPersonality, out recipientPersonalityDescription);
    //      }
    //      catch (Exception ex)
    //      {
    //        Mod.Warning($"Entry {entry.LogID} - Failed to get personality: '{ex.Message}'.");
    //      }
    //    }
    //    var tracker = H.GetTracker();
    //    var additionalInstructions = tracker.GetInstructions(InstructionsSet.ALL_PAWNS);
    //    if (initiator.IsColonist || (recipient != null && recipient.IsColonist))
    //      additionalInstructions += "\r\n" + tracker.GetInstructions(InstructionsSet.COLONISTS);
    //    if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Instructions fetched: '{additionalInstructions}.");
    //    var dialogueData = new Core.DialogueData
    //    {
    //      clientId = Settings.ClientId.Value,
    //      maxWords = Settings.MaxWords.Value,
    //      specialInstructions = Settings.SpecialInstructions.Value + " " + additionalInstructions,
    //      interaction = logEntryText,
    //      daysPassedSinceSettle = GenDate.DaysPassedSinceSettle,
    //      currentWeather = currentWeather.LabelCap + " - " + currentWeather.description,
    //      outdoorTemp = Find.CurrentMap.mapTemperature.OutdoorTemp,
    //      hourOfDay = GenLocalDate.HourOfDay(Find.CurrentMap),
    //      season = GenLocalDate.Season(Find.CurrentMap).ToString(),
    //      biome = Find.CurrentMap.Biome.label + " - " + H.RemoveWhiteSpace(Find.CurrentMap.Biome.description),
    //      recentIncidents = Find.CurrentMap.StoryState?.RecentRandomIncidents.Where(incident => incident?.label != null).Select(incident => incident.label).ToArray() ?? [],
    //      isOutside = initiator.IsOutside(),
    //      room = room?.GetRoomRoleLabel() ?? string.Empty,
    //      roomImpressiveness = room?.GetStat(RoomStatDefOf.Impressiveness) ?? -1f,
    //      roomCleanliness = room?.GetStat(RoomStatDefOf.Cleanliness) ?? -1f,
    //      wealthTotal = Find.CurrentMap.wealthWatcher?.WealthTotal ?? -1f,
    //      foodTotal = Find.CurrentMap.resourceCounter?.TotalHumanEdibleNutrition ?? -1f,
    //      defensesTotal = Find.CurrentMap.listerBuildings.allBuildingsColonist.Where(building => building.def.building != null && (building.def.building.IsTurret || building.def.building.isTrap) || building.def == ThingDefOf.Sandbags || building.def == ThingDefOf.Barricade).Count(),
    //      medicineTotal = Find.CurrentMap.resourceCounter?.GetCountIn(ThingRequestGroup.Medicine) ?? -1,
    //      drugsTotal = Find.CurrentMap.resourceCounter?.GetCountIn(ThingRequestGroup.Drug) ?? -1,
    //      colonistsCount = Find.CurrentMap.mapPawns?.FreeColonistsSpawnedCount ?? -1,
    //      prisonersCount = Find.CurrentMap.mapPawns?.PrisonersOfColonyCount ?? -1,
    //      otherFactions = Find.FactionManager.AllFactionsVisible.Where(faction => faction != Faction.OfPlayer).Select(faction => $"{faction.def.LabelCap.RawText} ({faction.RelationKindWith(Faction.OfPlayer)})").ToArray(),
    //      initiatorThingID = initiator.ThingID,
    //      initiatorInstructions = tracker.GetInstructions(initiator),
    //      initiatorFullName = initiator.Name?.ToStringFull ?? string.Empty,
    //      initiatorPersonality = initiatorPersonality ?? string.Empty,
    //      initiatorPersonalityDescription = H.RemoveWhiteSpaceAndColor(initiatorPersonalityDescription),
    //      initiatorJobReport = initiator.GetJobReport(),
    //      initiatorCarrying = H.RemoveWhiteSpaceAndColor(initiator.carryTracker?.CarriedThing?.LabelCap),
    //      initiatorNickName = initiator.Name?.ToStringShort ?? initiator.Label ?? string.Empty,
    //      initiatorGender = initiator.GetGenderLabel(),
    //      initiatorFactionName = initiator.Faction?.Name ?? string.Empty,
    //      initiatorFactionLabel = H.RemoveWhiteSpace(initiator.Faction?.def?.LabelCap),
    //      initiatorFactionDescription = H.RemoveWhiteSpace(initiator.Faction?.def?.description),
    //      initiatorDescription = H.RemoveWhiteSpace(initiator.DescriptionDetailed),
    //      initiatorRace = initiator.def?.defName ?? string.Empty,
    //      initiatorIsColonist = initiator.IsColonist,
    //      initiatorIsPrisoner = initiator.IsPrisoner,
    //      initiatorIsHostile = initiator.HostileTo(Faction.OfPlayer),
    //      initiatorRoyaltyTitle = initiator.royalty?.MostSeniorTitle?.Label ?? string.Empty,
    //      initiatorIsCreepJoiner = initiator.IsCreepJoiner,
    //      initiatorIsGhoul = initiator.IsGhoul,
    //      initiatorIsBloodFeeder = initiator.IsBloodfeeder(),
    //      initiatorIsSlave = initiator.IsSlave,
    //      initiatorIsAnimal = initiator.IsNonMutantAnimal,
    //      initiatorIdeologyName = initiator.Ideo?.name ?? string.Empty,
    //      initiatorIdeologyDescription = H.RemoveWhiteSpaceAndColor(initiator.Ideo?.description),
    //      initiatorIdeologyPrecepts = initiator.Ideo?.PreceptsListForReading?.Where(precept => precept.GetType() == typeof(Precept)).Select(precept => precept.Label + " - " + H.RemoveWhiteSpace(precept.Description)).ToArray() ?? [],
    //      initiatorAge = initiator.ageTracker?.AgeBiologicalYears ?? -1,
    //      initiatorHair = initiator.story?.hairDef?.label ?? string.Empty,
    //      initiatorFaceTattoo = initiator.style?.FaceTattoo?.label ?? string.Empty,
    //      initiatorBodyTattoo = initiator.style?.BodyTattoo?.label ?? string.Empty,
    //      initiatorBeard = initiator.style?.beardDef?.label ?? string.Empty,
    //      initiatorSkills = initiator.skills?.skills.Where(skill => skill.Level >= 5).Select(skill => $"{skill.LevelDescriptor} {skill.def?.label} - {H.RemoveWhiteSpace(skill.def?.description)}").ToArray() ?? [],
    //      initiatorTraits = initiator.story?.traits?.allTraits?.Select(trait => trait.Label + " - " + H.RemoveWhiteSpaceAndColor(trait.CurrentData.description.Formatted(initiator.Named("PAWN")).AdjustedFor(initiator).Resolve())).ToArray() ?? [],
    //      initiatorChildhood = initiator.story?.Childhood?.title != null ? initiator.story?.Childhood?.title + " - " + H.RemoveWhiteSpaceAndColor(H.GetBackstory(initiator, initiator.story?.Childhood)) : string.Empty,
    //      initiatorAdulthood = initiator.story?.Adulthood?.title != null ? initiator.story?.Adulthood?.title + " - " + H.RemoveWhiteSpaceAndColor(H.GetBackstory(initiator, initiator.story?.Adulthood)) : string.Empty,
    //      initiatorRelations = initiator.relations?.DirectRelations?.Select(relation => $"{relation.otherPawn?.Name?.ToStringFull} ({relation.def?.label})").ToArray() ?? [],
    //      initiatorApparel = initiator.apparel?.WornApparel?.Select(apparel => apparel.def?.label + " - " + H.RemoveWhiteSpace(apparel.def?.description)).ToArray() ?? [],
    //      initiatorWeapons = initiator.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.def.label + " - " + H.RemoveWhiteSpace(equipment.def.description)).ToArray() ?? [],
    //      initiatorHediffs = initiator.health.hediffSet?.hediffs?.Select(hediff => H.GetHediffString(hediff)).ToArray() ?? [],
    //      initiatorOpinionOfRecipient2 = recipient == null ? 0 : initiator.relations?.OpinionOf(recipient) ?? 0,
    //      initiatorMoodThoughts = initiatorMoodThoughts.Select(moodThought => H.RemoveWhiteSpace(moodThought.Description)).ToArray(),
    //      initiatorMoodString = initiator.needs?.mood?.MoodString ?? string.Empty,
    //      initiatorMoodPercentage = initiator.needs?.mood?.CurLevelPercentage ?? -1f,
    //      initiatorComfortPercentage = initiator.needs?.comfort?.CurLevelPercentage ?? -1f,
    //      initiatorFoodPercentage = initiator.needs?.food?.CurLevelPercentage ?? -1f,
    //      initiatorRestPercentage = initiator.needs?.rest?.CurLevelPercentage ?? -1f,
    //      initiatorJoyPercentage = initiator.needs?.joy?.CurLevelPercentage ?? -1f,
    //      initiatorBeautyPercentage = initiator.needs?.beauty?.CurLevelPercentage ?? -1f,
    //      initiatorDrugsDesirePercentage = initiator.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
    //      initiatorEnergyPercentage = initiator.needs?.energy?.CurLevelPercentage ?? -1f,
    //      initiatorLastBattle = H.RemoveWhiteSpaceAndColor(H.GetMostRecentBattle(initiator)?.GetName()),
    //      initiatorCombatLog = H.GetCombatLogEntries(initiator, 10).Select(entry => H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiator))).ToArray(),
    //      recipientThingID = recipient?.ThingID ?? string.Empty,
    //      recipientInstructions = recipient != null ? tracker.GetInstructions(recipient) : String.Empty,
    //      recipientFullName = recipient?.Name?.ToStringFull ?? string.Empty,
    //      recipientPersonality = recipientPersonality ?? string.Empty,
    //      recipientPersonalityDescription = H.RemoveWhiteSpaceAndColor(recipientPersonalityDescription),
    //      recipientJobReport = recipient?.GetJobReport() ?? string.Empty,
    //      recipientCarrying = H.RemoveWhiteSpaceAndColor(recipient?.carryTracker?.CarriedThing?.LabelCap),
    //      recipientNickName = recipient?.Name?.ToStringShort ?? recipient?.Label ?? string.Empty,
    //      recipientRoyaltyTitle = recipient?.royalty?.MostSeniorTitle?.Label ?? string.Empty,
    //      recipientGender = recipient?.GetGenderLabel() ?? string.Empty,
    //      recipientFactionName = recipient?.Faction?.Name ?? string.Empty,
    //      recipientFactionLabel = H.RemoveWhiteSpace(recipient?.Faction?.def?.LabelCap),
    //      recipientFactionDescription = H.RemoveWhiteSpace(recipient?.Faction?.def?.description),
    //      recipientDescription = H.RemoveWhiteSpace(recipient?.DescriptionDetailed),
    //      recipientRace = recipient?.def.defName ?? string.Empty,
    //      recipientIdeologyName = recipient?.Ideo?.name ?? string.Empty,
    //      recipientIdeologyDescription = H.RemoveWhiteSpaceAndColor(recipient?.Ideo?.description),
    //      recipientIdeologyPrecepts = recipient?.Ideo?.PreceptsListForReading?.Where(precept => precept.GetType() == typeof(Precept)).Select(precept => precept.Label + " - " + H.RemoveWhiteSpace(precept.Description)).ToArray() ?? [],
    //      recipientAge = recipient?.ageTracker?.AgeBiologicalYears ?? -1,
    //      recipientHair = recipient?.story?.hairDef?.label ?? string.Empty,
    //      recipientFaceTattoo = recipient?.style?.FaceTattoo?.label ?? string.Empty,
    //      recipientBodyTattoo = recipient?.style?.BodyTattoo?.label ?? string.Empty,
    //      recipientBeard = recipient?.style?.beardDef?.label ?? string.Empty,
    //      recipientIsColonist = recipient?.IsColonist ?? false,
    //      recipientIsPrisoner = recipient?.IsPrisoner ?? false,
    //      recipientIsHostile = recipient?.HostileTo(Faction.OfPlayer) ?? false,
    //      recipientIsCreepJoiner = recipient?.IsCreepJoiner ?? false,
    //      recipientIsGhoul = recipient?.IsGhoul ?? false,
    //      recipientIsBloodfeeder = recipient?.IsBloodfeeder() ?? false,
    //      recipientIsSlave = recipient?.IsSlave ?? false,
    //      recipientIsAnimal = recipient?.IsNonMutantAnimal ?? false,
    //      recipientSkills = recipient?.skills?.skills?.Where(skill => skill.Level >= 5).Select(skill => $"{skill.LevelDescriptor} {skill.def?.label} - {H.RemoveWhiteSpace(skill.def?.description)}").ToArray() ?? [],
    //      recipientTraits = recipient?.story?.traits?.allTraits?.Select(trait => trait.Label + " - " + H.RemoveWhiteSpaceAndColor(trait.CurrentData.description.Formatted(recipient.Named("PAWN")).AdjustedFor(recipient).Resolve())).ToArray() ?? [],
    //      recipientChildhood = recipient?.story?.Childhood?.title != null ? recipient?.story?.Childhood?.title + " - " + H.RemoveWhiteSpaceAndColor(H.GetBackstory(recipient, recipient?.story?.Childhood)) : string.Empty,
    //      recipientAdulthood = recipient?.story?.Adulthood?.title != null ? recipient?.story?.Adulthood?.title + " - " + H.RemoveWhiteSpaceAndColor(H.GetBackstory(recipient, recipient?.story?.Adulthood)) : string.Empty,
    //      recipientRelations = recipient?.relations?.DirectRelations?.Select(relation => $"{relation.otherPawn?.Name?.ToStringFull} ({relation.def?.label})").ToArray() ?? [],
    //      recipientApparel = recipient?.apparel?.WornApparel?.Select(apparel => apparel.def?.label + " - " + H.RemoveWhiteSpace(apparel.def?.description)).ToArray() ?? [],
    //      recipientWeapons = recipient?.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.def.label + " - " + H.RemoveWhiteSpace(equipment.def.description)).ToArray() ?? [],
    //      recipientHediffs = recipient?.health?.hediffSet?.hediffs?.Select(hediff => H.GetHediffString(hediff)).ToArray() ?? [],
    //      recipientOpinionOfInitiator2 = recipient?.relations?.OpinionOf(initiator) ?? 0,
    //      recipientMoodThoughts = recipientMoodThoughts?.Select(moodThought => H.RemoveWhiteSpace(moodThought.Description)).ToArray() ?? [],
    //      recipientMoodString = recipient?.needs?.mood?.MoodString ?? string.Empty,
    //      recipientMoodPercentage = recipient?.needs?.mood?.CurLevelPercentage ?? -1f,
    //      recipientComfortPercentage = recipient?.needs?.comfort?.CurLevelPercentage ?? -1f,
    //      recipientFoodPercentage = recipient?.needs?.food?.CurLevelPercentage ?? -1f,
    //      recipientRestPercentage = recipient?.needs?.rest?.CurLevelPercentage ?? -1f,
    //      recipientJoyPercentage = recipient?.needs?.joy?.CurLevelPercentage ?? -1f,
    //      recipientBeautyPercentage = recipient?.needs?.beauty?.CurLevelPercentage ?? -1f,
    //      recipientDrugsDesirePercentage = recipient?.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
    //      recipientEnergyPercentage = recipient?.needs?.energy?.CurLevelPercentage ?? -1f,
    //      recipientLastBattle = H.RemoveWhiteSpaceAndColor(H.GetMostRecentBattle(recipient)?.GetName()),
    //      recipientCombatLog = H.GetCombatLogEntries(recipient, 10).Select(entry => H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(recipient))).ToArray(),
    //    };
    //    string jsonData = JsonUtility.ToJson(dialogueData);
    //    if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - DialogueData fetched for entry.");
    //    WWWForm form = new WWWForm();
    //    form.AddField("dialogueDataJSON", jsonData);
    //    var dialogueResponse = await DialogueRequest.Post("home/GetDialogue", form, entry.LogID);
    //    if (dialogueResponse.rateLimited)
    //    {
    //      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Rate limited.");
    //      return;
    //    }
    //    tracker.AddConversation(initiator, recipient, logEntryText, dialogueResponse.text);
    //    if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Conversation added.");
    //    if (dialogueResponse.text == null)
    //      throw new Exception("Entry {entry.LogID} - Response text is null.");
    //    AddBubble(initiator, entry, dialogueResponse.text);
    //  }
    //  catch (Exception ex)
    //  {
    //    Mod.Error($"Entry {entry.LogID} - GetDialogue failed: {ex.ToString()}");
    //  }
    //}
  }
}
