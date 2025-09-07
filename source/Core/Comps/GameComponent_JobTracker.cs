#nullable enable
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Jobs.LowLevel.Unsafe;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;

namespace RimDialogue.Core.Comps
{
  // ------------- Data Model -------------
  public class JobRecord : IExposable
  {
    public int PawnId;
    public string? JobType;
    public string? JobReport;
    public int StartTick;
    public int EndTick;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public JobRecord() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public JobRecord(
      int pawnId,
      string jobReport,
      string jobType,
      int startTick,
      int endTick)
    {
      PawnId = pawnId;
      JobReport = jobReport;
      JobType = jobType;
      StartTick = startTick;
      EndTick = endTick;
    }

    private Pawn? _pawn;
    public Pawn? Pawn
    {
      get
      {
        _pawn ??= TrackerUtils.GetPawnById(PawnId);
        return _pawn;
      }
    }

    public void ExposeData()
    {
      Scribe_Values.Look(ref PawnId, "pawnId");
      Scribe_Values.Look(ref JobReport, "jobReport");
      Scribe_Values.Look(ref JobType, "jobType");
      Scribe_Values.Look(ref StartTick, "startTick");
      Scribe_Values.Look(ref EndTick, "endTick");
    }

    public override string ToString()
    {
      if (Pawn == null)
        return string.Empty;
      return $"[{Pawn.Name?.ToStringShort ?? "Unknown"}] {JobType}: start {StartTick}, ended {EndTick}";
    }
  }

  public class GameComponent_JobTracker : GameComponent
  {
    public Dictionary<int, List<JobRecord>> Records = [];

    private const int MaxEvents = 10;

    public GameComponent_JobTracker() { }
    public GameComponent_JobTracker(Game game) { }


    public static GameComponent_JobTracker Instance =>
        Current.Game.GetComponent<GameComponent_JobTracker>();

    public JobRecord? GetRandom(Pawn pawn)
    {
      var records = Records[pawn.thingIDNumber];
      if (!records.Any())
        return null;

      var now = Find.TickManager.TicksAbs;
      return records.RandomElementByWeight(jobRecord => jobRecord.EndTick / now);
    }

    public override void ExposeData()
    {
      //Scribe_Collections.Look(ref Records, "events", LookMode.Value, LookMode.Deep);
      //if (Scribe.mode == LoadSaveMode.PostLoadInit)
      //  Records ??= [];
    }

    //public void Add(
    //  Pawn pawn,
    //  string jobReport,
    //  string jobType,
    //  int startTick,
    //  int endTick)
    //{
    //  if (!Records.ContainsKey(pawn.thingIDNumber))
    //    Records[pawn.thingIDNumber] = [];
    //  var pawnRecords = Records[pawn.thingIDNumber];
    //  var record = new JobRecord(
    //    pawn.thingIDNumber,
    //    jobReport,
    //    jobType,
    //    startTick,
    //    endTick);
    //  lock (pawnRecords)
    //  {
    //    pawnRecords.Add(record);
    //    if (pawnRecords.Count > MaxEvents)
    //    {
    //      int remove = Records.Count - MaxEvents;
    //      if (remove > 0)
    //        pawnRecords.RemoveRange(0, remove);
    //    }
    //  }
    //  GameComponent_ContextTracker.Instance.Add(
    //    record.JobReport,
    //    record.JobType,
    //    record.StartTick,
    //    pawn);
    //}
   }
}
