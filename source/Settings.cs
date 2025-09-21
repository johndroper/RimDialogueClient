using HarmonyLib;
using RimDialogue.Configuration;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Verse;

namespace RimDialogue
{
  public class Settings : ModSettings
  {
    public const int AutoHideSpeedDisabled = 1;

    public static bool Activated = true;

    public static readonly Setting<int> MaxWords = new(nameof(MaxWords), 75);
    public static readonly Setting<int> MinWords = new(nameof(MinWords), 15);
    public static readonly SettingString SpecialInstructions = new(nameof(SpecialInstructions), string.Empty);
    public static readonly SettingString ClientId = new(nameof(ClientId), Guid.NewGuid().ToString());
    public static readonly SettingString ServerUrl = new(nameof(ServerUrl), "https://rimdialogue.proceduralproducts.com/");
    public static readonly Setting<int> MaxConversationsStored = new(nameof(MaxConversationsStored), 100);
    public static readonly Setting<bool> VerboseLogging = new(nameof(VerboseLogging), false);
    public static readonly Setting<bool> OnlyColonists = new(nameof(OnlyColonists), false);
    public static readonly Setting<bool> HideDiscordButton = new(nameof(HideDiscordButton), false);

    public static readonly Setting<bool> EnableCaravans = new(nameof(EnableCaravans), false);

    public static readonly Setting<bool> ShowInteractionBubbles = new(nameof(ShowInteractionBubbles), false);
    public static readonly Setting<bool> ShowDialogueBubbles = new(nameof(ShowDialogueBubbles), true);
    public static readonly Setting<float> DialogueMessageLifetime = new(nameof(DialogueMessageLifetime), 13f);
    public static readonly Setting<float> MinDialogueMessageLifetime = new(nameof(MinDialogueMessageLifetime), 2f);
    public static readonly Setting<int> DialogueMessageWidth = new(nameof(DialogueMessageWidth), 600);

    public static readonly Setting<int> MinDelayMinutesAll = new(nameof(MinDelayMinutesAll), 5);
    public static readonly Setting<int> MinDelayMinutes = new(nameof(MinDelayMinutes), 10);
    public static readonly Setting<int> MinTimeBetweenConversations = new(nameof(MinTimeBetweenConversations), 5);

    public static readonly Setting<int> RecentIncidentHours = new(nameof(RecentIncidentHours), 4);
    public static readonly Setting<int> RecentBattleHours = new(nameof(RecentBattleHours), 6);

    public static readonly Setting<float> TimelyEventWeight = new(nameof(TimelyEventWeight), 5f);

    public static readonly Setting<float> MessageChitChatWeight = new(nameof(MessageChitChatWeight), 0.05f);
    public static readonly Setting<float> GameConditionChitChatWeight = new(nameof(GameConditionChitChatWeight), 0.05f);
    public static readonly Setting<float> BattleChitChatWeight = new(nameof(BattleChitChatWeight), 0.05f);
    public static readonly Setting<float> RecentIncidentChitChatWeight = new(nameof(RecentIncidentChitChatWeight), 0.05f);
    public static readonly Setting<float> AlertChitChatWeight = new(nameof(AlertChitChatWeight), 0.05f);
    public static readonly Setting<float> SameIdeologyChitChatWeight = new(nameof(SameIdeologyChitChatWeight), 0.05f);
    public static readonly Setting<float> SkillChitChatWeight = new(nameof(SkillChitChatWeight), 0.05f);
    public static readonly Setting<float> ColonistChitChatWeight = new(nameof(ColonistChitChatWeight), 0.05f);
    public static readonly Setting<float> HealthChitChatWeight = new(nameof(HealthChitChatWeight), 0.05f);
    public static readonly Setting<float> ApparelChitChatWeight = new(nameof(ApparelChitChatWeight), 0.05f);
    public static readonly Setting<float> NeedChitChatWeight = new(nameof(NeedChitChatWeight), 0.05f);
    public static readonly Setting<float> FamilyChitChatWeight = new(nameof(FamilyChitChatWeight), 0.05f);
    public static readonly Setting<float> WeatherChitChatWeight = new(nameof(WeatherChitChatWeight), 0.05f);
    public static readonly Setting<float> FactionChitChatWeight = new(nameof(FactionChitChatWeight), 0.05f);
    public static readonly Setting<float> WeaponChitChatWeight = new(nameof(WeaponChitChatWeight), 0.05f);
    public static readonly Setting<float> AppearanceChitChatWeight = new(nameof(AppearanceChitChatWeight), 0.05f);
    public static readonly Setting<float> AnimalChitChatWeight = new(nameof(AnimalChitChatWeight), 0.05f);
    public static readonly Setting<float> RoomChitChatWeight = new(nameof(RoomChitChatWeight), 0.05f);
    public static readonly Setting<float> DeadColonistWeight = new(nameof(DeadColonistWeight), 0.05f);
    public static readonly Setting<float> QuestWeight = new(nameof(QuestWeight), 0.05f);
    public static readonly Setting<float> RecordWeight = new(nameof(RecordWeight), 0.05f);

    public static readonly Setting<float> MeleeCombatQuipChance = new(nameof(MeleeCombatQuipChance), .1f);
    public static readonly Setting<float> RangedFireQuipChance = new(nameof(RangedFireQuipChance), .1f);
    public static readonly Setting<float> RangedImpactQuipChance = new(nameof(RangedImpactQuipChance), .25f);
    public static readonly Setting<float> DamageTakenQuipChance = new(nameof(DamageTakenQuipChance), .25f);
    public static readonly Setting<float> ImHitChance = new(nameof(ImHitChance), .333f);
    public static readonly Setting<float> ThoughtChance = new(nameof(ThoughtChance), .25f);

    public static readonly Setting<int> DialogueMessageInterface = new(nameof(DialogueMessageInterface), 1);
    public static readonly Setting<int> MessageScrollSpeed = new(nameof(MessageScrollSpeed), 12);

    public static readonly Setting<int> DeepTalkCompensationFactor = new(nameof(DeepTalkCompensationFactor), 20);

    public static readonly Setting<int> BitmapFont = new(nameof(BitmapFont), (int)GetDefaultFontFace());

    public static readonly SettingString ModelName = new(nameof(ModelName), "Default");
    public static readonly SettingString FilterWords = new(nameof(FilterWords), string.Empty);
    public static readonly SettingString DefaultSavePath = new(nameof(DefaultSavePath), Path.Combine(GenFilePaths.ScreenshotFolderPath, "RimDialogue"));

    public static readonly Setting<int> MaxContextItems = new(nameof(MaxContextItems), 500);
    public static readonly Setting<int> MaxContextAgeDays = new(nameof(MaxContextAgeDays), 100);

    public static readonly Setting<float> JobContextWeight = new(nameof(JobContextWeight), 0.85f);
    public static readonly Setting<float> LogEntryContextWeight = new(nameof(LogEntryContextWeight), 1f);
    public static readonly Setting<float> BattleLogEntryContextWeight = new(nameof(BattleLogEntryContextWeight), 1.25f);
    public static readonly Setting<float> ConversationContextWeight = new(nameof(ConversationContextWeight), 1f);
    public static readonly Setting<float> ThreatBigLetterContextWeight = new(nameof(ThreatBigLetterContextWeight), 5f);
    public static readonly Setting<float> NegativeLetterContextWeight = new(nameof(NegativeLetterContextWeight), 4f);
    public static readonly Setting<float> PositiveLetterContextWeight = new(nameof(PositiveLetterContextWeight), 3f);
    public static readonly Setting<float> NeutralLetterContextWeight = new(nameof(NeutralLetterContextWeight), 1.5f);
    public static readonly Setting<float> MessageContextWeight = new(nameof(MessageContextWeight), 1f);
    public static readonly Setting<float> BattleContextWeight = new(nameof(BattleContextWeight), 3f);
    public static readonly Setting<float> HediffContextWeight = new(nameof(HediffContextWeight), 2f);

    public static HashSet<string> filterWords = [];

    public static char[] splitChars = [' ', '\t', '\r', '\n', '.', ',', '!', '?', ';', ':', '-', '_', '(', ')', '[', ']', '{', '}', '"', '\''];

    public static void SetFilterWords(string words)
    {
      FilterWords.Value = words;
      filterWords.Clear();
      filterWords.AddRange(words.Split([' '], StringSplitOptions.RemoveEmptyEntries)
        .Select(word => word.Trim())
        .Where(word => !string.IsNullOrWhiteSpace(word)));
    }

    public static bool IsFiltered(string text)
    {
      if (!filterWords.Any())
        return false;
      var isFiltered = text.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)
        .Select(word => word.Trim())
        .Where(word => !string.IsNullOrWhiteSpace(word))
        .Any(word => filterWords.Contains(word, StringComparer.OrdinalIgnoreCase));
      return isFiltered;
    }

    public static FontFace GetDefaultFontFace()
    {
      switch(LanguageDatabase.activeLanguage?.folderName ?? LanguageDatabase.defaultLanguage?.folderName)
      {
        case "ChineseSimplified":
          return FontFace.NotoSansSC;
        default:
          return FontFace.Calibri; 
      }
    }

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
      if (Scribe.mode is LoadSaveMode.PostLoadInit)
        SetFilterWords(FilterWords.Value);
    }
  }
}
