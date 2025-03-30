using HarmonyLib;
using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionWorkers;
using System;
using Verse.Profile;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
  public static class Verse_Profile_MemoryUtility_ClearAllMapsAndWorld
  {
    private static void Prefix()
    {
      Bubbles_Bubbler_Add.Clear();
      InteractionWorker_Dialogue.LastUsedTicksAll = 0;
      InteractionWorker_Dialogue.LastTicksByType = [];
      DialogueRequest.LastDialogue = DateTime.MinValue;
    }
  }
}
