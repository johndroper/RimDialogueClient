#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RimDialogue.Access;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using UnityEngine.Networking;
using Verse;
using static RimWorld.ColonistBar;

namespace RimDialogue.Core
{
  public static class Bubbler
  {
    private const float LabelPositionOffset = -0.6f;

    private static readonly Dictionary<Pawn, List<Bubble>> Dictionary = new();
    private static readonly Regex ColorTag = new("<\\/?color[^>]*>");
    private static readonly Regex WhiteSpace = new("\\s+");
    private static bool ShouldShow() => Settings.Activated && !WorldRendererUtility.WorldRenderedNow && (Settings.AutoHideSpeed.Value is Settings.AutoHideSpeedDisabled || (int)Find.TickManager!.CurTimeSpeed < Settings.AutoHideSpeed.Value);

    public static void Add(LogEntry entry)
    {

      //Mod.Log($"Adding log entry: {entry.LogID}");

      if (!ShouldShow()) { return; }

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

      if (!Settings.DoNonPlayer.Value && (!initiator.Faction?.IsPlayer ?? true)) { return; }
      if (!Settings.DoAnimals.Value && ((initiator.RaceProps?.Animal ?? false) || (recipient?.RaceProps?.Animal ?? false))) { return; }
      if (!Settings.DoDrafted.Value && ((initiator.drafter?.Drafted ?? false) || (recipient?.drafter?.Drafted ?? false))) { return; }
      
      //Mod.Log($"initiator name: {initiator.Name}");

      GetDialogue(initiator, recipient, entry);
    }

    private static string FormatOffset(float? offset) {
      if (offset == null)
          return string.Empty;
      return offset > 0 ? "+" + (int)offset : ((int)offset).ToString();
    }

    private static string RemoveWhiteSpace(string? input)
    {
      if (input == null) return string.Empty;
      return WhiteSpace.Replace(input, " ");
    }

    private static string RemoveWhiteSpaceAndColor(string? input)
    {
      if (input == null) return string.Empty;
      input = ColorTag.Replace(input, string.Empty);
      return RemoveWhiteSpace(input);
    }

    private static string GetHediffString(Hediff hediff)
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


    private static async void GetDialogue(Pawn initiator, Pawn? recipient, LogEntry entry)
    {
      try
      {
        var logEntryText = ColorTag.Replace(
          entry.ToGameStringFromPOV(initiator),
          string.Empty);
        Mod.Log(logEntryText);

        List<Thought_Memory> initiatorThoughtsAboutRecipient = initiator.needs?.mood?.thoughts?.memories?.Memories
          .Where(thoughtMemory => thoughtMemory is ISocialThought && thoughtMemory.otherPawn == recipient)
          .ToList() ?? [];
        List<Thought_Memory> recipientThoughtsAboutInitiator = recipient?.needs?.mood?.thoughts?.memories?.Memories
          .Where(thoughtMemory => thoughtMemory is ISocialThought && thoughtMemory.otherPawn == initiator)
          .ToList() ?? [];
        List<Thought_Memory> initiatorMoodThoughts = initiator.needs?.mood?.thoughts?.memories?.Memories
          .Where(thoughtMemory => thoughtMemory.MoodOffset() != 0f)
          .ToList() ?? [];
        List<Thought_Memory> recipientMoodThoughts = recipient?.needs?.mood?.thoughts?.memories?.Memories
          .Where(thoughtMemory => thoughtMemory.MoodOffset() != 0f)
          .ToList() ?? [];

        var currentWeather = Find.CurrentMap.weatherManager.CurWeatherPerceived;
        

        Room room = initiator.GetRoom();

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

        var dialogueData = new DialogueData
        {
          maxWords = Settings.MaxWords.Value,
          specialInstructions = Settings.SpecialInstructions.Value,
          interaction = logEntryText,
          scenario = Find.Scenario?.name + " - " + RemoveWhiteSpace(Find.Scenario?.description),
          daysPassedSinceSettle = GenDate.DaysPassedSinceSettle,
          currentWeather = currentWeather.LabelCap + " - " + currentWeather.description,
          outdoorTemp = Find.CurrentMap.mapTemperature.OutdoorTemp,
          biome = Find.CurrentMap.Biome.label + " - " + RemoveWhiteSpace(Find.CurrentMap.Biome.description),
          recentIncidents = Find.CurrentMap.StoryState?.RecentRandomIncidents.Where(incident => incident?.label != null).Select(incident => incident.label).ToArray() ?? [],
          isOutside = initiator.IsOutside(),
          room = room?.GetRoomRoleLabel() ?? String.Empty,
          wealthTotal = Find.CurrentMap.wealthWatcher?.WealthTotal ?? -1f,
          initiatorFullName = initiator.Name?.ToStringFull ?? String.Empty,
          initiatorNickName = initiator.Name?.ToStringShort ?? String.Empty,
          initiatorGender = initiator.GetGenderLabel(),
          initiatorFactionName = initiator.Faction?.Name ?? String.Empty,
          initiatorDescription = RemoveWhiteSpace(initiator.DescriptionDetailed),
          initiatorRace = initiator.def?.defName ?? String.Empty,
          initiatorIsColonist = initiator.IsColonist,
          initiatorIsPrisoner = initiator.IsPrisoner,
          initiatorRoyaltyTitle = initiator.royalty?.MostSeniorTitle?.Label ?? string.Empty,
          initiatorIsCreepJoiner = initiator.IsCreepJoiner,
          initiatorIsGhoul = initiator.IsGhoul,
          initiatorIsBloodFeeder = initiator.IsBloodfeeder(),
          initiatorIsSlave = initiator.IsSlave,
          initiatorIsAnimal = initiator.IsNonMutantAnimal,
          initiatorIdeology = initiator.Ideo?.description ?? string.Empty,
          initiatorAge = initiator.ageTracker?.AgeBiologicalYears ?? -1,
          initiatorHair = initiator.story?.hairDef?.label ?? string.Empty,
          initiatorFaceTattoo = initiator.style?.FaceTattoo?.label ?? string.Empty,
          initiatorBodyTattoo = initiator.style?.BodyTattoo?.label ?? string.Empty,
          initiatorBeard = initiator.style?.beardDef?.label ?? string.Empty,
          initiatorSkills = initiator.skills?.skills.Where(skill => skill.Level >= 5).Select(skill => $"{skill.LevelDescriptor} {skill.def?.label} - {RemoveWhiteSpace(skill.def?.description)}").ToArray() ?? [],
          initiatorTraits = initiator.story?.traits?.allTraits?.Select(trait => trait.Label + " - " + RemoveWhiteSpaceAndColor(trait.TipString(initiator))).ToArray() ?? [],
          initiatorChildhood = initiator.story?.Childhood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(initiator, initiator.story?.Childhood)),
          initiatorAdulthood = initiator.story?.Adulthood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(initiator, initiator.story?.Adulthood)),
          initiatorRelations = initiator.relations?.DirectRelations?.Select(relation => $"{relation.otherPawn?.Name?.ToStringFull} ({relation.def?.label})").ToArray() ?? [],
          initiatorApparel = initiator.apparel?.WornApparel?.Select(apparel => apparel.def?.label + " - " + RemoveWhiteSpace(apparel.def?.description)).ToArray() ?? [],
          initiatorWeapons = initiator.equipment?.AllEquipmentListForReading?.Select(equipment => equipment.def.label + " - " + RemoveWhiteSpace(equipment.def.description)).ToArray() ?? [],
          initiatorHediffs = initiator.health.hediffSet?.hediffs?.Select(hediff => GetHediffString(hediff)).ToArray() ?? [],
          initiatorOpinionOfRecipient = initiatorThoughtsAboutRecipient.Select(moodThought => moodThought.LabelCapSocial + " [" + (moodThought as ISocialThought)?.OpinionOffset() + "]").ToArray() ?? [],
          initiatorMoodThoughts = initiatorMoodThoughts.Select(moodThought => moodThought.Description + " [" + moodThought.MoodOffset().ToString("F") + "]"  ).ToArray(),
          initiatorMoodString = initiator.needs?.mood?.MoodString ?? string.Empty,
          initiatorMoodPercentage = initiator.needs?.mood?.CurLevelPercentage ?? -1f,
          initiatorComfortPercentage = initiator.needs?.comfort?.CurLevelPercentage ?? -1f,
          initiatorFoodPercentage = initiator.needs?.food?.CurLevelPercentage ?? -1f,
          initiatorRestPercentage = initiator.needs?.rest?.CurLevelPercentage ?? -1f,
          initiatorJoyPercentage = initiator.needs?.joy?.CurLevelPercentage ?? -1f,
          initiatorBeautyPercentage = initiator.needs?.beauty?.CurLevelPercentage ?? -1f,
          initiatorDrugsDesirePercentage = initiator.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
          initiatorEnergyPercentage = initiator.needs?.energy?.CurLevelPercentage ?? -1f,
          recipientFullName = recipient?.Name?.ToStringFull ?? String.Empty,
          recipientNickName = recipient?.Name?.ToStringShort ?? String.Empty,
          recipientRoyaltyTitle = recipient?.royalty?.MostSeniorTitle?.Label ?? String.Empty,
          recipientGender = recipient?.GetGenderLabel() ?? string.Empty,
          recipientFactionName = recipient?.Faction?.Name ?? String.Empty,
          recipientDescription = RemoveWhiteSpace(recipient?.DescriptionDetailed),
          recipientRace = recipient?.def.defName ?? String.Empty,
          recipientIdeology = recipient?.Ideo?.description ?? string.Empty,
          recipientAge = recipient?.ageTracker?.AgeBiologicalYears ?? -1,
          recipientHair = recipient?.story?.hairDef?.label ?? string.Empty,
          recipientFaceTattoo = recipient?.style?.FaceTattoo?.label ?? string.Empty,
          recipientBodyTattoo = recipient?.style?.BodyTattoo?.label ?? string.Empty,
          recipientBeard = recipient?.style?.beardDef?.label ?? string.Empty,
          recipientIsColonist = recipient?.IsColonist ?? false,
          recipientIsPrisoner = recipient?.IsPrisoner ?? false,
          recipientIsCreepJoiner = recipient?.IsCreepJoiner ?? false,
          recipientIsGhoul = recipient?.IsGhoul ?? false,
          recipientIsBloodfeeder = recipient?.IsBloodfeeder() ?? false,
          recipientIsSlave = recipient?.IsSlave ?? false,
          recipientIsAnimal = recipient?.IsNonMutantAnimal ?? false,
          recipientSkills = recipient?.skills?.skills?.Where(skill => skill.Level >= 5).Select(skill => $"{skill.LevelDescriptor} {skill.def?.label} - {RemoveWhiteSpace(skill.def?.description)}").ToArray() ?? [],
          recipientTraits = recipient?.story?.traits?.allTraits?.Select(trait => trait.Label + " - " + RemoveWhiteSpaceAndColor(trait.TipString(recipient))).ToArray() ?? [],
          recipientChildhood = recipient?.story?.Childhood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(recipient, recipient?.story?.Childhood)),
          recipientAdulthood = recipient?.story?.Adulthood?.title + " - " + RemoveWhiteSpaceAndColor(GetBackstory(recipient, recipient?.story?.Adulthood)),
          recipientRelations = recipient?.relations?.DirectRelations?.Select(relation => $"{relation.otherPawn?.Name?.ToStringFull} ({relation.def?.label})").ToArray() ?? [],
          recipientApparel = recipient?.apparel?.WornApparel?.Select(apparel => apparel.def?.label + " - " + RemoveWhiteSpace(apparel.DescriptionDetailed)).ToArray() ?? [],
          recipientWeapons = recipient?.equipment?.AllEquipmentListForReading?.Select(equipment => RemoveWhiteSpace(equipment.DescriptionDetailed)).ToArray() ?? [],
          recipientHediffs = recipient?.health?.hediffSet?.hediffs?.Select(hediff => GetHediffString(hediff)).ToArray() ?? [],
          recipientOpinionOfInitiator = recipientThoughtsAboutInitiator.Select(moodThought => moodThought.LabelCapSocial + " [" + (moodThought as ISocialThought)?.OpinionOffset() + "]").ToArray(),
          recipientMoodThoughts = recipientMoodThoughts.Select(moodThought => moodThought.Description + " [" + moodThought.MoodOffset().ToString("F") + "]").ToArray(),
          recipientMoodString = recipient?.needs?.mood?.MoodString ?? string.Empty,
          recipientMoodPercentage = recipient?.needs?.mood?.CurLevelPercentage ?? -1f,
          recipientComfortPercentage = recipient?.needs?.comfort?.CurLevelPercentage ?? -1f,
          recipientFoodPercentage = recipient?.needs?.food?.CurLevelPercentage ?? -1f,
          recipientRestPercentage = recipient?.needs?.rest?.CurLevelPercentage ?? -1f,
          recipientJoyPercentage = recipient?.needs?.joy?.CurLevelPercentage ?? -1f,
          recipientBeautyPercentage = recipient?.needs?.beauty?.CurLevelPercentage ?? -1f,
          recipientDrugsDesirePercentage = recipient?.needs?.drugsDesire?.CurLevelPercentage ?? -1f,
          recipientEnergyPercentage = recipient?.needs?.energy?.CurLevelPercentage ?? -1f
        };

        string dialogueDataJson = JsonUtility.ToJson(dialogueData);

        WWWForm form = new WWWForm();
        form.AddField("dialogueDataJSON", dialogueDataJson);

        using (UnityWebRequest request = UnityWebRequest.Post("http://44.202.96.70/home/GetDialogue", form))
        {
          var asyncOperation =  request.SendWebRequest();

          while (!asyncOperation.isDone)
          {
            await Task.Yield(); // Yield control back to the main thread
          }

          if (request.isNetworkError || request.isHttpError)
          {
            throw new Exception($"Network error: {request.error}");
          }
          else
          {
            while(!request.downloadHandler.isDone) { await Task.Yield(); }

            var body = request.downloadHandler.text;

            var dialogueResponse = JsonUtility.FromJson<DialogueResponse>(body);

            Mod.Log(dialogueResponse.text ?? "NULL");

            if (!Dictionary.ContainsKey(initiator))
              Dictionary[initiator] = new List<Bubble>();

            Dictionary[initiator].Add(new Bubble(initiator, entry, dialogueResponse.text ?? "NULL"));
          }
        }
      }
      catch (Exception ex)
      {
        Mod.Error($"A http post failed with error: [{ex.Source}: {ex.Message}]\n\nTrace:\n{ex.StackTrace}");
      }
    }

    private static void Remove(Pawn pawn, Bubble bubble)
    {
      if (Dictionary.ContainsKey(pawn) && Dictionary[pawn] != null)
        Dictionary[pawn].Remove(bubble);
      //if (Dictionary[pawn]!.Count is 0) { Dictionary.Remove(pawn); }
    }

    public static void Draw()
    {
      var altitude = GetAltitude();
      if (altitude <= 0 || altitude > Settings.AltitudeMax.Value) { return; }

      var scale = Settings.AltitudeBase.Value / altitude;
      if (scale > Settings.ScaleMax.Value) { scale = Settings.ScaleMax.Value; }

      var selected = Find.Selector!.SingleSelectedObject as Pawn;

      foreach (var pawn in Dictionary.Keys.OrderBy(pawn => pawn == selected).ThenBy(static pawn => pawn.Position.y).ToArray()) { DrawBubble(pawn, pawn == selected, scale); }
    }

    private static void DrawBubble(Pawn pawn, bool isSelected, float scale)
    {
      if (WorldRendererUtility.WorldRenderedNow || !pawn.Spawned || pawn.Map != Find.CurrentMap || pawn.Map!.fogGrid!.IsFogged(pawn.Position)) { return; }

      var pos = GenMapUI.LabelDrawPosFor(pawn, LabelPositionOffset);

      var offset = Settings.OffsetStart.Value;
      var count = 0;

      foreach (var bubble in Dictionary[pawn].OrderByDescending(static bubble => bubble.Entry.Tick).ToArray())
      {
        if (count > Settings.PawnMax.Value) { return; }
        if (!bubble.Draw(pos + GetOffset(offset), isSelected, scale)) { Remove(pawn, bubble); }
        offset += (Settings.OffsetDirection.Value.IsHorizontal ? bubble.Width : bubble.Height) + Settings.OffsetSpacing.Value;
        count++;
      }
    }

    private static float GetAltitude()
    {
      var altitude = Mathf.Max(1f, (float)Reflection.Verse_CameraDriver_RootSize.GetValue(Find.CameraDriver));
      Compatibility.Apply(ref altitude);

      return altitude;
    }

    private static Vector2 GetOffset(float offset)
    {
      var direction = Settings.OffsetDirection.Value.AsVector2;
      return new Vector2(offset * direction.x, offset * direction.y);
    }

    public static void Rebuild() => Dictionary.Values.Do(static list => list.Do(static bubble => bubble.Rebuild()));

    public static void Clear() => Dictionary.Clear();
  }


}
