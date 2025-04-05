#nullable enable

using RimWorld;
using System;
using System.Collections.Generic;
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
      var tracker = Current.Game.GetComponent<GameComponent_ConversationTracker>();
      float y = 0;

      Text.Font = GameFont.Medium;
      Widgets.Label(new Rect(0, y, inRect.width - 300f, 30f), "RimDialogue");

      Text.Font = GameFont.Small;
      // Filter Dropdown
      var filterRect = new Rect(inRect.width - 300f, y, 300f, 30f);
      var filterButtonText = filterMode switch
      {
        FilterMode.All => allPawnsText,
        FilterMode.Colonist => colonistsText,
        FilterMode.Pawn => selectedPawn?.Name.ToStringShort ?? throw new Exception("selectedPawn is null"),
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

        options.AddRange(Find.CurrentMap.mapPawns.FreeColonists.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
          new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
          {
            selectedPawn = pawn;
            filterMode = FilterMode.Pawn;
          })
        ));
        options.AddRange(Find.CurrentMap.mapPawns.PrisonersOfColony.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
          new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
          {
            selectedPawn = pawn;
            filterMode = FilterMode.Pawn;
          })
        ));
        options.AddRange(Find.CurrentMap.mapPawns.SpawnedColonyAnimals.OrderBy(pawn => pawn?.Name?.ToStringShort ?? unknownText).Select(pawn =>
          new FloatMenuOption(pawn?.Name?.ToStringShort ?? unknownText, () =>
          {
            selectedPawn = pawn;
            filterMode = FilterMode.Pawn;
          })
        ));
        Find.WindowStack.Add(new FloatMenu(options));
      }
      y += 50f;
      float colY = y;
      float leftColWidth = inRect.width * 0.4f - 10;
      float rightColWidth = inRect.width - leftColWidth - 20;

      var instructionsLabelRect = new Rect(0, y, leftColWidth, 25f);
      var instructionsLabel = filterMode switch
      {
        FilterMode.All => additionalInstructionsText.Replace("[pawn]", allPawnsText),
        FilterMode.Colonist => additionalInstructionsText.Replace("[pawn]", colonistsText),
        FilterMode.Pawn => additionalInstructionsText.Replace("[pawn]", selectedPawn != null ? selectedPawn.Name.ToStringShort : unknownText),
        _ => additionalInstructionsText.Replace("[pawn]", unknownText)
      };

      Widgets.Label(instructionsLabelRect, instructionsLabel);
      
      y += 30f;

      Rect instructionsScrollRect = new Rect(0, y, leftColWidth, inRect.height - y - 10);
      Rect instructionsContentRect = new Rect(0, 0, leftColWidth - 16f, 2000);
      Widgets.BeginScrollView(instructionsScrollRect, ref instructionsScrollPosition, instructionsContentRect);

      switch (filterMode)
      {
        case FilterMode.All:
          tracker.AddAdditionalInstructions(InstructionsSet.ALL_PAWNS,
            Widgets.TextArea(instructionsContentRect, tracker.GetInstructions(InstructionsSet.ALL_PAWNS)));
          TooltipHandler.TipRegion(instructionsScrollRect, allPawnsTip);
          break;
        case FilterMode.Colonist:
          tracker.AddAdditionalInstructions(InstructionsSet.COLONISTS,
            Widgets.TextArea(instructionsContentRect, tracker.GetInstructions(InstructionsSet.COLONISTS)));
          TooltipHandler.TipRegion(instructionsScrollRect, colonistsTip);
          break;
        case FilterMode.Pawn:
          if (selectedPawn == null)
            throw new InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
          tracker.AddAdditionalInstructions(selectedPawn, Widgets.TextArea(instructionsContentRect, tracker.GetInstructions(selectedPawn)));
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
            throw new System.InvalidOperationException("SelectedPawn is null but filter mode is set to SelectedPawn.");
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
        convoY += topMargin;
        var headerRect = new Rect(0, convoY, conversationContentRectWidth, 25f);
        Widgets.Label(headerRect, conversation.Participants);
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
      Widgets.EndScrollView();
    }
  }
}
