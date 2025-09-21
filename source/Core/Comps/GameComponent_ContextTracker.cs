#if !RW_1_5
#nullable enable
using Google.Protobuf.Compiler;
using RimDialogue.Access;
using RimDialogue.Context;
using RimDialogue.Core.Comps;
using RimDialogue.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;
using static RimDialogue.Core.GameComponent_LetterTracker;
using static RimWorld.ColonistBar;

namespace RimDialogue.Core
{
  public class GameComponent_ContextTracker : GameComponent
  {
    public static GameComponent_ContextTracker? Instance => Current.Game.GetComponent<GameComponent_ContextTracker>();

    private ContextDb globalDb = new ContextDb();
    private Dictionary<Pawn, ContextDb> pawnDb = [];
    private ReaderWriterLockSlim _pawnDbLock = new();

    public GameComponent_ContextTracker(Game game) : base()
    {
      Mod.Log("GameComponent_ContextTracker initialized.");
    }

    public static ContextData? Create(Hediff hediff)
    {
      if (hediff == null || hediff.pawn == null || !hediff.pawn.IsColonist)
        return null;

      return new ContextData(
        $"{hediff.pawn} received {hediff.GetTooltip(hediff.pawn, false)}",
        hediff.def.defName,
        hediff.tickAdded,
        Settings.HediffContextWeight.Value,
        hediff.pawn);
    }

    public static ContextData? Create(Battle battle)
    {
      if (battle == null)
        return null;

      var participants = battle.Entries
        .SelectMany(entry => entry.GetConcerns())
        .Distinct()
        .ToArray();

      if (!participants.Any())
        return null;

      return new ContextData(
        $"The battle '{battle.GetName()}' occurred with {
            String.Join(", ",
              participants
                .Take(25)
                .Select(pawn => $"{pawn.LabelShort} ({pawn.Faction?.Name})")
                .ToArray())}",
        "battle",
        battle.CreationTimestamp,
        Math.Min(battle.Importance, Settings.BattleContextWeight.Value));
    }

    public static ContextData? Create(Message message)
    {
      if (message == null || string.IsNullOrWhiteSpace(message.text))
        return null;
      return new ContextData(
        message.text,
        message.def.defName,
        message.startingTick,
        Settings.MessageContextWeight.Value);
    }

    public static ContextData? Create(RecentLetter letter)
    {
      if (letter == null || letter.Text == null || letter.Type == null)
        return null;

      float weight = 5f;
      if (letter.Type == LetterDefOf.ThreatBig.defName)
        weight = Settings.ThreatBigLetterContextWeight.Value;
      else if (letter.Type == LetterDefOf.NegativeEvent.defName)
        weight = Settings.NegativeLetterContextWeight.Value;
      else if (letter.Type == LetterDefOf.PositiveEvent.defName)
        weight = Settings.PositiveLetterContextWeight.Value;
      else if (letter.Type == LetterDefOf.NeutralEvent.defName)
        weight = Settings.NeutralLetterContextWeight.Value;

      return new ContextData(
        letter.Text,
        letter.Type,
        letter.Ticks,
        weight);
    }

    public static ContextData? Create(JobRecord jobRecord)
    {
      return new ContextData(
        $"{jobRecord.Pawn.Name?.ToStringShort ?? jobRecord.Pawn.Label} was {jobRecord.Report} for {jobRecord.Duration.ToStringTicksToPeriod()}.",
        "job",
        jobRecord.StartTick,
        Settings.JobContextWeight.Value);
    }


    public static ContextData? Create(LogEntry entry)
    {
      if (entry == null)
        return null;

      if (entry is PlayLogEntry_InteractionWithMany interactionMany)
      {
        Pawn? initiator = Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interactionMany) as Pawn;
        if (initiator == null)
          throw new Exception("initiator is null in PlayLogEntry_InteractionWithMany.");
        if (!initiator.IsColonist)
          return null;
        var recipients = Reflection.Verse_PlayLogEntry_InteractionWithMany_Recipients.GetValue(interactionMany) as List<Pawn>;
        List<Pawn> involvedPawns = new List<Pawn>();
        involvedPawns = [initiator];
        if (recipients != null)
          involvedPawns.AddRange(recipients);
        else
          throw new Exception("recipients is null in PlayLogEntry_InteractionWithMany.");
        var text = entry.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          interactionMany.GetType().Name,
          entry.Tick,
          Settings.LogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else if (entry is PlayLogEntry_InteractionSinglePawn singlePawn)
      {
        var initiator = (Pawn)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(singlePawn);
        if (initiator == null)
          throw new Exception("initiator is null in PlayLogEntry_InteractionSinglePawn.");
        if (!initiator.IsColonist)
          return null;
        List<Pawn> involvedPawns = new List<Pawn>();
        involvedPawns.Add(initiator);
        var text = singlePawn.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          singlePawn.GetType().Name,
          entry.Tick,
          Settings.LogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else if (entry is PlayLogEntry_Interaction interaction)
      {
        if (Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interaction) is not Pawn initiator)
          throw new Exception("initiator is null.");
        if (!initiator.IsColonist)
          return null;
        Pawn? recipient = Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(interaction) as Pawn;
        List<Pawn> involvedPawns = new List<Pawn>();
        involvedPawns.Add(initiator);
        if (recipient != null)
          involvedPawns.Add(recipient);
        var text = entry.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          interaction.GetType().Name,
          entry.Tick,
          Settings.LogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else if (entry is BattleLogEntry_DamageTaken damageTaken)
      {
        var initiator = (Pawn)Reflection.Verse_BattleLogEntry_DamageTaken_InitiatorPawn.GetValue(damageTaken);
        if (initiator == null || !initiator.IsColonist)
          return null;
        var recipient = (Pawn)Reflection.Verse_BattleLogEntry_DamageTaken_RecipientPawn.GetValue(damageTaken);
        List<Pawn> involvedPawns = new List<Pawn>();
        if (initiator != null)
          involvedPawns.Add(initiator);
        if (recipient != null)
          involvedPawns.Add(recipient);
        var text = damageTaken.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          damageTaken.GetType().Name,
          entry.Tick,
          Settings.BattleLogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else if (entry is BattleLogEntry_MeleeCombat meleeCombat)
      {
        var initiator = (Pawn)Reflection.Verse_BattleLogEntry_MeleeCombat_Initiator.GetValue(meleeCombat);
        if (initiator == null || !initiator.IsColonist)
          return null;
        var recipient = (Pawn)Reflection.Verse_BattleLogEntry_MeleeCombat_RecipientPawn.GetValue(meleeCombat);
        List<Pawn> involvedPawns = new List<Pawn>();
        involvedPawns.Add(initiator);
        if (recipient != null)
          involvedPawns.Add(recipient);
        var text = meleeCombat.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          meleeCombat.GetType().Name,
          entry.Tick,
          Settings.BattleLogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else if (entry is BattleLogEntry_RangedImpact rangedImpact)
      {
        var initiator = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_InitiatorPawn.GetValue(rangedImpact);
        if (initiator == null || !initiator.IsColonist)
          return null;
        var recipient = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_RecipientPawn.GetValue(rangedImpact);
        var target = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_OriginalTargetPawn.GetValue(rangedImpact);
        List<Pawn> involvedPawns = new List<Pawn>();
        involvedPawns.Add(initiator);
        if (recipient != null)
          involvedPawns.Add(recipient);
        if (target != null)
          involvedPawns.Add(target);
        var text = rangedImpact.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          rangedImpact.GetType().Name,
          entry.Tick,
          Settings.BattleLogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else if (entry is BattleLogEntry_RangedFire rangedFire)
      {
        List<Pawn> involvedPawns = new List<Pawn>();
        var initiator = (Pawn)Reflection.Verse_BattleLogEntry_RangedFire_InitiatorPawn.GetValue(rangedFire);
        if (initiator == null)
          return null;
        var recipient = (Pawn)Reflection.Verse_BattleLogEntry_RangedFire_RecipientPawn.GetValue(rangedFire);
        involvedPawns.Add(initiator);
        if (recipient != null)
          involvedPawns.Add(recipient);
        var text = rangedFire.ToGameStringFromPOV(initiator);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          rangedFire.GetType().Name,
          entry.Tick,
          Settings.BattleLogEntryContextWeight.Value,
          involvedPawns.ToArray());
      }
      else
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Entry {entry.LogID} - Unknown Context Tracker log entry type: '{entry.GetType().Name}'");
        var text = entry.ToGameStringFromPOV(null);
        if (string.IsNullOrWhiteSpace(text))
          return null;
        return new ContextData(
          text,
          entry.GetType().ToString(),
          entry.Tick,
          Settings.LogEntryContextWeight.Value);
      }
    }

    public static ContextData? Create(Conversation conversation)
    {
      if (
        conversation == null ||
        conversation.Initiator == null ||
        !conversation.Initiator.IsColonist ||
        string.IsNullOrWhiteSpace(conversation.Text))
        return null;
      List<Pawn> involvedPawns = new List<Pawn>();
      if (conversation.Initiator != null)
        involvedPawns.Add(conversation.Initiator);
      if (conversation.Recipient != null)
        involvedPawns.Add(conversation.Recipient);
      return new ContextData(
        conversation.Text ?? "Missing Conversation text.",
        conversation.GetType().Name,
        conversation.Timestamp ?? 0,
        Settings.ConversationContextWeight.Value,
        involvedPawns.ToArray());
    }

    public override void StartedNewGame()
    {
      base.StartedNewGame();
      globalDb = new ContextDb();
      pawnDb.Clear();
    }


    public void Add(
      string text,
      string type,
      int tick,
      float weight)
    {
      Add(new ContextData(text, type, tick, weight));
    }

    public void Add(
      string text,
      string type,
      int tick,
      float weight,
      params Pawn[] pawns)
    {
      Add(new ContextData(text, type, tick, weight, pawns));
    }

    public void Add(JobRecord jobRecord)
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Job record context added '{jobRecord.ToString()}'");
      var contextData = Create(jobRecord);
      if (contextData != null)
        Add(contextData);
    }

    public void Add(Hediff hediff)
    {
      var contextData = Create(hediff);
      if (contextData != null)
        Add(contextData);
    }

    public void Add(Battle battle)
    {
      var contextData = Create(battle);
      if (contextData != null)
        Add(contextData);
    }

    public void Add(RecentLetter record)
    {
      var contextData = Create(record);
      if (contextData != null)
        Add(contextData);
    }

    public void Add(Conversation conversation)
    {
      var contextData = Create(conversation);
      if (contextData != null)
        Add(contextData);
    }

    public void Add(LogEntry entry)
    {
      var contextData = Create(entry);
      if (contextData != null)
        Add(contextData);
    }

    public async void Add(ContextData contextData)
    {
      if (Settings.MaxContextItems.Value <= 0)
        return;

      await AddAsync(contextData);
    }

    public async Task AddAsync(
      ContextData contextData)
    {
      var dbs = new List<ContextDb>(contextData.Pawns.Length);
      _pawnDbLock.EnterUpgradeableReadLock();
      try
      {
        if (!contextData.Pawns.Any())
        {
          dbs.Add(globalDb);
        }
        else
          foreach (var pawn in contextData.Pawns)
          {
            if (!pawnDb.TryGetValue(pawn, out var db))
            {
              _pawnDbLock.EnterWriteLock();
              try
              {
                if (!pawnDb.TryGetValue(pawn, out db))
                {
                  db = new ContextDb();
                  pawnDb[pawn] = db;
                }
              }
              finally { _pawnDbLock.ExitWriteLock(); }
            }
            dbs.Add(db);
          }
      }
      finally { _pawnDbLock.ExitUpgradeableReadLock(); }

      foreach(var db in dbs)
      {
        await db.Add(contextData);
      }
    }

    public async Task<ContextData?[]> BlendedSearch(
      Pawn initiator,
      Pawn recipient,
      string query,
      int maxResults = 10)
    {
      if (Settings.MaxContextItems.Value <= 0)
        return [];
      query = H.RemoveWhiteSpaceAndColor(query);
      var initiatorResults = await Search(initiator, query, maxResults);
      var recipientResults = await Search(recipient, query, maxResults);
      var globalResults = await Search(query, maxResults);
      return initiatorResults
        .Concat(recipientResults)
        .Concat(globalResults)
        .Where(r => r != null)
        .Distinct()
        .OrderBy(r => r!.Tick)
        .ToArray();
    }

    public async Task<ContextData?[]> BlendedSearch(
      Pawn initiator,
      string query,
      int maxResults = 10)
    {
      if (Settings.MaxContextItems.Value <= 0)
        return [];
      query = H.RemoveWhiteSpaceAndColor(query);
      var initiatorResults = await Search(initiator, query, maxResults);
      var globalResults = await Search(query, maxResults);
      return initiatorResults
        .Concat(globalResults)
        .Where(r => r != null)
        .Distinct()
        .OrderBy(r => r!.Tick)
        .ToArray();
    }

    public async Task<ContextData?[]> Search(
      Pawn pawn,
      string query,
      int maxResults = 10)
    {
      ContextDb db;
      _pawnDbLock.EnterReadLock();
      try
      {
        if (!pawnDb.TryGetValue(pawn, out db))
          return [];
      }
      finally { _pawnDbLock.ExitReadLock(); }
      return await db.Search(query, maxResults);
    }

    public async Task<ContextData?[]> Search(
      string query,
      int maxResults = 10)
    {
      return await globalDb.Search(query, maxResults);
    }

    public override void FinalizeInit()
    {
      base.FinalizeInit();

    }

    public async void LoadAsync(IEnumerable<ContextData> contextDatae)
    {
      try
      {
        await Task.WhenAll(contextDatae.Select(AddAsync));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error during ContextTracker.LoadAsync: {ex}");
      }
    }

    public override void LoadedGame()
    {
      base.LoadedGame();

      if (Settings.MaxContextItems.Value <= 0)
        return;

      List<ContextData> contextDatae = new List<ContextData>();
      try
      {
        var playlogEntries = Find.PlayLog.AllEntries;
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{playlogEntries.Count} play log entries to be loaded into context.");
        contextDatae.AddRange(
          playlogEntries
            .Select(Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading PlayLog entries: {ex}");
      }

      var battles = Find.BattleLog.Battles;

      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{battles.Count} battles to be loaded into context.");
        contextDatae.AddRange(
          Find.BattleLog.Battles
            .Select(Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading battle log entries: {ex}");
      }

      try
      {
        var battleEntries = battles
            .SelectMany(battle => battle.Entries)
            .ToArray();
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{battleEntries.Length} battle log entries to be loaded into context.");
        contextDatae.AddRange(
          battleEntries
            .Select(Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading BattleLog entries: {ex}");
      }

      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{GameComponent_ConversationTracker.Instance.Conversations.Count} conversations to be loaded into context.");
        contextDatae.AddRange(
          GameComponent_ConversationTracker.Instance.Conversations
            .Select(Create)
            .Where(cd => cd != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading Conversations: {ex}");
      }

      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{GameComponent_LetterTracker.Instance.RecentLetters.Count} letters to be loaded into context.");

        //foreach(var letter in GameComponent_LetterTracker.Instance.RecentLetters)
        //{
        //  Mod.Log($"Letter: {letter.Label} - {letter.Text} (Type: {letter.Type})");
        //}

        contextDatae.AddRange(
          GameComponent_LetterTracker.Instance.RecentLetters
            .Select(letter => new ContextData(
              letter.Label + " - " + letter.Text,
              "letter",
              letter.Ticks,
              5f)));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading Letters: {ex}");
      }

      try
      {
        var messages = GameComponent_MessageTracker.Instance.TrackedMessages;
        if (Settings.VerboseLogging.Value)
          Mod.Log($"{messages?.Count ?? 0} messages to be loaded into context.");
        contextDatae.AddRange(
          messages
            .Select(message => new ContextData(
              message.MessageText ?? string.Empty,
              "message",
              message.Ticks,
              1f)));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading Messages: {ex}");
      }
      
      foreach(var pawn in Find.CurrentMap?.mapPawns.FreeColonists ?? [])
      {
        try
        {
          if (pawn != null && pawn.health != null && pawn.health?.hediffSet != null)
          {
            var hediffs = pawn.health?.hediffSet.hediffs
              .Select(Create)
              .Where(cd => cd != null)
              .ToArray();
            if (Settings.VerboseLogging.Value)
              Mod.Log($"{hediffs?.Length ?? 0} hediffs to be loaded into context for {pawn}.");
            if (hediffs != null)
              contextDatae.AddRange(hediffs!);
          }
        }
        catch (Exception ex)
        {
          Mod.Error($"Error loading Comp_PawnContext for {pawn.Name}: {ex}");
        }
      }

      try
      {
        contextDatae.AddRange(
          GameComponent_JobTracker.Instance.JobRecords
            .SelectMany(keyPair => keyPair.Value)
            .Select(record => Create(record))
            .Where(context => context != null)!);
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading JobRecords: {ex}");
      }

      try
      {
        contextDatae.AddRange(
          GameComponent_PawnDeathTracker.Instance.DeadColonists
            .Select(record => new ContextData(
              record.ToString(),
              "death",
              record.TimeStamp,
              1f)));
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading DeathRecords: {ex}");
      }

      LoadAsync(contextDatae);
    }

    public async void CleanUp()
    {
      try
      {
        await Task.Run(() =>
        {
          globalDb.Cleanup(Settings.MaxContextItems.Value);
          _pawnDbLock.EnterReadLock();
          try
          {
            foreach (var keypair in pawnDb)
            {
              if (keypair.Key.IsColonist)
                keypair.Value.Cleanup(Settings.MaxContextItems.Value);
            }
          }
          finally { _pawnDbLock.ExitReadLock(); }
        });
      }
      catch (Exception ex)
      {
        Mod.Error($"Error during ContextTracker.Cleanup: {ex}");
      }
    }


    public override void GameComponentUpdate()
    {
      try
      {
        if (Find.TickManager.TicksGame % 25000 == 0)
          CleanUp();
      }
      catch (Exception ex)
      {
        Log.ErrorOnce($"Error during ContextTracker.GameComponentUpdate: {ex}", 348975432);
      }
    }

    //public override void ExposeData()
    //{
    //  base.ExposeData();
    //}

  }
}
#endif
