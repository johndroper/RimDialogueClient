using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RimDialogue.Configuration;
using HarmonyLib;
using UnityEngine;
using Verse;
using System;

namespace RimDialogue
{
  public class Settings : ModSettings
  {
    public const int AutoHideSpeedDisabled = 1;

    private static readonly string[] SameConfigVersions =
    {
      "2.6",
      "2.7"
    };

    private static bool _resetRequired;

    public static bool Activated = true;

    public static readonly Setting<int> MaxWords = new(nameof(MaxWords), 25);
    public static readonly SettingString SpecialInstructions = new(nameof(SpecialInstructions), string.Empty);
    public static readonly SettingString ClientId = new(nameof(ClientId), Guid.NewGuid().ToString());
    public static readonly SettingString ServerUrl = new(nameof(ServerUrl), "http://rimdialogue.proceduralproducts.com/home/getdialogue");
    public static readonly Setting<int> MaxSpeed = new(nameof(MaxSpeed), 1);
    public static readonly Setting<int> MaxConversationsStored = new(nameof(MaxConversationsStored), 25);

    private static IEnumerable<Setting> AllSettings => typeof(Settings).GetFields().Select(static field => field.GetValue(null) as Setting).Where(static setting => setting is not null)!;

    public static void Reset() => AllSettings.Do(static setting => setting.ToDefault());

    public void CheckResetRequired()
    {
      if (!_resetRequired) { return; }
      _resetRequired = false;

      Write();

      RimDialogue.Mod.Warning("Settings were reset with new update");
    }

    public override void ExposeData()
    {
      if (_resetRequired) { return; }

      var version = Scribe.mode is LoadSaveMode.Saving ? RimDialogue.Mod.Version : null;
      Scribe_Values.Look(ref version, "Version");
      if (Scribe.mode is LoadSaveMode.LoadingVars && (version is null || (version is not RimDialogue.Mod.Version && !SameConfigVersions.Contains(Regex.Match(version, "^\\d+\\.\\d+").Value))))
      {
        _resetRequired = true;
        return;
      }

      AllSettings.Do(static setting => setting.Scribe());
    }
  }
}
