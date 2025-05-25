//using HarmonyLib;
//using RimDialogue.Core;
//using RimDialogue.Core.InteractionDefs;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEngine;
//using Verse;

//namespace RimDialogue.Access
//{
//  [HarmonyPatch]
//  public static class Patch_ThoughtMaker_MakeThought
//  {

//    public static MethodBase TargetMethod()
//    {
//      return AccessTools.Method(typeof(ThoughtMaker), nameof(ThoughtMaker.MakeThought),
//          new[] {
//            typeof(ThoughtDef)
//          });
//    }

//    // Postfix to run after the thought is created
//    public static void Postfix(ThoughtDef def, Thought __result)
//    {
//      GameComponent_ThoughtTracker.Instance.AddThought(__result);
//    }
//  }

//  [HarmonyPatch]
//  public static class Patch_ThoughtMaker_MakeThought2
//  {
//    public static MethodBase TargetMethod()
//    {
//      return AccessTools.Method(typeof(ThoughtMaker), nameof(ThoughtMaker.MakeThought),
//          new[] {
//            typeof(ThoughtDef),
//            typeof(int)
//          });
//    }

//    public static void Postfix(ThoughtDef def, ref int forcedStage, Thought __result)
//    {
//      GameComponent_ThoughtTracker.Instance.AddThought(__result);
//    }
//  }

//  [HarmonyPatch]
//  public static class Patch_ThoughtMaker_MakeThought3
//  {
//    public static MethodBase TargetMethod()
//    {
//      return AccessTools.Method(typeof(ThoughtMaker), nameof(ThoughtMaker.MakeThought),
//          new[] {
//            typeof(ThoughtDef),
//            typeof(Precept)
//          });
//    }

//    public static void Postfix(ThoughtDef def, Precept sourcePrecept, Thought __result)
//    {
//      GameComponent_ThoughtTracker.Instance.AddThought(__result);
//    }
//  }

//}
