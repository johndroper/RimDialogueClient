#nullable enable
using HarmonyLib;
using RimDialogue.Core;
using RimDialogue.Core.Comps;
using RimWorld;
using System;
using System.IO;
using Verse;
using Verse.AI;

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
        out (Pawn? pawn, string? report, int startTick) __state)
    {
      try
      {


        var pawn = (Pawn)Reflection.Verse_Pawn_JobTracker_Pawn.GetValue(__instance);
        if (!pawn.IsColonist || pawn.Dead || pawn.Destroyed || pawn.Map == null)
        {
          __state = (null, null, 0);
          return;
        }

        //HaulToContainer job is buggy
        if (__instance?.curJob.GetCachedDriver(pawn) is JobDriver_HaulToContainer)
        {
          __state = (null, null, 0);
          return;
        }

        string? report = __instance?.curJob.GetReport(pawn).RemoveTags();

        __state = (
          pawn,
          report,
          __instance?.curJob.startTick ?? 0);

#if DEBUG
        try
        {
          File.AppendAllText($"{GenFilePaths.SaveDataFolderPath}\\Logs\\jobs.log", report + Environment.NewLine);
        }
        catch (Exception ex)
        {
          Log.Error($"RimDialogue failed to log job state: {ex}");
        }
#endif

      }
      catch (Exception ex)
      {
        Mod.Error($"RimDialogue failed Pawn_JobTracker Prefix: {ex}");
        __state = (null, null, 0);
      }
    }

    public static void Postfix((Pawn? pawn, string? report, int startTick) __state)
    {
      try
      {
        var (pawn, report, startTick) = __state;
        if (pawn == null || report == null || string.IsNullOrWhiteSpace(report))
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
