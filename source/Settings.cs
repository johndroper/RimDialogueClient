using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RimDialogue.Configuration;
using HarmonyLib;
using UnityEngine;
using Verse;
using System;
using RimDialogue.Access;

namespace RimDialogue
{
  public class Settings : ModSettings
  {
    public const int AutoHideSpeedDisabled = 1;

    public static bool Activated = true;

    public static readonly Setting<int> MaxWords = new(nameof(MaxWords), 25);
    public static readonly SettingString SpecialInstructions = new(nameof(SpecialInstructions), string.Empty);
    public static readonly SettingString ClientId = new(nameof(ClientId), Guid.NewGuid().ToString());
    public static readonly SettingString ServerUrl = new(nameof(ServerUrl), "http://rimdialogue.proceduralproducts.com/");
    public static readonly Setting<int> MaxSpeed = new(nameof(MaxSpeed), 1);
    public static readonly Setting<int> MaxConversationsStored = new(nameof(MaxConversationsStored), 25);
    public static readonly Setting<bool> VerboseLogging = new(nameof(VerboseLogging), false);
    public static readonly Setting<int> MinTimeBetweenConversations = new(nameof(MinTimeBetweenConversations), 0);
    public static readonly Setting<bool> EnableCaravans = new(nameof(EnableCaravans), false);

    public static readonly Setting<bool> ShowInteractionBubbles = new(nameof(ShowInteractionBubbles), false);
    public static readonly Setting<bool> ShowDialogueBubbles = new(nameof(ShowDialogueBubbles), true);
    public static readonly Setting<bool> ShowDialogueMessages = new(nameof(ShowDialogueMessages), true);
    public static readonly Setting<float> DialogueMessageLifetime = new(nameof(DialogueMessageLifetime), 13f);
    public static readonly Setting<float> MinDialogueMessageLifetime = new(nameof(MinDialogueMessageLifetime), 2f);
    public static readonly Setting<int> DialogueMessageWidth = new(nameof(DialogueMessageWidth), 600);

    public static readonly Setting<int> ChitChatMinMinutes = new(nameof(ChitChatMinMinutes), 0);
    public static readonly Setting<int> RecentIncidentHours = new(nameof(RecentIncidentHours), 2);
    public static readonly Setting<int> RecentBattleHours = new(nameof(RecentBattleHours), 6);

    public static readonly Setting<float> MessageChitChatWeight = new(nameof(MessageChitChatWeight), 1f);
    public static readonly Setting<float> GameConditionChitChatWeight = new(nameof(GameConditionChitChatWeight), 1f);
    public static readonly Setting<float> RecentBattleChitChatWeight = new(nameof(RecentBattleChitChatWeight), 1f);
    public static readonly Setting<float> RecentIncidentChitChatWeight = new(nameof(RecentIncidentChitChatWeight), 1f);
    public static readonly Setting<float> AlertChitChatWeight = new(nameof(AlertChitChatWeight), 1f);


    private static IEnumerable<Setting> AllSettings => typeof(Settings).GetFields().Select(static field => field.GetValue(null) as Setting).Where(static setting => setting is not null)!;

    public static void Reset() => AllSettings.Do(static setting => setting.ToDefault());

    public override void ExposeData()
    {
      var version = Scribe.mode is LoadSaveMode.Saving ? RimDialogue.Mod.Version : null;
      Scribe_Values.Look(ref version, "Version");
      AllSettings.Do(static setting => setting.Scribe());
    }
  }
}
