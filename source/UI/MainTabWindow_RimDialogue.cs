using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimDialogue
{
  public class MainTabWindow_RimDialogue : MainTabWindow
  {
    private Vector2 scrollPosition = Vector2.zero; // For scrolling the conversation list
    private Pawn selectedPawn = null; // Filtered pawn

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
      if (Widgets.ButtonText(filterRect, selectedPawn == null ? "All Pawns" : selectedPawn.Name.ToStringShort))
      {
        List<FloatMenuOption> options = new List<FloatMenuOption>
        {
            new FloatMenuOption("All Pawns", () => selectedPawn = null)
        };
        options.AddRange(Find.CurrentMap.mapPawns.FreeColonists.OrderBy(pawn => pawn?.Name?.ToStringShort ?? "Unknown").Select(pawn =>
            new FloatMenuOption(pawn?.Name?.ToStringShort ?? "Unknown", () => selectedPawn = pawn)
        ));
        options.AddRange(Find.CurrentMap.mapPawns.PrisonersOfColony.OrderBy(pawn => pawn?.Name?.ToStringShort ?? "Unknown").Select(pawn =>
          new FloatMenuOption(pawn?.Name?.ToStringShort ?? "Unknown", () => selectedPawn = pawn)
        ));
        options.AddRange(Find.CurrentMap.mapPawns.SpawnedColonyAnimals.OrderBy(pawn => pawn?.Name?.ToStringShort ?? "Unknown").Select(pawn =>
          new FloatMenuOption(pawn?.Name?.ToStringShort ?? "Unknown", () => selectedPawn = pawn)
        ));
        Find.WindowStack.Add(new FloatMenu(options));
      }
      y += 50f;
      float colY = y;
      float leftColWidth = inRect.width * 0.333f - 10;
      float rightColWidth = inRect.width - leftColWidth - 20;

      // Additional Instructions
      Widgets.Label(new Rect(0, y, leftColWidth, 25f), "Additional Instructions");
      y += 30f;
      Rect textAreaRect = new Rect(0, y, leftColWidth, inRect.height - y - 10); 
      tracker.AddAdditionalInstructions(selectedPawn, Widgets.TextArea(textAreaRect, tracker.GetAdditionalInstructions(selectedPawn)));
      y += 120f;

      Widgets.DrawLineVertical(leftColWidth + 10, colY, inRect.height);
      
      // Conversation List
      y = colY;
      float x = leftColWidth + 20;
      Widgets.Label(new Rect(x, y, rightColWidth, 25f), "Conversations");
      y += 30f;
      var scrollRect = new Rect(x, y, rightColWidth, inRect.height - y - 10);
      var filteredConversations = tracker.Conversations.Where(c => selectedPawn == null || c.InvolvesPawn(selectedPawn)).ToArray();
      float contentRectWidth = scrollRect.width - 16f;
      const float labelHeight = 30f;
      const float topMargin = 15f;
      const float bottomMargin = 4f;
      float contentRectHeight = filteredConversations.Sum(conversation => topMargin + labelHeight + Text.CalcHeight(conversation.text, contentRectWidth) + bottomMargin);
      var contentRect = new Rect(0, 0, contentRectWidth, contentRectHeight);
      Widgets.BeginScrollView(scrollRect, ref scrollPosition, contentRect);
      float convoY = 0;
      foreach (var conversation in filteredConversations)
      {
        convoY += topMargin;
        var headerRect = new Rect(0, convoY, contentRect.width, 25f);
        Widgets.Label(headerRect, conversation.Participants);
        convoY += labelHeight;
        string displayText = conversation.text;
        float textHeight = Text.CalcHeight(displayText, contentRect.width);
        var convoRect = new Rect(0, convoY, contentRect.width, textHeight);
        Widgets.Label(convoRect, displayText);
        convoY += textHeight;
        convoY += bottomMargin;
        Color previousColor = GUI.color;
        GUI.color = Widgets.SeparatorLineColor; 
        Widgets.DrawLineHorizontal(0, convoY + bottomMargin, contentRect.width);
        GUI.color = previousColor;
      }
      Widgets.EndScrollView();
    }
  }
}
