//using HarmonyLib;
//using RimDialogue.Core.Comps;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Jobs;
//using UnityEngine.Rendering;
//using Verse;
//using Verse.AI;

//namespace RimDialogue.Access
//{
//  //public void EndCurrentJob(JobCondition condition, bool startNewJob = true, bool canReturnToPool = true)

//  [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob))]
//  public static class Patch_EndCurrentJob
//  {
//    public static void Prefix(
//        Pawn_JobTracker __instance,
//        JobCondition condition,
//        bool startNewJob,
//        bool canReturnToPool,
//        out (Pawn pawn, string report, Job job, JobCondition cond) __state)
//    {
//      var pawn = (Pawn)Reflection.Verse_Pawn_JobTracker_Pawn.GetValue(__instance);
//      __state = (
//        pawn,
//        __instance?.curJob.GetReport(pawn),
//        __instance?.curJob,
//        condition);
//    }

//    public static void Postfix((Pawn pawn, string report, Job job, JobCondition cond) __state)
//    {
//      var (pawn, report, job, condition) = __state;
//      if (pawn != null && job != null)
//        GameComponent_JobTracker.Instance.Add(
//          pawn,
//          report,
//          "job",
//          job.startTick,
//          Find.TickManager.TicksAbs);
//    }
//  }
//}
