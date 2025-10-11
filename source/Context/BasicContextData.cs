#nullable enable
using RimDialogue.Access;
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Context
{
  public class BasicContextData : ContextData
  {
    public static BasicContextData[] CreateBackstory(Pawn pawn)
    {
      if (pawn.story == null || !pawn.IsColonist)
        return [];
      var contexts = new List<BasicContextData>();
      if (pawn.story.Childhood != null)
      {
        var backstoryContext = new BasicContextData(
          $"{pawn.Name?.ToStringShort ?? pawn.Label} has the '{pawn.story.Childhood.title}' childhood backstory. {H.RemoveWhiteSpaceAndColor(H.GetBackstory(pawn, pawn.story?.Childhood))}",
          "childhood_backstory",
          1f,
          pawn);
        if (backstoryContext != null)
          contexts.Add(backstoryContext);
      }
      if (pawn.story != null && pawn.story.Adulthood != null)
      {
        var backstoryContext = new BasicContextData(
          $"{pawn.Name?.ToStringShort ?? pawn.Label} has the '{pawn.story.Adulthood.title}' adulthood backstory. {H.RemoveWhiteSpaceAndColor(H.GetBackstory(pawn, pawn.story.Adulthood))}",
          "adulthood_backstory",
          1f,
          pawn);
        if (backstoryContext != null)
          contexts.Add(backstoryContext);
      }
      return contexts.ToArray();
    }

    public static BasicContextData[] CreateRelations(Pawn pawn)
    {
      if (pawn.relations?.DirectRelations == null
          || !pawn.relations.DirectRelations.Any()
          || !pawn.IsColonist)
        return [];

      return pawn.relations.DirectRelations
        .Where(r => r.otherPawn != null && r.otherPawn.IsColonist)
        .Select(r => new BasicContextData(
          $"{pawn.Name?.ToStringShort ?? pawn.Label} is {r.def.GetGenderSpecificLabel(pawn)} of {r.otherPawn?.Name?.ToStringShort ?? r.otherPawn?.Label}.",
          "relation",
          1f,
          pawn))
        .ToArray();
    }
    public static BasicContextData[] CreatePlants(Map map)
    {
      if (map == null || map.listerThings?.AllThings == null || !map.listerThings.AllThings.Any())
        return [];
        
      return map.listerThings.AllThings
        .Where(thing => thing.def.category == ThingCategory.Plant)
        .GroupBy(plant => plant.def.label)
        .Select(group => new BasicContextData(
          $"There {H.DescribeCount(group.Count())} {group.Key} growing in the area. {H.RemoveWhiteSpaceAndColor(group.First().def.description)}",
          "plant",
          1f))
        .ToArray();
    }

    public static BasicContextData[] CreateHistoricalQuests()
    {
      var quests = Reflection.RimWorld_QuestManager_HistoricalQuests.GetValue(Find.QuestManager) as List<Quest>;
      return quests
        .Where(quest => !quest.hidden && (quest.State == QuestState.EndedSuccess || quest.State == QuestState.EndedFailed))
        .Select(quest => new BasicContextData(
          $"The quest '{quest.name}' was {GetQuestState(quest)} {quest.TicksSinceAccepted.ToStringTicksToPeriod()} ago. {H.RemoveWhiteSpaceAndColor(quest.description)}",
          "historical_quest",
          1f))
        .ToArray();
    }

    private static string GetQuestState(Quest quest)
    {
      return quest.State switch
      {
        QuestState.EndedSuccess => "completed",
        QuestState.EndedFailed => "failed",
        _ => "unknown",
      };
    }


    public BasicContextData(
      string text,
      string type,
      float weight,
      params Pawn[] pawns) : base(type, weight, pawns)
    {
      Text = H.RemoveWhiteSpaceAndColor(text);
    }
    public override string Text { get; protected set; }

  }

  public class ExpiringContextData : BasicContextData, IExpirable
  {
    //rooms
    //Find.CurrentMap.regionAndRoomUpdater.roomLookup

    public static ExpiringContextData[]? CurrentRooms = null;

    public static ExpiringContextData[] CreateRooms(Map map)
    {
      if (map == null || map.regionAndRoomUpdater?.roomLookup == null || !map.regionAndRoomUpdater.roomLookup.Any())
        return [];
      foreach (var roomContext in CurrentRooms ?? [])
      {
        roomContext.Expire();
      }
      CurrentRooms = map.regionAndRoomUpdater.roomLookup.Values
        .Where(room => room != null && room.Role != RoomRoleDefOf.None)
        .GroupBy(room => room.Role.label)
        .Select(group => new ExpiringContextData(
          $"There are { H.DescribeCount(group.Count())} {group.Key} in the colony that contain: " +
            group.SelectMany(room => room.ContainedAndAdjacentThings)
              .Where(thing => !thing.def.IsBlueprint)
              .GroupBy(thing => thing.LabelShort)
              .Select(thingGroup => $"{ H.DescribeCount(thingGroup.Count(), false) } {thingGroup.Key}")
              .Aggregate((a, b) => a + ", " + b) + ".",
          "room",
          1f))
        .ToArray();
      return CurrentRooms;
    }

    public static ExpiringContextData[]? CurrentWildlife = null;
    public static ExpiringContextData[] CreateWildlife(Map map)
    {
      if (map == null || map.mapPawns?.AllPawns == null || !map.mapPawns.AllPawns.Any())
        return [];
      if (CurrentWildlife != null)
      {
        foreach (var context in CurrentWildlife)
        {
          context.Expire();
        }
      }
      CurrentWildlife = map.mapPawns.AllPawns
        .Where(pawn => pawn.IsAnimal && !pawn.IsColonyAnimal)
        .GroupBy(pawn => pawn.def.label)
        .Select(group => new ExpiringContextData(
          $"There { H.DescribeCount(group.Count())} wild {group.Key} in the nearby area. {H.RemoveWhiteSpaceAndColor(group.First().DescriptionDetailed)}",
          "wildlife",
          1f))
        .ToArray();
      return CurrentWildlife;
    }

    public static ExpiringContextData[]? CurrentAnimals = null;
    public static ExpiringContextData[] CreateAnimals(Map map)
    {
      if (map == null || map.mapPawns?.AllPawns == null || !map.mapPawns.AllPawns.Any())
        return [];
      foreach (var context in CurrentAnimals ?? [])
      {
        context.Expire();
      }
      CurrentAnimals = map.mapPawns.AllPawns
        .Where(pawn => pawn.IsColonyAnimal)
        .GroupBy(pawn => pawn.def.label)
        .Select(group => new ExpiringContextData(
          $"There { H.DescribeCount(group.Count())} tame {group.Key} in the colony. {H.RemoveWhiteSpaceAndColor(group.First().DescriptionDetailed)}",
          "animal",
          1f))
        .ToArray();
      return CurrentAnimals;
    }

    public event Action<IContext>? OnExpired;

    public ExpiringContextData(
      string text,
      string type,
      float weight,
      params Pawn[] pawns)
      : base(text, type, weight, pawns)
    {
      IsExpired = false;
    }

    public void Expire()
    {
      //if (Settings.VerboseLogging.Value) Mod.Log($"Expiring '{this.Text}'.");
      IsExpired = true;
      OnExpired?.Invoke(this);
    }

    public bool IsExpired
    {
      get;
      set;
    }
  }
}
