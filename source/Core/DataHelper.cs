#nullable enable

using RimDialogue.Access;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core
{
  public static partial class H
  {
    private static readonly Regex ColorTag = new("<\\/?color[^>]*>");
    private static readonly Regex WhiteSpace = new("\\s+");

    public static GameComponent_ConversationTracker GetTracker()
    {
      var tracker = Current.Game.GetComponent<GameComponent_ConversationTracker>();
      Mod.LogV($"Tracker fetched.");
      return tracker;
    }

    public static PawnData MakePawnData(Pawn pawn, string? instructions)
    {
      string personality = string.Empty;
      string personalityDescription = string.Empty;

      if (Reflection.IsAssemblyLoaded("SP_Module1"))
      {
        try
        {
          if (Settings.VerboseLogging.Value)
            Mod.Log($"Trying to get personality.");
          Reflection.GetPersonality(pawn, out personality, out personalityDescription);
        }
        catch (Exception ex)
        {
          Mod.Warning($"Failed to get personality: {ex.Message}");
        }
      }

      return new PawnData
      {
        ThingID = pawn.ThingID,
        Instructions = instructions,
        FullName = pawn.Name?.ToStringFull ?? String.Empty,
        Personality = personality ?? string.Empty,
        PersonalityDescription = H.RemoveWhiteSpaceAndColor(personalityDescription),
        NickName = pawn.Name?.ToStringShort ?? pawn.Label ?? String.Empty,
        Gender = pawn.GetGenderLabel(),
        FactionName = pawn.Faction?.Name ?? String.Empty,
        FactionLabel = H.RemoveWhiteSpace(pawn.Faction?.def?.LabelCap),
        FactionDescription = H.RemoveWhiteSpace(pawn.Faction?.def?.description),
        Description = H.RemoveWhiteSpace(pawn.DescriptionDetailed),
        Race = pawn.def?.defName ?? String.Empty,
        IsColonist = pawn.IsColonist,
        IsPrisoner = pawn.IsPrisoner,
        IsHostile = pawn.HostileTo(Faction.OfPlayer),
        RoyaltyTitle = pawn.royalty?.MostSeniorTitle?.Label ?? string.Empty,
        IsCreepJoiner = pawn.IsCreepJoiner,
        IsGhoul = pawn.IsGhoul,
        IsBloodFeeder = pawn.IsBloodfeeder(),
        IsSlave = pawn.IsSlave,
        IsAnimal = pawn.IsNonMutantAnimal,
        IdeologyName = pawn.Ideo?.name ?? string.Empty,
        IdeologyDescription = H.RemoveWhiteSpaceAndColor(pawn.Ideo?.description),
        IdeologyPrecepts = pawn.Ideo?.PreceptsListForReading?.Where(precept => precept.GetType() == typeof(Precept)).Select(precept => precept.Label + " - " + H.RemoveWhiteSpace(precept.Description)).ToArray() ?? [],
        Age = pawn.ageTracker?.AgeBiologicalYears ?? -1,
        Childhood = pawn.story?.Childhood?.title != null ? pawn.story?.Childhood?.title + " - " + H.RemoveWhiteSpaceAndColor(H.GetBackstory(pawn, pawn.story?.Childhood)) : string.Empty,
        Adulthood = pawn.story?.Adulthood?.title != null ? pawn.story?.Adulthood?.title + " - " + H.RemoveWhiteSpaceAndColor(H.GetBackstory(pawn, pawn.story?.Adulthood)) : string.Empty,
        MoodString = pawn.needs?.mood?.MoodString ?? string.Empty,
      };
    }



    public static string UncapitalizeFirst(this TaggedString input)
    {
      if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
        return input;

      return char.ToLower(input[0]) + input.RawText.Substring(1);
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

    public static string GetBackstory(Pawn? pawn, BackstoryDef? backstory)
    {
      if (pawn == null || backstory == null)
        return string.Empty;

      return backstory.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
    }
    public static string GetHediffString(Hediff hediff)
    {
      string text = hediff.Label;
      if (hediff.Part != null)
        text = text + " in their " + hediff.Part.Label;
      return text;
    }

    public static IEnumerable<Battle> GetRecentBattles(int hours)
    {
      return Find.BattleLog.Battles
        .Where(battle => battle.CreationTimestamp >= Find.TickManager.TicksAbs - hours * 2500)
        .OrderBy(battle => battle.CreationTimestamp);
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


  }
}
