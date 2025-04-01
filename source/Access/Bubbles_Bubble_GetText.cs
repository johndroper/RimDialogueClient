using Bubbles.Core;
using HarmonyLib;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(Bubble), "GetText")]
  public static class Bubbles_Bubble_GetText
  {
    public static bool Prefix(Bubble __instance, ref string __result)
    {
      var text = Bubbles_Bubbler_Add.GetDialogueText(__instance);
      if (text != null)
        __result = text;
      return (text == null);
    }
  }
}
