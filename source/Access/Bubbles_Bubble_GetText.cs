using Bubbles.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(Bubble), "GetText")]
  public static class Bubbles_Bubble_GetText
  {
    public static bool Prefix(Bubble __instance, ref string __result)
    {
      __result = Bubbles_Bubbler_Add.GetDialogueText(__instance);
      return false;
    }
  }
}
