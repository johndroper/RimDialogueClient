#nullable enable

using RimDialogue.Core;
using RimDialogue.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimDialogue
{
  public enum FilterMode
  {
    All,
    Colonist,
    Pawn
  }

  public static class InstructionsSet
  {
    public const string ALL_PAWNS = "ALL_PAWNS";
    public const string COLONISTS = "COLONISTS";
  }

  public class MainTabWindow_RimDialogue : MainTabWindow
  {
    public class ConversationUI
    {
      public Conversation Conversation;
      
      public ConversationUI(Conversation conversation)
      {
        Conversation = conversation;
      }

      private float? _height = null;
      private float _lastWidth = -1;
      public float GetHeight(float width)
      {
        if (_lastWidth != width)
        {
          _lastWidth = width;
          _height = null;
        }
        _height ??= Text.CalcHeight(Conversation.Text ?? string.Empty, width);
        return _height ?? 0;
      }

      private string? _period;
      private int _lastTick = 0;
      public string Period
      {
        get
        {
          var now = Find.TickManager.TicksAbs;
          if (_period == null || now - _lastTick > 600)
          {
            _period = (now - Conversation.Timestamp ?? 0).ToStringTicksToPeriod() + agoText;
            _lastTick = now;
          }
          return _period ?? string.Empty;
        }
      }
    }

    private Vector2 conversationScrollPosition = Vector2.zero; // For scrolling the conversation list
    private Vector2 instructionsScrollPosition = Vector2.zero;

    private FilterMode filterMode = FilterMode.All;
    private Pawn? selectedPawn = null;

    private static string additionalInstructionsText = "RimDialogue.AdditionalInstructions".Translate().ToString();
    private static string allPawnsText = "RimDialogue.AllPawns".Translate().ToString();
    private static string colonistsText = "RimDialogue.Colonists".Translate().ToString();
    private static string unknownText = "RimDialogue.Unknown".Translate().ToString();
    private static string allPawnsTip = "RimDialogue.AllPawnsTip".Translate().ToString();
    private static string colonistsTip = "RimDialogue.ColonistsTip".Translate().ToString();
    private static string pawnTip = "RimDialogue.PawnTip".Translate().ToString();
    private static string agoText = "RimDialogue.Ago".Translate().ToString();
    private static string regenerateButtonText = "RimDialogue.RegenerateButton".Translate().ToString();
    private static string filterButtonTip = "RimDialogue.FilterButtonLabel".Translate().ToString();

    public override Vector2 RequestedTabSize => new Vector2(800f, 400f);

    private GameComponent_ConversationTracker Tracker => Current.Game.GetComponent<GameComponent_ConversationTracker>();

    public MainTabWindow_RimDialogue()
    {
      Tracker.ConversationAdded += Tracker_ConversationAdded;
    }
    public override void OnAcceptKeyPressed()
    {
      // Don't do anything on Enter
    }

    private void Tracker_ConversationAdded(object sender, ConversationArgs e)
    {
      _filteredConversations = null;
    }

    const string titleText = "RimDialogue";
    static Vector2? titleSize;
    static Rect? titleRect;
    static Rect? discordRect;

    public override void DoWindowContents(Rect inRect)
    {
      if (Find.CurrentMap == null || !Find.CurrentMap.mapPawns.FreeColonists.Any())
        return;

      float y = 0;

      Text.Font = GameFont.Medium;
      titleSize ??= Text.CalcSize(titleText);
      titleRect ??= new Rect(0, y, titleSize.Value.x, 30f);
      Widgets.Label(titleRect.Value, titleText);

      if (!Settings.HideDiscordButton.Value)
      {
        discordRect ??= new Rect(titleRect.Value.x + titleRect.Value.width + 3, titleRect.Value.y, 24, 24);
        if (Widgets.ButtonImage(discordRect.Value, DialogueMessageWindow.DiscordLogo, true, "Discord - Get Help, Give Feedback, Post Memes"))
        {
          Process.Start(new ProcessStartInfo
          {
            FileName = "https://discord.gg/KavBmswUen",
            UseShellExecute = true
          });
        }
      }

      DrawnPawnTab(inRect, y);

    }

    private ConversationUI[]? _filteredConversations;
    public ConversationUI[] FilteredConversations
    {
      get
      {
        if (_filteredConversations == null)
        {
          switch (filterMode)
          {
            case FilterMode.Colonist:
              _filteredConversations = Tracker.Conversations
                .Where(c => c.InvolvesColonist())
                .OrderByDescending(c => c.Timestamp)
                .Select(c => new ConversationUI(c))
                .ToArray();
              break;
            case FilterMode.Pawn:
              if (selectedPawn == null)
                selectedPawn = Find.CurrentMap.mapPawns.FreeColonists.FirstOrDefault();
              _filteredConversations = Tracker.Conversations
                .Where(c => c.Involves(selectedPawn))
                .OrderByDescending(c => c.Timestamp)
                .Select(c => new ConversationUI(c))
                .ToArray();
              break;
            default:
              _filteredConversations = Tracker.
                Conversations
                .OrderByDescending(c => c.Timestamp)
                .Select(c => new ConversationUI(c))
                .ToArray();
              break;
          }
        }
        return _filteredConversations;
      }
    }

    const float labelHeight = 20f;
    const float topMargin = 15f;
    const float bottomMargin = 4f;

    private float? _conversationContentHeight = null;
    public float GetConversationContentHeight(float conversationContentRectWidth)
    {
      _conversationContentHeight ??= FilteredConversations.
        Sum(ui =>
          topMargin +
          labelHeight +
          (ui.Conversation.Timestamp != null ? labelHeight : 0) +
          Text.CalcHeight(ui.Conversation.Text, conversationContentRectWidth) +
          bottomMargin);
      return _conversationContentHeight ?? 0;
    }

    private Rect lastInRect = Rect.zero;
    private string? instructionsLabel = null;
    const float filterButtonWidth = 100f;
    const float regenerateButtonWidth = 100f;

    public void DrawnPawnTab(Rect inRect, float y)
    {
      Text.Font = GameFont.Small;

      if (inRect != lastInRect)
      {
        _conversationContentHeight = null;
        lastInRect = inRect;
      }

      string instructions;
      switch (filterMode)
      {
        case FilterMode.Colonist:
          instructions = Tracker.GetInstructions(InstructionsSet.COLONISTS);
          break;
        case FilterMode.Pawn:
          if (selectedPawn == null)
            throw new InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
          instructions = Tracker.GetInstructions(selectedPawn);
          break;
        default:
          instructions = Tracker.GetInstructions(InstructionsSet.ALL_PAWNS);
          break;
      }

      // Filter Dropdown
      var filterRect = new Rect(
        inRect.width - filterButtonWidth - regenerateButtonWidth,
        y,
        filterButtonWidth,
        30f);
      var filterButtonText = filterMode switch
      {
        FilterMode.All => allPawnsText,
        FilterMode.Colonist => colonistsText,
        FilterMode.Pawn => selectedPawn?.Name?.ToStringShort ?? unknownText,
        _ => allPawnsText
      };

      if (Widgets.ButtonText(filterRect, filterButtonText))
      {
        List<FloatMenuOption> options = new List<FloatMenuOption>
        {
          new FloatMenuOption(allPawnsText, () =>
          {
            selectedPawn = null;
            filterMode = FilterMode.All;
            instructionsLabel = additionalInstructionsText.Replace("[pawn]", allPawnsText) + $" ({instructions.Length} {"RimDialogue.Characters".Translate()})";
            _filteredConversations = null;
          })
        };
        options.Add(
          new FloatMenuOption(colonistsText, () =>
          {
            selectedPawn = null;
            filterMode = FilterMode.Colonist;
            instructionsLabel = additionalInstructionsText.Replace("[pawn]", colonistsText) + $" ({instructions.Length} {"RimDialogue.Characters".Translate()})";
            _filteredConversations = null;
          }));
        try
        {
          options.AddRange(Find.CurrentMap.mapPawns.FreeColonists.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
            new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
            {
              selectedPawn = pawn;
              filterMode = FilterMode.Pawn;
              instructionsLabel = additionalInstructionsText.Replace("[pawn]", pawn?.Name?.ToStringShort) + $" ({instructions.Length} {"RimDialogue.Characters".Translate()})";
              _filteredConversations = null;
            })
          ));
        }
        catch (Exception ex)
        {
          Log.ErrorOnce($"Failed to get colonists for filter menu: {ex.Message}", 87985);
        }

        try
        {
          options.AddRange(Find.CurrentMap.mapPawns.PrisonersOfColony.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
            new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
            {
              selectedPawn = pawn;
              filterMode = FilterMode.Pawn;
            })
          ));
        }
        catch (Exception ex)
        {
          Log.ErrorOnce($"Failed to get prisoners for filter menu: {ex.Message}", 87984);
        }

        try
        {
          options.AddRange(Find.CurrentMap.mapPawns.SpawnedColonyAnimals.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
            new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
            {
              selectedPawn = pawn;
              filterMode = FilterMode.Pawn;
            })
          ));
        }
        catch (Exception ex)
        {
          Log.ErrorOnce($"Failed to get animals for filter menu: {ex.Message}", 87983);
        }
        Find.WindowStack.Add(new FloatMenu(options));
      }
            
      TooltipHandler.TipRegion(filterRect, filterButtonTip);

      if (filterMode == FilterMode.Pawn || filterMode == FilterMode.Colonist)
      {
        //Regenerate Instructions Button
        
        var regenerateRect = new Rect(inRect.width - regenerateButtonWidth, y, regenerateButtonWidth, 30f);
        if (Widgets.ButtonText(regenerateRect, regenerateButtonText))
        {
          Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
            "RimDialogue.AreYouSure".Translate(),
            () =>
            {
              switch (filterMode)
              {
                case FilterMode.All:
                  break;
                case FilterMode.Colonist:
                  Tracker.GetScenarioInstructions(Find.Scenario?.name + "\r\n" + H.RemoveWhiteSpace(Find.Scenario?.description));
                  break;
                case FilterMode.Pawn:
                  if (selectedPawn == null)
                    throw new InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
                  Tracker.GetInstructionsAsync(selectedPawn.MakeData(null, -1));
                  break;
              }
            },
            true));
        }
        var regenerateButtonTip = "RimDialogue.RegenerateButtonTip".Translate();
        TooltipHandler.TipRegion(regenerateRect, regenerateButtonTip);
      }

      //y + titleRect.height + 5f

      y += 50f;
      float colY = y;
      float leftColWidth = inRect.width * 0.4f - 10;
      float rightColWidth = inRect.width - leftColWidth - 20;

      var instructionsLabelRect = new Rect(0, y, leftColWidth, 25f);
      Widgets.Label(instructionsLabelRect, instructionsLabel);

      y += 30f;

      Rect instructionsScrollRect = new Rect(0, y, leftColWidth, inRect.height - y - 10);
      Rect instructionsContentRect = new Rect(0, 0, leftColWidth - 16f, 2000);
      Widgets.BeginScrollView(instructionsScrollRect, ref instructionsScrollPosition, instructionsContentRect);

      switch (filterMode)
      {
        case FilterMode.All:
          Tracker.AddAdditionalInstructions(
            InstructionsSet.ALL_PAWNS,
            Widgets.TextArea(instructionsContentRect, instructions));
          TooltipHandler.TipRegion(instructionsScrollRect, allPawnsTip);
          break;
        case FilterMode.Colonist:
          Tracker.AddAdditionalInstructions(
            InstructionsSet.COLONISTS,
            Widgets.TextArea(instructionsContentRect, instructions));
          TooltipHandler.TipRegion(instructionsScrollRect, colonistsTip);
          break;
        case FilterMode.Pawn:
          if (selectedPawn == null)
            throw new InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
          Tracker.AddAdditionalInstructions(selectedPawn, Widgets.TextArea(instructionsContentRect, instructions));
          TooltipHandler.TipRegion(instructionsScrollRect, pawnTip);
          break;
      }

      y += 120f;
      Widgets.EndScrollView();
      Widgets.DrawLineVertical(leftColWidth + 10, colY, inRect.height);

      // Conversation List
      y = colY;
      float x = leftColWidth + 20;
      Widgets.Label(new Rect(x, y, rightColWidth, 25f), "RimDialogue.Conversations".Translate());
      y += 30f;
      var conversationsScrollRect = new Rect(x, y, rightColWidth, inRect.height - y - 10);
      float conversationContentRectWidth = conversationsScrollRect.width - 16f;
      float conversationContentRectHeight = GetConversationContentHeight(conversationContentRectWidth);
      var conversationContentRect = new Rect(0, 0, conversationContentRectWidth, conversationContentRectHeight);
      Widgets.BeginScrollView(conversationsScrollRect, ref conversationScrollPosition, conversationContentRect);
      float convoY = 0;
      foreach (var ui in FilteredConversations)
      {
        try
        {
          convoY += topMargin;
          var headerRect = new Rect(0, convoY, conversationContentRectWidth, 25f);
          Widgets.Label(headerRect, ui.Conversation.Participants);
          var memeButtonRect = new Rect(headerRect.width - 40, convoY, 40, headerRect.height);
          if (Widgets.ButtonText(memeButtonRect, "Save"))
          {
            var bitmapFont = BitmapFont.Get((FontFace)Settings.BitmapFont.Value);
            Find.WindowStack.Add(new Window_ComicPanelViewer(bitmapFont, ui.Conversation));
          }
          var copyButtonRect = new Rect(headerRect.width - 90, convoY, 40, headerRect.height);
          if (Widgets.ButtonText(copyButtonRect, "Copy"))
          {
            GUIUtility.systemCopyBuffer = ui.Conversation.Text ?? string.Empty;
            SoundDefOf.Click.PlayOneShotOnCamera();
          }
          convoY += labelHeight;
          if (ui.Conversation.Timestamp != null)
          {
            var periodRect = new Rect(0, convoY, conversationContentRectWidth, 25f);
            Widgets.Label(periodRect, ui.Period);
            convoY += labelHeight;
          }
          string displayText = ui.Conversation.FormattedText ?? string.Empty;
          float textHeight = ui.GetHeight(conversationContentRectWidth);
          var convoRect = new Rect(0, convoY, conversationContentRectWidth, textHeight);
          Widgets.Label(convoRect, displayText);
          convoY += textHeight;
          convoY += bottomMargin;
          Color previousColor = GUI.color;
          GUI.color = Widgets.SeparatorLineColor;
          Widgets.DrawLineHorizontal(0, convoY + bottomMargin, conversationContentRectWidth);
          GUI.color = previousColor;
        }
        catch (Exception ex)
        {
          Log.ErrorOnce($"Failed to draw conversation: {ex.Message}", 87987);
        }
      }
      Widgets.EndScrollView();
    }
  }
}
