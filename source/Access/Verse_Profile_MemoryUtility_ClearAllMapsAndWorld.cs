using RimDialogue.Core;
using HarmonyLib;
using Verse.Profile;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
  public static class Verse_Profile_MemoryUtility_ClearAllMapsAndWorld
  {
    private static void Prefix() => Bubbles_Bubbler_Add.Clear();
  }
}
