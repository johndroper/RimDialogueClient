using Bubbles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using static UnityEngine.Scripting.GarbageCollector;
using static Verse.Widgets;

namespace RimDialogue.Configuration
{



  public static class SettingsEditor
  {
    private static string?[] _colorBuffer = new string[4];

    private static Vector2 _scrollPosition = Vector2.zero;
    private static Rect _viewRect;

    private static string InterfaceDropdownLabel(int value)
    {
      return value switch
      {
        1 => "Moveable",
        2 => "Top Center",
        3 => "None",
        _ => "Unknown"
      };
    }

    private static string BitmapFontDropdownLabel(int value)
    {
      return ((FontFace)value).ToString();
    }

    public static void SliderLabeled(this Listing_Standard listing, string label, ref int value, int min, int max, int roundTo = -1, string? display = null)
    {
      var floatValue = (float)value;
      SliderLabeled(listing, label, ref floatValue, min, max, roundTo, display);
      value = (int)floatValue;
    }

    public static void SliderLabeled(this Listing_Standard listing, string label, ref float value, float min, float max, float roundTo = -1f, string? display = null)
    {
      var rect = listing.GetRect(Text.LineHeight);

      Widgets.Label(rect.LeftHalf(), label);

      var anchor = Text.Anchor;
      Text.Anchor = TextAnchor.MiddleRight;
      Widgets.Label(rect.RightHalf(), display ?? value.ToString(CultureInfo.InvariantCulture));
      Text.Anchor = anchor;

      value = listing.Slider(value, min, max);
      if (roundTo > 0f) { value = Mathf.Round(value / roundTo) * roundTo; }
    }

    public static void DrawSettings(Rect rect)
    {
      try
      {
        var listingRect = new Rect(rect.x, rect.y + 40f, rect.width, rect.height - 40f);
        var l = new Listing_Settings();
        l.Begin(rect);
        if (l.ButtonText("RimDialogue.ResetToDefault".Translate())) { Reset(); }
        l.End();
        l.BeginScrollView(listingRect, ref _scrollPosition, ref _viewRect);
        l.Label("RimDialogue.SpecialInstructions".Translate());
        Settings.SpecialInstructions.Value = l.TextEntry(Settings.SpecialInstructions.Value, 10);
        var modelNameRow = l.GetRect(40f);
        Widgets.Label(modelNameRow.LeftPart(0.6f), "Model");
        if (Widgets.ButtonText(modelNameRow.RightPart(0.4f), Settings.ModelName.Value))
        {
          List<FloatMenuOption> options = new List<FloatMenuOption>();
          options.Add(new FloatMenuOption("Default", delegate
          {
            Settings.ModelName.Value = "Default";
          }));
          if (Mod.LoginData.models != null)
          {
            foreach (string model in Mod.LoginData.models.OrderBy(model => model))
            {
              options.Add(new FloatMenuOption(model, delegate
              {
                Settings.ModelName.Value = model;
              }));
            }
          }
          Find.WindowStack.Add(new FloatMenu(options));
        }
        l.Label("RimDialogue.ClientId".Translate());
        Settings.ClientId.Value = l.TextEntry(Settings.ClientId.Value, 1);
        l.Label("RimDialogue.ServerUrl".Translate());
        if (string.IsNullOrWhiteSpace(Settings.ServerUrl.Value))
          Settings.ServerUrl.Value = "http://rimdialogue.proceduralproducts.com/home/getdialogue";
        Settings.ServerUrl.Value = l.TextEntry(Settings.ServerUrl.Value, 1);

        var bitmapFontRow = l.GetRect(40f);
        Widgets.Label(bitmapFontRow.LeftPart(0.8f), "BitmapFont");
        Widgets.Dropdown<int, int>(
            bitmapFontRow.RightPart(0.2f),
            Settings.BitmapFont.Value,
            val => val,
            val => new List<DropdownMenuElement<int>>
            {
              new DropdownMenuElement<int> { option = new FloatMenuOption(FontFace.Calibri.ToString(), () => Settings.BitmapFont.Value = (int)FontFace.Calibri), payload = (int)FontFace.Calibri },
              new DropdownMenuElement<int> { option = new FloatMenuOption(FontFace.NotoSansSC.ToString(), () => Settings.BitmapFont.Value = (int)FontFace.NotoSansSC), payload = (int)FontFace.NotoSansSC },
            },
            BitmapFontDropdownLabel(Settings.BitmapFont.Value)
        );

        //Widgets.Dropdown<int, int>(
        //    modelNameRow.RightPart(0.2f),
        //    Settings.ModelName.Value,
        //    val => val,
        //    val => Mod.LoginData.Models.Select((modelName, index) => new DropdownMenuElement<string>
        //    {
        //      option = new FloatMenuOption(modelName ?? "Unknown",
        //      () => Settings.ModelName.Value = modelName),
        //      payload = modelName
        //    }).ToList(),
        //    Settings.ModelName.Value
        //);

        l.Gap();

        var messageListing = l.BeginSection(30f * 7);
        messageListing.ColumnWidth = listingRect.width - 50f;
        messageListing.Label("RimDialogue.DialogueMessages".Translate());
        messageListing.Gap();
        messageListing.Indent();
        var row = messageListing.GetRect(30f);
        Widgets.Label(row.LeftPart(0.8f), "Interface");
        Widgets.Dropdown<int, int>(
            row.RightPart(0.2f),                            
            Settings.DialogueMessageInterface.Value,       
            val => val,                                    
            val => new List<DropdownMenuElement<int>>      
            {
              new DropdownMenuElement<int> { option = new FloatMenuOption("Moveable", () => Settings.DialogueMessageInterface.Value = 1), payload = 1 },
              new DropdownMenuElement<int> { option = new FloatMenuOption("Top Center", () => Settings.DialogueMessageInterface.Value = 2), payload = 2 },
              new DropdownMenuElement<int> { option = new FloatMenuOption("None", () => Settings.DialogueMessageInterface.Value = 3), payload = 3 }
            },
            InterfaceDropdownLabel(Settings.DialogueMessageInterface.Value)
        );
        if (Settings.DialogueMessageInterface.Value == 1)
          messageListing.SliderLabeled("RimDialogue.MessageScrollSpeed".Translate(), ref Settings.MessageScrollSpeed.Value, 1, 50);
        if (Settings.DialogueMessageInterface.Value == 2)
        {
          messageListing.SliderLabeled("RimDialogue.DialogueMessageWidth".Translate(), ref Settings.DialogueMessageWidth.Value, 200, 1200);
          messageListing.SliderLabeled("RimDialogue.DialogueMessageLifetime".Translate(), ref Settings.DialogueMessageLifetime.Value, 1f, 100f, roundTo: 1f);
          messageListing.SliderLabeled("RimDialogue.MinDialogueMessageLifetime".Translate(), ref Settings.MinDialogueMessageLifetime.Value, 1f, 10f, roundTo: 1f);
        }
        messageListing.Outdent();
        l.EndSection(messageListing);

        l.Gap();

        var dialogueListing = l.BeginSection(30f * 15);
        dialogueListing.ColumnWidth = listingRect.width - 50f;
        dialogueListing.Label("RimDialogue.DialogueSettings".Translate());
        dialogueListing.Indent();
        dialogueListing.CheckboxLabeled("RimDialogue.ShowInteractionBubbles".Translate(), ref Settings.ShowInteractionBubbles.Value);
        dialogueListing.CheckboxLabeled("RimDialogue.ShowDialogueBubbles".Translate(), ref Settings.ShowDialogueBubbles.Value);
        dialogueListing.CheckboxLabeled("RimDialogue.EnableCaravans".Translate(), ref Settings.EnableCaravans.Value);
        dialogueListing.CheckboxLabeled("RimDialogue.VerboseLogging".Translate(), ref Settings.VerboseLogging.Value);
        dialogueListing.SliderLabeled("RimDialogue.MaxWords".Translate(), ref Settings.MaxWords.Value, 1, 100);
        dialogueListing.SliderLabeled("RimDialogue.MinWords".Translate(), ref Settings.MinWords.Value, 1, 100);
        dialogueListing.SliderLabeled("RimDialogue.MaxConversationsStored".Translate(), ref Settings.MaxConversationsStored.Value, 0, 100);
        dialogueListing.SliderLabeled("RimDialogue.MinDelayMinutesAll".Translate(), ref Settings.MinDelayMinutesAll.Value, 0, 60);
        dialogueListing.SliderLabeled("RimDialogue.MinDelayMinutes".Translate(), ref Settings.MinDelayMinutes.Value, 0, 60);
        dialogueListing.SliderLabeled("RimDialogue.MinTimeBetweenConversations".Translate(), ref Settings.MinTimeBetweenConversations.Value, 0, 60);
        dialogueListing.SliderLabeled("RimDialogue.DeepTalkCompensationFactor".Translate(), ref Settings.DeepTalkCompensationFactor.Value, 1, 100);
        dialogueListing.CheckboxLabeled("RimDialogue.OnlyColonists".Translate(), ref Settings.OnlyColonists.Value);
        dialogueListing.Outdent();
        l.EndSection(dialogueListing);

        l.Gap();


        var weights = l.BeginSection(45f * 20f + 50f);
        weights.ColumnWidth = listingRect.width - 50f;
        weights.Label("RimDialogue.DialogueWeights".Translate());
        weights.Indent();
        if (l.ButtonText("RimDialogue.ToggleAllChitchatWeights".Translate())) { ToggleAllChitChatWeights(); }
        weights.Gap();
        weights.SliderLabeled("RimDialogue.TimelyEventWeight".Translate(), ref Settings.TimelyEventWeight.Value, 0, 10, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.MessageChitChatWeight".Translate(), ref Settings.MessageChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.GameConditionChitChatWeight".Translate(), ref Settings.GameConditionChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.BattleChitChatWeight".Translate(), ref Settings.BattleChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.RecentIncidentChitChatWeight".Translate(), ref Settings.RecentIncidentChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.AlertChitChatWeight".Translate(), ref Settings.AlertChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.SameIdeologyChitChatWeight".Translate(), ref Settings.SameIdeologyChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.SkillsChitChatWeight".Translate(), ref Settings.SkillChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.ColonistChitChatWeight".Translate(), ref Settings.ColonistChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.HealthChitChatWeight".Translate(), ref Settings.HealthChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.ApparelChitChatWeight".Translate(), ref Settings.ApparelChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.NeedChitChatWeight".Translate(), ref Settings.NeedChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.FamilyChitChatWeight".Translate(), ref Settings.FamilyChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.WeatherChitChatWeight".Translate(), ref Settings.WeatherChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.FactionChitChatWeight".Translate(), ref Settings.FactionChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.WeaponChitChatWeight".Translate(), ref Settings.WeaponChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.AppearanceChitChatWeight".Translate(), ref Settings.AppearanceChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.AnimalChitChatWeight".Translate(), ref Settings.AnimalChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.RoomChitChatWeight".Translate(), ref Settings.RoomChitChatWeight.Value, 0, 1, roundTo: .01f);
        weights.SliderLabeled("RimDialogue.DeadColonistDeepTalk".Translate(), ref Settings.DeadColonistWeight.Value, 0, 1, roundTo: .01f);
        weights.Outdent();
        l.EndSection(weights);

        var chances = l.BeginSection(45f * 5f + 50f);
        chances.ColumnWidth = listingRect.width - 50f;
        chances.Label("RimDialogue.ChanceOf".Translate());
        chances.Indent();
        chances.Gap();
        chances.SliderLabeled("RimDialogue.MeleeCombatQuipChance".Translate(), ref Settings.MeleeCombatQuipChance.Value, 0, 1, roundTo: .01f);
        chances.SliderLabeled("RimDialogue.RangedFireQuipChance".Translate(), ref Settings.RangedFireQuipChance.Value, 0, 1, roundTo: .01f);
        chances.SliderLabeled("RimDialogue.RangedImpactQuipChance".Translate(), ref Settings.RangedImpactQuipChance.Value, 0, 1, roundTo: .01f);
        chances.SliderLabeled("RimDialogue.DamageTakenQuipChance".Translate(), ref Settings.DamageTakenQuipChance.Value, 0, 1, roundTo: .01f);
        chances.SliderLabeled("RimDialogue.ThoughtChance".Translate(), ref Settings.ThoughtChance.Value, 0, 1, roundTo: .01f);

        chances.Outdent();
        l.EndSection(chances);
        l.EndScrollView(ref _viewRect);
      }
      catch(Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    private static void Reset()
    {
      _colorBuffer = new string[4];

      Settings.Reset();
    }

    private static void ToggleAllChitChatWeights()
    {
      Settings.ToggleAllChitChatWeights();
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
