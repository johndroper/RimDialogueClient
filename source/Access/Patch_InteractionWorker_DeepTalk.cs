using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(InteractionWorker_DeepTalk), nameof(InteractionWorker_DeepTalk.RandomSelectionWeight))]
  public static class Patch_InteractionWorker_DeepTalk
  {
    public static void Postfix(Pawn initiator, Pawn recipient, ref float __result)
    {
      __result *= Settings.DeepTalkCompensationFactor.Value;
    }
  }
}
