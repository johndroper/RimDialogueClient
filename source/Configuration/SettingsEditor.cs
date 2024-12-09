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

      l.CheckboxLabeled("RimDialogue.DoNonPlayer".Translate(), ref Settings.DoNonPlayer.Value);
      l.CheckboxLabeled("RimDialogue.DoAnimals".Translate(), ref Settings.DoAnimals.Value);
      l.CheckboxLabeled("RimDialogue.DoDrafted".Translate(), ref Settings.DoDrafted.Value);
      var doTextColors = Settings.DoTextColors.Value;
      l.CheckboxLabeled("RimDialogue.DoTextColors".Translate(), ref Settings.DoTextColors.Value);
      if (doTextColors != Settings.DoTextColors.Value) { Bubbler.Rebuild(); }
      l.Gap();

      l.SliderLabeled("RimDialogue.AutoHideSpeed".Translate(), ref Settings.AutoHideSpeed.Value, 1, 4, display: Settings.AutoHideSpeed.Value == Settings.AutoHideSpeedDisabled ? "RimDialogue.AutoHideSpeedOff".Translate().ToString() : Settings.AutoHideSpeed.Value.ToString());

      l.SliderLabeled("RimDialogue.AltitudeBase".Translate(), ref Settings.AltitudeBase.Value, 3, 44);
      l.SliderLabeled("RimDialogue.AltitudeMax".Translate(), ref Settings.AltitudeMax.Value, 20, 60);
      l.SliderLabeled("RimDialogue.ScaleMax".Translate(), ref Settings.ScaleMax.Value, 1f, 5f, 0.05f, Settings.ScaleMax.Value.ToStringPercent());
      l.SliderLabeled("RimDialogue.PawnMax".Translate(), ref Settings.PawnMax.Value, 1, 15);

      l.SliderLabeled("RimDialogue.FontSize".Translate(), ref Settings.FontSize.Value, 5, 30);
      l.SliderLabeled("RimDialogue.PaddingX".Translate(), ref Settings.PaddingX.Value, 1, 40);
      l.SliderLabeled("RimDialogue.PaddingY".Translate(), ref Settings.PaddingY.Value, 1, 40);
      l.SliderLabeled("RimDialogue.WidthMax".Translate(), ref Settings.WidthMax.Value, 100, 500, 4);

      l.SliderLabeled("RimDialogue.OffsetSpacing".Translate(), ref Settings.OffsetSpacing.Value, 2, 12);
      l.SliderLabeled("RimDialogue.OffsetStart".Translate(), ref Settings.OffsetStart.Value, 0, 400, 2);

      var offsetDirection = Settings.OffsetDirection.Value.AsInt;
      l.SliderLabeled("RimDialogue.OffsetDirection".Translate(), ref offsetDirection, 0, 3, display: "RimDialogue.OffsetDirections".Translate().ToString().Split('|').ElementAtOrDefault(offsetDirection));
      Settings.OffsetDirection.Value = new Rot4(offsetDirection);

      l.SliderLabeled("RimDialogue.OpacityStart".Translate(), ref Settings.OpacityStart.Value, 0.3f, 1f, 0.05f, Settings.OpacityStart.Value.ToStringPercent());
      l.SliderLabeled("RimDialogue.OpacityHover".Translate(), ref Settings.OpacityHover.Value, 0.05f, 1f, 0.05f, Settings.OpacityHover.Value.ToStringPercent());
      l.SliderLabeled("RimDialogue.FadeStart".Translate(), ref Settings.FadeStart.Value, 100, 5000, 50);
      l.SliderLabeled("RimDialogue.FadeLength".Translate(), ref Settings.FadeLength.Value, 50, 2500, 50);

      l.ColorEntry("RimDialogue.Background".Translate(), ref _colorBuffer[0], ref Settings.Background.Value);
      l.ColorEntry("RimDialogue.Foreground".Translate(), ref _colorBuffer[1], ref Settings.Foreground.Value);
      l.ColorEntry("RimDialogue.BackgroundSelected".Translate(), ref _colorBuffer[2], ref Settings.SelectedBackground.Value);
      l.ColorEntry("RimDialogue.ForegroundSelected".Translate(), ref _colorBuffer[3], ref Settings.SelectedForeground.Value);

      l.SliderLabeled("RimDialogue.MaxWords".Translate(), ref Settings.MaxWords.Value, 1, 100);

      l.SliderLabeled("RimDialogue.MaxSpeed".Translate(), ref Settings.MaxSpeed.Value, 1, 4);

      l.SliderLabeled("RimDialogue.MaxConversationsStored".Translate(), ref Settings.MaxConversationsStored.Value, 0, 100);

      l.Label("RimDialogue.SpecialInstructions".Translate());
      Settings.SpecialInstructions.Value = l.TextEntry(Settings.SpecialInstructions.Value, 10);

      l.Label("RimDialogue.ClientId".Translate());
      Settings.ClientId.Value = l.TextEntry(Settings.ClientId.Value, 1);

      l.Label("RimDialogue.ServerUrl".Translate());
      Settings.ServerUrl.Value = l.TextEntry(Settings.ServerUrl.Value, 1);

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
