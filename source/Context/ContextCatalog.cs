#nullable enable
using RimDialogue.Access;
using RimDialogue.Core;
using RimDialogue.Core.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Context
{
  public class ContextCatalog<T> where T : ContextData
  {
    private ContextDb<T> globalDb = new("global", null);
    private Dictionary<Pawn, ContextDb<T>> pawnDbs = [];
    private ReaderWriterLockSlim pawnDbLock = new();

    public void Clear()
    {
      pawnDbLock.EnterWriteLock();
      try
      {
        globalDb = new ContextDb<T>("global_" + typeof(T).Name, null);
        pawnDbs.Clear();
      }
      finally { pawnDbLock.ExitWriteLock(); }
    }

    public async Task Add(
      T contextData)
    {
      var dbs = new List<ContextDb<T>>(contextData.Pawns.Length);
      pawnDbLock.EnterUpgradeableReadLock();
      try
      {
        if (!contextData.Pawns.Any())
        {
          dbs.Add(globalDb);
        }
        else
          foreach (var pawn in contextData.Pawns)
          {
            if (!pawnDbs.TryGetValue(pawn, out var db))
            {
              pawnDbLock.EnterWriteLock();
              try
              {
                if (!pawnDbs.TryGetValue(pawn, out db))
                {
                  var name = pawn.Name?.ToStringShort ?? pawn.Label;
                  db = new ContextDb<T>(name + "_" + typeof(T).Name, [name, $"{name}'s"]);
                  pawnDbs[pawn] = db;
                }
              }
              finally { pawnDbLock.ExitWriteLock(); }
            }
            dbs.Add(db);
          }
      }
      finally { pawnDbLock.ExitUpgradeableReadLock(); }
      foreach (var db in dbs)
      {
        await db.Add(contextData);
      }
    }

    public virtual async Task<T[]> BlendedSearch(
      Pawn initiator,
      Pawn recipient,
      string query,
      int maxResults = 10)
    {
      if (!Settings.EnableContext.Value)
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
        .ToArray()!;
    }

    public virtual async Task<T[]> BlendedSearch(
      Pawn initiator,
      string query,
      int maxResults = 10)
    {
      if (!Settings.EnableContext.Value)
        return [];
      query = H.RemoveWhiteSpaceAndColor(query);
      var initiatorResults = await Search(initiator, query, maxResults);
      var globalResults = await Search(query, maxResults);
      return initiatorResults
        .Concat(globalResults)
        .Where(r => r != null)
        .Distinct()
        .ToArray()!;
    }

    public async Task<T?[]> Search(
      Pawn pawn,
      string query,
      int maxResults = 10)
    {
      ContextDb<T> db;
      pawnDbLock.EnterReadLock();
      try
      {
        if (!pawnDbs.TryGetValue(pawn, out db))
          return [];
      }
      finally { pawnDbLock.ExitReadLock(); }
      return await db.Search(query, maxResults);
    }

    public async Task<T?[]> Search(
      string query,
      int maxResults = 10)
    {
      return await globalDb.Search(query, maxResults);
    }

    public async void CleanUp()
    {
      try
      {
        await Task.Run(() =>
        {
          globalDb.Cleanup(Settings.MaxTemporalContextItems.Value);
          pawnDbLock.EnterReadLock();
          try
          {
            foreach (var keypair in pawnDbs)
            {
              if (keypair.Key.IsColonist)
                keypair.Value.Cleanup(Settings.MaxTemporalContextItems.Value);
            }
          }
          finally { pawnDbLock.ExitReadLock(); }
        });
      }
      catch (Exception ex)
      {
        Mod.Error($"Error during ContextTracker.Cleanup: {ex}");
      }
    }
  }

  //public class InstantContextCatalog : ContextCatalog<DynamicContextData>
  //{
  //  public async void Add(
  //    string text,
  //    Func<string> valueFunc,
  //    string type,
  //    float weight)
  //  {
  //    await Add(new InstantContextData(text, valueFunc, type, weight));
  //  }

  //  public async void Add(
  //    string text,
  //    Func<string> valueFunc,
  //    string type,
  //    float weight,
  //    params Pawn[] pawns)
  //  {
  //    await Add(new InstantContextData(text, valueFunc, type, weight, pawns));
  //  }
  //}

  public class BasicContextCatalog : ContextCatalog<BasicContextData>
  {
    public async void Add(
      string text,
      string type,
      float weight)
    {
      await Add(new BasicContextData(text, type, weight));
    }
    public async void Add(
      string text,
      string type,
      float weight,
      params Pawn[] pawns)
    {
      await Add(new BasicContextData(text, type, weight, pawns));
    }
  }



  public class TemporalContextCatalog : ContextCatalog<TemporalContextData>
  {
    public static TemporalContextData? Create(Pawn deadPawn, DamageInfo? dinfo, Hediff? exactCulprit)
    {
      if (deadPawn == null || !deadPawn.IsColonist)
        return null;

      return new TemporalContextData(
        $"{deadPawn} died. {(dinfo != null ? $"Cause of death: {dinfo.Value.Def.defName}." : "")} {(exactCulprit != null ? $"Exact cause: {exactCulprit.LabelCap}." : "")}",
        "death",
        Find.TickManager.TicksAbs,
        Settings.HediffContextWeight.Value, //this is wrong
        deadPawn);
    }

    public static TemporalContextData? Create(Hediff hediff)
    {
      if (hediff == null || hediff.pawn == null || !hediff.pawn.IsColonist)
        return null;

      return new TemporalContextData(
        $"{hediff.pawn} received {hediff.GetTooltip(hediff.pawn, false)}",
        hediff.def.defName,
        hediff.tickAdded,
        Settings.HediffContextWeight.Value,
        hediff.pawn);
    }

    public static TemporalContextData? Create(Battle battle)
    {
      if (battle == null)
        return null;

      var participants = battle.Entries
        .SelectMany(entry => entry.GetConcerns())
        .Distinct()
        .ToArray();

      if (!participants.Any())
        return null;

      return new TemporalContextData(
        $"The battle '{battle.GetName()}' occurred with {String.Join(", ",
              participants
                .Take(25)
                .Select(pawn => pawn.LabelShort + (pawn.Faction != null ? $" ({pawn.Faction.Name})" : string.Empty))
                .ToArray())}",
        "battle",
        battle.CreationTimestamp,
        Math.Min(battle.Importance, Settings.BattleContextWeight.Value));
    }

    public static TemporalContextData? Create(Message message)
    {
      if (message == null || string.IsNullOrWhiteSpace(message.text))
        return null;
      return new TemporalContextData(
        message.text,
        message.def.defName,
        message.startingTick,
        Settings.MessageContextWeight.Value);
    }

    public static TemporalContextData? Create(RecentLetter letter)
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

      return new TemporalContextData(
        letter.Text,
        letter.Type,
        letter.Ticks,
        weight);
    }

    //public static TemporalContextData? Create(JobRecord jobRecord)
    //{
    //  var jobText = $"{jobRecord.Pawn.Name?.ToStringShort ?? jobRecord.Pawn.Label} was {jobRecord.Report?.Replace(".", "")}";

    //  if (jobRecord.Duration > 1)
    //    jobText += $" for {jobRecord.Duration.ToStringTicksToPeriod()}.";
    //  else if (jobRecord.Count > 1)
    //    jobText += $" {jobRecord.Count} times.";
    //  else
    //    jobText += ".";

    //    return new ExpiringTemporalContextData(
    //        jobText,
    //        "job",
    //        jobRecord.StartTick,
    //        Settings.JobContextWeight.Value,
    //        GenDate.TicksPerDay * 7,
    //        jobRecord.Pawn);
    //}

    public static TemporalContextData? Create(LogEntry entry)
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
        return new TemporalContextData(
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
        return new TemporalContextData(
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
        return new TemporalContextData(
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
        return new TemporalContextData(
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
        return new TemporalContextData(
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
        return new TemporalContextData(
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
        return new TemporalContextData(
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
        return new TemporalContextData(
          text,
          entry.GetType().ToString(),
          entry.Tick,
          Settings.LogEntryContextWeight.Value);
      }
    }

    public static TemporalContextData? Create(Conversation conversation)
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
      return new TemporalContextData(
        conversation.Text ?? "Missing Conversation text.",
        conversation.GetType().Name,
        conversation.Timestamp ?? 0,
        Settings.ConversationContextWeight.Value,
        involvedPawns.ToArray());
    }

    public async Task Add(
      string text,
      string type,
      int tick,
      float weight)
    {
      await Add(new TemporalContextData(text, type, tick, weight));
    }

    public async void Add(
      string text,
      string type,
      int tick,
      float weight,
      params Pawn[] pawns)
    {
      await Add(new TemporalContextData(text, type, tick, weight, pawns));
    }

    public class ExpiringTemporalContextData : TemporalContextData, IExpirable
    {
      public event Action<IContext>? OnExpired;

      public ExpiringTemporalContextData(
        string text,
        string type,
        int tick,
        float weight,
        int lifetimeTicks,
        params Pawn[] pawns) : base(text, type, tick, weight, pawns)
      {
        LifetimeTicks = lifetimeTicks;
        CreationTick = Find.TickManager.TicksAbs;
      }
      public int LifetimeTicks;
      public int CreationTick;

      public bool IsExpired => Find.TickManager.TicksAbs > CreationTick + LifetimeTicks;

      public void Expire()
      {
        //Do nothing
      }
    }

  }
}
