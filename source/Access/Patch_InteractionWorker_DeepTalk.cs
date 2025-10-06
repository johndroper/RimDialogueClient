using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(InteractionWorker_DeepTalk), nameof(InteractionWorker_DeepTalk.RandomSelectionWeight))]
  public static class Patch_InteractionWorker_DeepTalk
  {
    public static void Postfix(Pawn initiator, Pawn recipient, ref float __result)
    {
      try
      {
        __result *= Settings.DeepTalkCompensationFactor.Value;
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"An error occurred in Patch_InteractionWorker_DeepTalk.\r\n{ex}", 45345789);
      }
    }
  }
}
