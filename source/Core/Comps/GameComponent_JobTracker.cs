#nullable enable
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Jobs.LowLevel.Unsafe;
using Verse;
using Verse.AI;
using Verse.Noise;
using static UnityEngine.GraphicsBuffer;

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
      EndTick = Find.TickManager.TicksAbs;
    }

    public int Duration => EndTick - StartTick;

    public int TimeSince => Find.TickManager.TicksAbs - EndTick;

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
    private Dictionary<Pawn, JobRecord> JobCounts = [];
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
      Scribe_Collections.Look(ref JobRecords, "events", LookMode.Value, LookMode.Deep);
      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        JobRecords ??= [];
    }

    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();
      if (Find.TickManager.TicksGame % 2500 == 0)
      {
        foreach(var pawnRecords in JobRecords.Values)
        {
          if (pawnRecords.Count > MaxRecords)
          {
            int remove = JobRecords.Count - MaxRecords;
            if (remove > 0)
              pawnRecords.RemoveRange(0, remove);
          }
        }
      }
    }

    public void Add(JobRecord jobRecord)
    {
      if (!JobRecords.ContainsKey(jobRecord.Pawn.thingIDNumber))
        JobRecords[jobRecord.Pawn.thingIDNumber] = [];
      var pawnRecords = JobRecords[jobRecord.Pawn.thingIDNumber];
      pawnRecords.Add(jobRecord);

      if (GameComponent_ContextTracker.Instance != null)
        GameComponent_ContextTracker.Instance.Add(jobRecord);
    }

    public void Build(
      Pawn pawn,
      string report,
      int startTick)
    {
      if (pawn == null || !pawn.IsColonist)
        return;

      if (JobCounts.TryGetValue(pawn, out JobRecord jobRecord))
      {
        if (jobRecord.Report == report)
          jobRecord.Increment();
        else
        {
          Add(jobRecord);
          JobCounts[pawn] = new JobRecord(
            pawn,
            report,
            startTick);
        }
      }
      else
        JobCounts.Add(pawn, new JobRecord(
            pawn,
            report,
            startTick));
    }
  }
}
