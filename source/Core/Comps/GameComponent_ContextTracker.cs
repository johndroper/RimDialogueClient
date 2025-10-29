#if !RW_1_5
#nullable enable
using RimDialogue.Access;
using RimDialogue.Context;
using RimDialogue.Core.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Verse;
using static RimDialogue.MainTabWindow_RimDialogue;

namespace RimDialogue.Core
{
  public class GameComponent_ContextTracker : GameComponent
  {
    public static GameComponent_ContextTracker? Instance => Current.Game.GetComponent<GameComponent_ContextTracker>();

    private ContextDb<ContextData> contextDb = new("basic", null);
    private TemporalContextCatalog temporalContextCatalog = new();

    public List<string> Canon = [];

    public GameComponent_ContextTracker(Game game) : base()
    {
      Mod.Log("GameComponent_ContextTracker initialized.");
    }

    public async void Add(TemporalContextData contextData)
    {
      if (!Settings.EnableContext.Value)
        return;
      await temporalContextCatalog.Add(contextData);
    }
    public async void Add(ContextData contextData)
    {
      if (!Settings.EnableContext.Value)
        return;
      await contextDb.Add(contextData);
    }

    public async Task<TemporalContextData[]> GetTemporalContext(Pawn initiator, Pawn recipient, string query, int maxItems)
    {
      if (!Settings.EnableContext.Value)
        return [];
      return await temporalContextCatalog.BlendedSearch(initiator, recipient, query, maxItems);
    }

    public async Task<TemporalContextData[]> GetTemporalContext(Pawn initiator, string query, int maxItems)
    {
      if (!Settings.EnableContext.Value)
        return [];
      return await temporalContextCatalog.BlendedSearch(initiator, query, maxItems);
    }

    public async Task<ContextData[]> GetContext(string query, int maxItems)
    {
      if (!Settings.EnableContext.Value)
        return [];
      var results = await contextDb
        .Search(query, maxItems);
      return results.Where(r => r != null).ToArray()!;
    }

    public override void StartedNewGame()
    {
      base.StartedNewGame();

      if (!Settings.EnableContext.Value)
        return;

      contextDb = new("basic", null);
      temporalContextCatalog.Clear();
      LoadBasicContext();
    }

    public void LoadBasicContext()
    {
      List<ContextData> contextDatae = [];

      if (Canon != null)
        contextDatae.AddRange(Canon.Select(canonItem => new BasicContextData(canonItem, "canon", 1f)));

      foreach (Pawn pawn in Find.CurrentMap?.mapPawns.FreeColonists ?? [])
      {
        contextDatae.AddRange(BasicContextData.CreateRelations(pawn));
        contextDatae.AddRange(DynamicContextData.CreateSkillContexts(pawn));
        contextDatae.AddRange(DynamicContextData.CreateTraitContexts(pawn));
        contextDatae.AddRange(DynamicContextData.CreateApparelContexts(pawn));
        contextDatae.AddRange(DynamicContextData.CreateIdeo(pawn));
        contextDatae.AddRange(BasicContextData.CreateBackstory(pawn));
        contextDatae.AddRange(DynamicContextData.CreateHediffsContexts(pawn));
        contextDatae.AddRange(DynamicContextData.CreateEquipmentContexts(pawn));
        contextDatae.AddRange(DynamicContextData.CreateAppearance(pawn));
        contextDatae.AddRange(BasicContextData.CreateHistoricalQuests());
        if (Find.CurrentMap != null)
        {
          contextDatae.AddRange(ExpiringContextData.CreateAnimals(Find.CurrentMap));
          contextDatae.AddRange(ExpiringContextData.CreateRooms(Find.CurrentMap));
          contextDatae.AddRange(ExpiringContextData.CreateWildlife(Find.CurrentMap));
          contextDatae.AddRange(BasicContextData.CreatePlants(Find.CurrentMap));
        }
      }
      contextDatae.AddRange(DynamicContextData.CreateFactions());

#if DEBUG
      try
      {
        string logPath = Path.Combine(GenFilePaths.SaveDataFolderPath, "Logs", "context_data.log");
        Mod.Log($"Writing context debug data to {logPath}.");
        using (StreamWriter writer = new StreamWriter(logPath, false))
        {
          writer.WriteLine($"Context Data Log - {DateTime.Now}");
          writer.WriteLine($"Total items: {contextDatae.Count}");
          writer.WriteLine("-------------------------------------------");

          foreach (var item in contextDatae)
          {
            writer.WriteLine($"{item.Type}\t{item.Weight}\t{item.Text}");
          }
        }
        Mod.Log($"Wrote temporal context debug data to {logPath}");
      }
      catch (Exception ex)
      {
        Mod.Error($"Error writing context debug data: {ex}");
      }
#endif

      LoadAsync(contextDatae);

      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded {contextDatae.Count} basic context items.");
    }

    public override void FinalizeInit()
    {
      base.FinalizeInit();
      lastCleanupTick = Find.TickManager.TicksGame + GenDate.TicksPerHour;
      lastContextRefresh = Find.TickManager.TicksGame + GenDate.TicksPerHour * 8;
    }

    public async void LoadTemporalAsync(IEnumerable<TemporalContextData> contextDatae)
    {
      try
      {
        await Task.WhenAll(contextDatae.Select(temporalContextCatalog.Add));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error during ContextTracker.LoadAsync: {ex}");
      }
    }

    public async void LoadAsync(IEnumerable<ContextData> contextDatae)
    {
      try
      {
        await Task.WhenAll(contextDatae.Select(contextDb.Add));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error during ContextTracker.LoadAsync: {ex}");
      }
    }

    public void LoadTemporalContent()
    {
      var watch = new Stopwatch();
      watch.Start();
      List<TemporalContextData> temporalContextDatae = [];
      try
      {
        var playlogEntries = Find.PlayLog.AllEntries
          .OrderByDescending(entry => entry.Tick)
          .Take(1000)
          .ToArray();
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{playlogEntries.Length} play log entries to be loaded into context.");
        temporalContextDatae.AddRange(
          playlogEntries
            .Select(TemporalContextCatalog.Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading PlayLog entries: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded PlayLog in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      var battles = Find.BattleLog.Battles;
      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{battles.Count} battles to be loaded into context.");
        temporalContextDatae.AddRange(
          Find.BattleLog.Battles
            .Select(TemporalContextCatalog.Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading battle log entries: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded BattleLog in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      try
      {
        var battleEntries = battles
            .SelectMany(battle => battle.Entries)
            .OrderByDescending(entry => entry.Tick)
            .Take(1000)
            .ToArray();
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{battleEntries.Length} battle log entries to be loaded into context.");
        temporalContextDatae.AddRange(
          battleEntries
            .Select(TemporalContextCatalog.Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading BattleLog entries: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded BattleLog entries in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{GameComponent_ConversationTracker.Instance.Conversations.Count} conversations to be loaded into context.");
        temporalContextDatae.AddRange(
          GameComponent_ConversationTracker.Instance.Conversations
            .Select(TemporalContextCatalog.Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading Conversations: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded Conversations in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{GameComponent_LetterTracker.Instance.RecentLetters.Count} letters to be loaded into context.");
        temporalContextDatae.AddRange(
          GameComponent_LetterTracker.Instance.RecentLetters
            .Select(letter => new TemporalContextData(
              letter.Label + " - " + letter.Text,
              "letter",
              letter.Ticks,
              5f)));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading Letters: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded Letters in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      try
      {
        var messages = GameComponent_MessageTracker.Instance.TrackedMessages;
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{messages?.Count ?? 0} messages to be loaded into context.");
        temporalContextDatae.AddRange(
          messages
            .Select(message => new TemporalContextData(
              message.MessageText ?? string.Empty,
              "message",
              message.Ticks,
              1f)));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading Messages: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded Messages in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      foreach (var pawn in Find.CurrentMap?.mapPawns.FreeColonists ?? [])
      {
        try
        {
          if (pawn != null && pawn.health != null && pawn.health?.hediffSet != null)
          {
            var hediffs = pawn.health?.hediffSet.hediffs
              .Select(TemporalContextCatalog.Create)
              .Where(cd => cd != null)
              .ToArray();
            if (Settings.VerboseLogging.Value)
              Mod.Log($"{hediffs?.Length ?? 0} hediffs to be loaded into context for {pawn}.");
            if (hediffs != null)
              temporalContextDatae.AddRange(hediffs!);
          }
        }
        catch (Exception ex)
        {
          Mod.Error($"Error loading Comp_PawnContext for {pawn.Name}: {ex}");
        }
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded Hediffs in {watch.Elapsed.TotalSeconds} seconds.");

      //watch.Restart();
      //try
      //{
      //  var jobContext = GameComponent_JobTracker.Instance.JobRecords
      //      .SelectMany(keyPair => keyPair.Value)
      //      .Select(record => TemporalContextCatalog.Create(record))
      //      .Where(context => context != null)
      //      .ToArray();
      //  if (Settings.VerboseLogging.Value)
      //    Mod.Log($"{jobContext.Length} jobs to be loaded into context.");
      //  temporalContextDatae.AddRange(jobContext!);
      //}
      //catch (Exception ex)
      //{
      //  Mod.Error($"Error loading JobRecords: {ex}");
      //}
      //watch.Stop();
      //if (Settings.VerboseLogging.Value)
      //  Mod.Log($"Loaded JobRecords in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
      try
      {
        temporalContextDatae.AddRange(
          GameComponent_PawnDeathTracker.Instance.DeadColonists
            .Select(record => new TemporalContextData(
              record.ToString(),
              "death",
              record.TimeStamp,
              1f)));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading DeathRecords: {ex}");
      }
      watch.Stop();
      if (Settings.VerboseLogging.Value)
        Mod.Log($"Loaded DeathRecords in {watch.Elapsed.TotalSeconds} seconds.");

      watch.Restart();
#if DEBUG
      try
      {
        string logPath = Path.Combine(GenFilePaths.SaveDataFolderPath, "Logs", "temporal_context_data.log");
        Mod.Log($"Writing temporal context debug data to {logPath}.");
        using (StreamWriter writer = new StreamWriter(logPath, false))
        {
          writer.WriteLine($"Temporal Context Data Log - {DateTime.Now}");
          writer.WriteLine($"Total items: {temporalContextDatae.Count}");
          writer.WriteLine("-------------------------------------------");
            
          foreach (var item in temporalContextDatae)
          {
            writer.WriteLine($"{item.Type}\t{item.Tick}\t{item.Weight}\t{item.Text}");
          }
        }
        Mod.Log($"Wrote temporal context debug data to {logPath}");
      }
      catch (Exception ex)
      {
        Mod.Error($"Error writing context debug data: {ex}");
      }
#endif
      LoadTemporalAsync(temporalContextDatae);
      watch.Stop();

      if (Settings.VerboseLogging.Value)
        Mod.Log($"LoadTemporalAsync loaded {temporalContextDatae.Count} records in {watch.Elapsed.TotalSeconds} seconds.");
    }

    public override void LoadedGame()
    {
      base.LoadedGame();

      if (!Settings.EnableContext.Value)
        return;

      LoadBasicContext();
      LoadTemporalContent();
    }

    public async void CleanUp()
    {
      await Task.Run(() =>
      {
        contextDb.Cleanup(1000);
        temporalContextCatalog.CleanUp();
      });
    }

    public int lastCleanupTick = 0;
    public int lastContextRefresh = 0;
    public override void GameComponentUpdate()
    {
      try
      {
        if (Find.TickManager.TicksGame > lastCleanupTick + GenDate.TicksPerDay)
        {
          lastCleanupTick = Find.TickManager.TicksGame;
          CleanUp();
        }
          
      }
      catch (Exception ex)
      {
        Log.ErrorOnce($"Error during ContextTracker.GameComponentUpdate: {ex}", 348975432);
      }

      if (Find.TickManager.TicksGame > lastContextRefresh + (GenDate.TicksPerDay * 0.5))
      {
        if (Settings.VerboseLogging.Value) Mod.Log("Refreshing expiring context.");
        lastContextRefresh = Find.TickManager.TicksGame;
        LoadAsync(ExpiringContextData.CreateRooms(Find.CurrentMap));
        LoadAsync(ExpiringContextData.CreateWildlife(Find.CurrentMap));
        LoadAsync(ExpiringContextData.CreateAnimals(Find.CurrentMap));

        if (Scribe.mode == LoadSaveMode.LoadingVars)
          Canon = [];
      }
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Collections.Look(ref Canon, "canon", LookMode.Value);
    }

  }
}
#endif
