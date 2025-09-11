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
      try
      {
        Bubbles_Bubbler_Add.Clear();
        InteractionWorker_Dialogue.LastUsedTicksAll = 0;
        InteractionWorker_Dialogue.LastTicksByType = [];
        DialogueRequest.LastDialogue = DateTime.MinValue;
        DialogueRequest.Requests.Clear();
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"Error in Verse_Profile_MemoryUtility_ClearAllMapsAndWorld. {ex}", 3587101);
      }
    }
  }
}
