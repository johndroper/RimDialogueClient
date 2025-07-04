#nullable enable

using RimDialogue.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

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

    // Set the initial window size
    public override Vector2 RequestedTabSize => new Vector2(800f, 400f);

    // Main UI rendering
    public override void DoWindowContents(Rect inRect)
    {
      if (Find.CurrentMap == null || !Find.CurrentMap.mapPawns.FreeColonists.Any())
        return;

      var tracker = Current.Game.GetComponent<GameComponent_ConversationTracker>();
      float y = 0;

      Text.Font = GameFont.Medium;
      
      var titleText = "RimDialogue";
      var titleSize = Text.CalcSize(titleText);
      var titleRect = new Rect(0, y, titleSize.x, 30f);

      Widgets.Label(titleRect, titleText);

      var discordRect = new Rect(titleRect.x + titleRect.width + 3, titleRect.y, 24, 24);
      if (Widgets.ButtonImage(discordRect, DialogueMessageWindow.DiscordLogo, true, "Discord - Get Help, Give Feedback, Post Memes"))
      {
        Process.Start(new ProcessStartInfo
        {
          FileName = "https://discord.gg/KavBmswUen",
          UseShellExecute = true
        });
      }
      Text.Font = GameFont.Small;
      const float filterButtonWidth = 100f;

      var filterButtonLabelText = "RimDialogue.FilterButtonLabel".Translate();
      var filterButtonLabelSize = Text.CalcSize(filterButtonLabelText);

      var filterButtonLabelRect = new Rect(inRect.width - filterButtonWidth - filterButtonLabelSize.x, titleRect.y, filterButtonLabelSize.x, 30f);
      Widgets.Label(filterButtonLabelRect, filterButtonLabelText);

      // Filter Dropdown
      var filterRect = new Rect(inRect.width - filterButtonWidth, y, filterButtonWidth, 30f);
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
          })
        };
        options.Add(
          new FloatMenuOption(colonistsText, () =>
          {
            selectedPawn = null;
            filterMode = FilterMode.Colonist;
          }));
        try
        {
          options.AddRange(Find.CurrentMap.mapPawns.FreeColonists.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
            new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
            {
              selectedPawn = pawn;
              filterMode = FilterMode.Pawn;
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
      y += 50f;
      float colY = y;
      float leftColWidth = inRect.width * 0.4f - 10;
      float rightColWidth = inRect.width - leftColWidth - 20;

      string instructionsLabel = "RimDialogue.Unknown".Translate();
      string instructions = "RimDialogue.Unknown".Translate();
      switch (filterMode)
      {
        case FilterMode.All:
          instructions = tracker.GetInstructions(InstructionsSet.ALL_PAWNS);
          instructionsLabel = additionalInstructionsText.Replace("[pawn]", allPawnsText) + $" ({instructions.Length} {"RimDialogue.Characters".Translate()})";
          break;
        case FilterMode.Colonist:
          instructions = tracker.GetInstructions(InstructionsSet.COLONISTS);
          instructionsLabel = additionalInstructionsText.Replace("[pawn]", colonistsText) + $" ({instructions.Length} {"RimDialogue.Characters".Translate()})";
          break;
        case FilterMode.Pawn:
          if (selectedPawn == null)
            throw new InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
          instructions = tracker.GetInstructions(selectedPawn);
          instructionsLabel = additionalInstructionsText.Replace("[pawn]", selectedPawn != null ? selectedPawn.Name.ToStringShort : unknownText) + $" ({instructions.Length} {"RimDialogue.Characters".Translate()})";
          break;
      }

      var instructionsLabelRect = new Rect(0, y, leftColWidth, 25f);
      Widgets.Label(instructionsLabelRect, instructionsLabel);

      y += 30f;

      Rect instructionsScrollRect = new Rect(0, y, leftColWidth, inRect.height - y - 10);
      Rect instructionsContentRect = new Rect(0, 0, leftColWidth - 16f, 2000);
      Widgets.BeginScrollView(instructionsScrollRect, ref instructionsScrollPosition, instructionsContentRect);

      switch (filterMode)
      {
        case FilterMode.All:
          tracker.AddAdditionalInstructions(InstructionsSet.ALL_PAWNS,
            Widgets.TextArea(instructionsContentRect, instructions));
          TooltipHandler.TipRegion(instructionsScrollRect, allPawnsTip);
          break;
        case FilterMode.Colonist:
          tracker.AddAdditionalInstructions(InstructionsSet.COLONISTS,
            Widgets.TextArea(instructionsContentRect, instructions));
          TooltipHandler.TipRegion(instructionsScrollRect, colonistsTip);
          break;
        case FilterMode.Pawn:
          if (selectedPawn == null)
            throw new InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
          tracker.AddAdditionalInstructions(selectedPawn, Widgets.TextArea(instructionsContentRect, instructions));
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
      Conversation[] filteredConversations;
      switch (filterMode)
      {
        case FilterMode.Colonist:
          filteredConversations = tracker.Conversations
            .Where(c => c.InvolvesColonist())
            .OrderByDescending(c => c.timestamp)
            .ToArray();
          break;
        case FilterMode.Pawn:
          if (selectedPawn == null)
            selectedPawn = Find.CurrentMap.mapPawns.FreeColonists.FirstOrDefault();
          filteredConversations = tracker.Conversations
            .Where(c => c.InvolvesPawn(selectedPawn))
            .OrderByDescending(c => c.timestamp)
            .ToArray();
          break;
        default:
          filteredConversations = tracker.Conversations.OrderByDescending(c => c.timestamp).ToArray();
          break;
      }
      float conversationContentRectWidth = conversationsScrollRect.width - 16f;
      const float labelHeight = 20f;
      const float topMargin = 15f;
      const float bottomMargin = 4f;
      float conversationContentRectHeight = filteredConversations.Sum(conversation => topMargin + labelHeight + (conversation.timestamp != null ? labelHeight : 0) + Text.CalcHeight(conversation.text, conversationContentRectWidth) + bottomMargin);
      var conversationContentRect = new Rect(0, 0, conversationContentRectWidth, conversationContentRectHeight);
      Widgets.BeginScrollView(conversationsScrollRect, ref conversationScrollPosition, conversationContentRect);
      float convoY = 0;
      foreach (var conversation in filteredConversations)
      {
        try
        {
          convoY += topMargin;
          var headerRect = new Rect(0, convoY, conversationContentRectWidth, 25f);
          Widgets.Label(headerRect, conversation.Participants);
          var memeButtonRect = new Rect(headerRect.width - 40, convoY, 40, headerRect.height);
          if (Widgets.ButtonText(memeButtonRect, "Save"))
          {
            var bitmapFont = BitmapFont.Get((FontFace)Settings.BitmapFont.Value);
            Find.WindowStack.Add(new Window_ComicPanelViewer(bitmapFont, conversation));
          }
          convoY += labelHeight;
          if (conversation.timestamp != null)
          {
            var periodRect = new Rect(0, convoY, conversationContentRectWidth, 25f);
            Widgets.Label(periodRect, (Find.TickManager.TicksGame - conversation.timestamp ?? 0).ToStringTicksToPeriod() + agoText);
            convoY += labelHeight;
          }
          string displayText = conversation.text ?? string.Empty;
          float textHeight = Text.CalcHeight(displayText, conversationContentRectWidth);
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
