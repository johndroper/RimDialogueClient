using System.Linq;
using RimDialogue.Core;
using UnityEngine;
using Verse;

namespace RimDialogue.Configuration
{
  public static class SettingsEditor
  {
    private static string?[] _colorBuffer = new string[4];

    private static Vector2 _scrollPosition = Vector2.zero;
    private static Rect _viewRect;

    public static void DrawSettings(Rect rect)
    {
      var listingRect = new Rect(rect.x, rect.y + 40f, rect.width, rect.height - 40f);
      var l = new Listing_Settings();
      l.Begin(rect);
      if (l.ButtonText("RimDialogue.ResetToDefault".Translate())) { Reset(); }
      l.End();
      l.BeginScrollView(listingRect, ref _scrollPosition, ref _viewRect);
      l.Label("RimDialogue.SpecialInstructions".Translate());
      Settings.SpecialInstructions.Value = l.TextEntry(Settings.SpecialInstructions.Value, 10);
      l.Label("RimDialogue.ClientId".Translate());
      Settings.ClientId.Value = l.TextEntry(Settings.ClientId.Value, 1);
      l.Label("RimDialogue.ServerUrl".Translate());
      if (string.IsNullOrWhiteSpace(Settings.ServerUrl.Value))
        Settings.ServerUrl.Value = "http://rimdialogue.proceduralproducts.com/home/getdialogue";
      Settings.ServerUrl.Value = l.TextEntry(Settings.ServerUrl.Value, 1);

      l.CheckboxLabeled("RimDialogue.ShowInteractionBubbles".Translate(), ref Settings.ShowInteractionBubbles.Value);
      l.CheckboxLabeled("RimDialogue.ShowDialogueBubbles".Translate(), ref Settings.ShowDialogueBubbles.Value);
      l.CheckboxLabeled("RimDialogue.ShowDialogueMessages".Translate(), ref Settings.ShowDialogueMessages.Value);
      l.CheckboxLabeled("RimDialogue.EnableCaravans".Translate(), ref Settings.EnableCaravans.Value);
      l.SliderLabeled("RimDialogue.DialogueMessageWidth".Translate(), ref Settings.DialogueMessageWidth.Value, 200, 1200);
      l.SliderLabeled("RimDialogue.DialogueMessageLifetime".Translate(), ref Settings.DialogueMessageLifetime.Value, 1f, 100f);
      l.SliderLabeled("RimDialogue.MinDialogueMessageLifetime".Translate(), ref Settings.MinDialogueMessageLifetime.Value, 1f, 10f);
      l.SliderLabeled("RimDialogue.MaxWords".Translate(), ref Settings.MaxWords.Value, 1, 100);
      l.SliderLabeled("RimDialogue.MaxSpeed".Translate(), ref Settings.MaxSpeed.Value, 1, 4);
      l.SliderLabeled("RimDialogue.MaxConversationsStored".Translate(), ref Settings.MaxConversationsStored.Value, 0, 100);
      l.SliderLabeled("RimDialogue.MinTimeBetweenConversations".Translate(), ref Settings.MinTimeBetweenConversations.Value, 0, 60);
      //l.CheckboxLabeled("RimDialogue.OnlyColonists".Translate(), ref Settings.OnlyColonists.Value);

      l.SliderLabeled("RimDialogue.MessageChitChatWeight".Translate(), ref Settings.MessageChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.GameConditionChitChatWeight".Translate(), ref Settings.GameConditionChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.RecentBattleChitChatWeight".Translate(), ref Settings.RecentBattleChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.RecentIncidentChitChatWeight".Translate(), ref Settings.RecentIncidentChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.AlertChitChatWeight".Translate(), ref Settings.AlertChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.SameIdeologyChitChatWeight".Translate(), ref Settings.SameIdeologyChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.SkillsChitChatWeight".Translate(), ref Settings.SkillChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.ColonistChitChatWeight".Translate(), ref Settings.ColonistChitChatWeight.Value, 0, 1);
      l.SliderLabeled("RimDialogue.HealthChitChatWeight".Translate(), ref Settings.HealthChitChatWeight.Value, 0, 1);

      l.CheckboxLabeled("RimDialogue.VerboseLogging".Translate(), ref Settings.VerboseLogging.Value);
      l.EndScrollView(ref _viewRect);
    }

    private static void Reset()
    {
      _colorBuffer = new string[4];

      Settings.Reset();
    }

    public static void ShowWindow() => Find.WindowStack!.Add(new Dialog());

    private sealed class Dialog : Window
    {
      public override Vector2 InitialSize => new(600f, 600f);

      public Dialog()
      {
        optionalTitle = $"<b>{Mod.Name}</b>";
        doCloseX = true;
        doCloseButton = true;
        draggable = true;

        _viewRect = default;
      }

      public override void DoWindowContents(Rect rect)
      {
        rect.yMax -= 60f;
        DrawSettings(rect);
      }

      public override void PostClose()
      {
        Mod.Instance.WriteSettings();
        _viewRect = default;
      }
    }
  }
}
