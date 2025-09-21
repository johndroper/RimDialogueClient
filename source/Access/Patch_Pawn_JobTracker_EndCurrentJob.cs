using HarmonyLib;
using RimDialogue.Core;
using RimDialogue.Core.Comps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine.Rendering;
using Verse;
using Verse.AI;
using static RimWorld.PsychicRitualRoleDef;

namespace RimDialogue.Access
{
  //public void EndCurrentJob(JobCondition condition, bool startNewJob = true, bool canReturnToPool = true)

  [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob))]
  public static class Patch_EndCurrentJob
  {
    public static void Prefix(
        Pawn_JobTracker __instance,
        JobCondition condition,
        bool startNewJob,
        bool canReturnToPool,
        out (Pawn pawn, string report, int startTick) __state)
    {
      try
      {
        var pawn = (Pawn)Reflection.Verse_Pawn_JobTracker_Pawn.GetValue(__instance);
        if (!pawn.IsColonist || pawn.Dead || pawn.Destroyed || pawn.Map == null)
        {
          __state = (null, "Pawn is dead, destroyed, or has no map", 0);
          return;
        }
        __state = (
          pawn,
          __instance?.curJob.GetReport(pawn).RemoveTags(),
          __instance?.curJob.startTick ?? 0);
      }
      catch (Exception ex)
      {
        Mod.Error($"RimDialogue failed Pawn_JobTracker Prefix: {ex}");
        __state = (null, "Pawn_JobTracker Prefix Error", 0);
      }
    }

#if Debug
    private static void LogJob(Pawn pawn, string report, Job job, JobCondition cond, int count)
    {
      try
      {
        string logDir = Path.Combine("D:\\junk", "RimDialogue", "JobLogs");
        Directory.CreateDirectory(logDir);
        string logFile = Path.Combine(logDir, $"job_log_{DateTime.Now:yyyy-MM-dd}.txt");
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        string entry = $"[{timestamp}] Pawn: {pawn.Name?.ToStringFull ?? pawn.LabelCap}, " +
                       $"Job: {report ?? "N/A"}, DefName: {job.def?.defName ?? "N/A"}, " +
                       $"Condition: {cond}, Tick: {Find.TickManager.TicksAbs}, " +
                       $"Count: {count}";
        File.AppendAllText(logFile, entry + Environment.NewLine);
      }
      catch (Exception ex)
      {
        Log.Error($"RimDialogue failed to log job state: {ex}");
      }
    }
#endif

    public static void Postfix((Pawn pawn, string report, int startTick) __state)
    {
      try
      {
        var (pawn, report, startTick) = __state;
        if (pawn == null || string.IsNullOrWhiteSpace(report))
          return;
        if (GameComponent_JobTracker.Instance == null)
          return;
        GameComponent_JobTracker.Instance.Build(
          pawn,
          report,
          startTick);
      }
      catch (Exception ex)
      {
        Mod.Error($"RimDialogue failed Pawn_JobTracker Postfix: {ex}.");
      }
    }
  }
}
