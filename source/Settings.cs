using HarmonyLib;
using RimDialogue.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue
{
  public class Settings : ModSettings
  {
    public const int AutoHideSpeedDisabled = 1;

    public static bool Activated = true;

    public static readonly Setting<int> MaxWords = new(nameof(MaxWords), 50);
    public static readonly Setting<int> MinWords = new(nameof(MinWords), 15);
    public static readonly SettingString SpecialInstructions = new(nameof(SpecialInstructions), string.Empty);
    public static readonly SettingString ClientId = new(nameof(ClientId), Guid.NewGuid().ToString());
    public static readonly SettingString ServerUrl = new(nameof(ServerUrl), "http://rimdialogue.proceduralproducts.com/");
    //public static readonly Setting<int> MaxSpeed = new(nameof(MaxSpeed), 3);
    public static readonly Setting<int> MaxConversationsStored = new(nameof(MaxConversationsStored), 25);
    public static readonly Setting<bool> VerboseLogging = new(nameof(VerboseLogging), false);

    public static readonly Setting<bool> EnableCaravans = new(nameof(EnableCaravans), false);

    public static readonly Setting<bool> ShowInteractionBubbles = new(nameof(ShowInteractionBubbles), false);
    public static readonly Setting<bool> ShowDialogueBubbles = new(nameof(ShowDialogueBubbles), true);
    public static readonly Setting<bool> ShowDialogueMessages = new(nameof(ShowDialogueMessages), true);
    public static readonly Setting<float> DialogueMessageLifetime = new(nameof(DialogueMessageLifetime), 13f);
    public static readonly Setting<float> MinDialogueMessageLifetime = new(nameof(MinDialogueMessageLifetime), 2f);
    public static readonly Setting<int> DialogueMessageWidth = new(nameof(DialogueMessageWidth), 600);

    public static readonly Setting<int> MinDelayMinutesAll = new(nameof(MinDelayMinutesAll), 1);
    public static readonly Setting<int> MinDelayMinutes = new(nameof(MinDelayMinutes), 5);
    public static readonly Setting<int> MinTimeBetweenConversations = new(nameof(MinTimeBetweenConversations), 1);

    public static readonly Setting<int> RecentIncidentHours = new(nameof(RecentIncidentHours), 4);
    public static readonly Setting<int> RecentBattleHours = new(nameof(RecentBattleHours), 6);

    public static readonly Setting<float> MessageChitChatWeight = new(nameof(MessageChitChatWeight), 1f);
    public static readonly Setting<float> GameConditionChitChatWeight = new(nameof(GameConditionChitChatWeight), 1f);
    public static readonly Setting<float> BattleChitChatWeight = new(nameof(BattleChitChatWeight), 1f);
    public static readonly Setting<float> RecentIncidentChitChatWeight = new(nameof(RecentIncidentChitChatWeight), 1f);
    public static readonly Setting<float> AlertChitChatWeight = new(nameof(AlertChitChatWeight), 1f);
    public static readonly Setting<float> SameIdeologyChitChatWeight = new(nameof(SameIdeologyChitChatWeight), 1f);
    public static readonly Setting<float> SkillChitChatWeight = new(nameof(SkillChitChatWeight), 1f);
    public static readonly Setting<float> ColonistChitChatWeight = new(nameof(ColonistChitChatWeight), 1f);
    public static readonly Setting<float> HealthChitChatWeight = new(nameof(HealthChitChatWeight), 1f);
    public static readonly Setting<float> ApparelChitChatWeight = new(nameof(ApparelChitChatWeight), 1f);
    public static readonly Setting<float> NeedChitChatWeight = new(nameof(NeedChitChatWeight), 1f);
    public static readonly Setting<float> FamilyChitChatWeight = new(nameof(FamilyChitChatWeight), 1f);
    public static readonly Setting<float> WeatherChitChatWeight = new(nameof(WeatherChitChatWeight), 1f);
    public static readonly Setting<float> FactionChitChatWeight = new(nameof(FactionChitChatWeight), 1f);
    public static readonly Setting<float> WeaponChitChatWeight = new(nameof(WeaponChitChatWeight), 1f);
    public static readonly Setting<float> AppearanceChitChatWeight = new(nameof(AppearanceChitChatWeight), 1f);
    public static readonly Setting<float> AnimalChitChatWeight = new(nameof(AnimalChitChatWeight), 1f);
    public static readonly Setting<float> RoomChitChatWeight = new(nameof(RoomChitChatWeight), 1f);

    public static readonly Setting<float> DeadColonistWeight = new(nameof(DeadColonistWeight), 1f);

    private static IEnumerable<Setting> AllSettings => typeof(Settings).GetFields().Select(static field => field.GetValue(null) as Setting).Where(static setting => setting is not null)!;

    public static void Reset() => AllSettings.Do(static setting => setting.ToDefault());

    public static Setting<float>[] chitChatWeights = new[]
    {
        MessageChitChatWeight,
        GameConditionChitChatWeight,
        BattleChitChatWeight,
        RecentIncidentChitChatWeight,
        AlertChitChatWeight,
        SameIdeologyChitChatWeight,
        SkillChitChatWeight,
        ColonistChitChatWeight,
        HealthChitChatWeight,
        ApparelChitChatWeight,
        NeedChitChatWeight,
        FamilyChitChatWeight,
        WeatherChitChatWeight,
        FactionChitChatWeight,
        WeaponChitChatWeight,
        AppearanceChitChatWeight,
        AnimalChitChatWeight,
        RoomChitChatWeight,
        DeadColonistWeight
    };

    public static void ToggleAllChitChatWeights() // Add this method
    {
      foreach (var weight in chitChatWeights)
      {
        weight.Value = weight.Value == 0 ? 1 : 0;
      }
    }

    public override void ExposeData()
    {
      var version = Scribe.mode is LoadSaveMode.Saving ? RimDialogue.Mod.Version : null;
      Scribe_Values.Look(ref version, "Version");
      AllSettings.Do(static setting => setting.Scribe());
    }
  }
}
