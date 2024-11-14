using RimDialogue.Configuration;
using RimDialogue.Core;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
  public static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
  {
    private static void Postfix(WidgetRow? row, bool worldView)
    {
      if (worldView || row is null) { return; }

      var activated = Settings.Activated;
      row.ToggleableIcon(ref activated, Textures.Icon, "RimDialogue.Toggle".Translate(), SoundDefOf.Mouseover_ButtonToggle);

      if (activated != Settings.Activated && Event.current!.shift) { SettingsEditor.ShowWindow(); }
      else { Settings.Activated = activated; }

      if (!Settings.Activated) { Bubbler.Clear(); }
    }
  }
}
