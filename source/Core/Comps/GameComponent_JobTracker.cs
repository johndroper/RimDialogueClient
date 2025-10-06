#nullable enable
using RimDialogue.Context;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using static RimDialogue.MainTabWindow_RimDialogue;

namespace RimDialogue.Core.Comps
{
  // ------------- Data Model -------------
  public class JobRecord : IExposable
  {
    public int PawnId;
    public string? Report;
    public int Count;
    public int StartTick;
    public int EndTick;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public JobRecord() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public JobRecord(
      Pawn pawn,
      string jobReport,
      int startTick)
    {
      PawnId = pawn.thingIDNumber;
      _pawn = pawn;
      Report = jobReport;
      StartTick = startTick;
      EndTick = startTick;
      Count = 1;
    }

    public void Increment()
    {
      Count += 1;
      EndTick = Find.TickManager.TicksGame;
    }

    public int Duration => EndTick - StartTick;

    public int TimeSince => Find.TickManager.TicksGame - EndTick;

    private Pawn? _pawn;
    public Pawn Pawn
    {
      get
      {
        _pawn ??= TrackerUtils.GetPawnById(PawnId);
        return _pawn ?? throw new Exception("Can't find pawn.");
      }
    }

    public void ExposeData()
    {
      Scribe_Values.Look(ref PawnId, "pawnId");
      Scribe_Values.Look(ref Report, "jobReport");
      Scribe_Values.Look(ref StartTick, "startTick");
      Scribe_Values.Look(ref EndTick, "endTick");
    }

    public override string ToString()
    {
      if (Pawn == null)
        return string.Empty;
      return $"[{Pawn.Name?.ToStringShort ?? "Unknown"}] was {Report} from {StartTick} to {EndTick}.";
    }
  }

  public class GameComponent_JobTracker : GameComponent
  {
    private Dictionary<Pawn, Dictionary<string, JobRecord>> JobCounts = [];
    public Dictionary<int, List<JobRecord>> JobRecords = [];

    private const int MaxRecords = 100;

    public GameComponent_JobTracker() { }
    public GameComponent_JobTracker(Game game) { }
    public static GameComponent_JobTracker Instance =>
        Current.Game.GetComponent<GameComponent_JobTracker>();
    public JobRecord? GetRandom(Pawn pawn)
    {
      var records = JobRecords[pawn.thingIDNumber];
      if (!records.Any())
        return null;
      var now = Find.TickManager.TicksAbs;
      return records.RandomElementByWeight(jobRecord => (jobRecord.EndTick / now) * jobRecord.Count);
    }

    public override void ExposeData()
    {
      var watch = Stopwatch.StartNew();
      Scribe_Collections.Look(ref JobRecords, "JobRecords", LookMode.Value, LookMode.Deep);
      watch.Stop();

      if (Scribe.mode == LoadSaveMode.LoadingVars)
      {
        JobRecords ??= [];
        foreach (int key in JobRecords.Keys)
        {
          var jobRecords = JobRecords[key];
          if (jobRecords.Count > MaxRecords)
            jobRecords.RemoveRange(0, jobRecords.Count - MaxRecords);
        }
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Loaded {JobRecords.Values.Sum(list => list.Count)} job records into tracker in {watch.Elapsed.TotalSeconds} seconds.");
      }
    }

    public int lastAddTick = GenDate.TicksPerHour;
    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();
      if (Find.TickManager.TicksGame > lastAddTick + GenDate.TicksPerHour)
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Adding job records to tracker. {JobCounts.Values.Sum(dict => dict.Count)} jobs to add.");
        lastAddTick = Find.TickManager.TicksGame;
        foreach (var jobRecords in JobCounts.Values)
        {
          foreach(var jobRecord in jobRecords.Values)
            Add(jobRecord);
        }
        JobRecords.Clear();
      }
    }

    public void Add(JobRecord jobRecord)
    {
      if (!JobRecords.ContainsKey(jobRecord.Pawn.thingIDNumber))
        JobRecords[jobRecord.Pawn.thingIDNumber] = [];
      var pawnRecords = JobRecords[jobRecord.Pawn.thingIDNumber];
      pawnRecords.Add(jobRecord);
      while (pawnRecords.Count > MaxRecords)
        pawnRecords.RemoveAt(0);
#if !RW_1_5
      if (GameComponent_ContextTracker.Instance != null)
      {
        var context = TemporalContextCatalog.Create(jobRecord);
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Adding context for job record: {context?.Text}");
        if (context == null)
          return;
        GameComponent_ContextTracker.Instance.Add(context);
      }
#endif
    }
    public void Build(
      Pawn pawn,
      string report,
      int startTick)
    {
      if (pawn == null || !pawn.IsColonist)
        return;

      if (JobCounts.TryGetValue(pawn, out Dictionary<string, JobRecord> jobRecords))
      {
        if (jobRecords.TryGetValue(report, out JobRecord? jobRecord))
          jobRecord.Increment();
        else
          jobRecords.Add(report, new JobRecord(
            pawn,
            report,
            startTick));
      }
      else
      {
        JobCounts.Add(pawn, []);
        JobCounts[pawn].Add(report, new JobRecord(
          pawn,
          report,
          startTick));
      }
    }
  }
}
