#nullable enable
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

public class TwoPawnComicPanel : ComicPanel
{
  private readonly Pawn pawnA;
  private readonly string? dialogueA;
  private readonly Pawn? pawnB;
  private readonly string? dialogueB;

  public TwoPawnComicPanel(Pawn pawnA, string? dialogueA, Pawn? pawnB, string? dialogueB, float skyHeight, BitmapFont font, List<ComicPanelItem> BackgroundItems) : base(skyHeight, font, BackgroundItems)
  {
    this.pawnA = pawnA;
    this.dialogueA = dialogueA;
    this.pawnB = pawnB;
    this.dialogueB = dialogueB;
  }

  Texture2D? portraitATex;
  Texture2D? portraitBTex;
  protected override void DrawInternal(Rect canvas)
  {
    const float textWidth = 200f;

    float portraitY = canvas.y + canvas.height - PortraitHeight - 20;

    // --- Portrait A ---
    if (portraitATex == null)
    {
      if (RimDialogue.Settings.VerboseLogging.Value)
        RimDialogue.Mod.Log($"Fetching portrait for {pawnA.Name} ({pawnA.thingIDNumber})");

      RenderTexture portraitART = PortraitsCache.Get(pawnA, ColonistBarColonistDrawer.PawnTextureSize, Rot4.East);
      portraitATex = ConvertRenderTextureToTexture2D(portraitART);
    }

    Rect portraitARect = new Rect(
        canvas.width * 0.33f - PortraitWidth / 2f,
        portraitY,
        PortraitWidth,
        PortraitHeight
    );
    Graphics.DrawTexture(portraitARect, portraitATex);

    // --- Portrait B ---
    if (portraitBTex == null)
    {
      //RimDialogue.Mod.Log($"Fetching portrait for {pawnB?.Name?.ToStringShort ?? "Null"} ({pawnB?.thingIDNumber.ToString() ?? "Null"})");
      RenderTexture portraitBRT = PortraitsCache.Get(pawnB, ColonistBarColonistDrawer.PawnTextureSize, Rot4.West);
      portraitBTex = ConvertRenderTextureToTexture2D(portraitBRT);
    }

    Rect portraitBRect = new Rect(
        canvas.width * 0.71f - PortraitWidth / 2f,
        portraitY,
        PortraitWidth,
        PortraitHeight
    );
    Graphics.DrawTexture(portraitBRect, portraitBTex);

    // --- Bubble A ---
    if (dialogueA != null)
    {
      float dialogueAHeight = this.BitmapFont.GetTextHeight(dialogueA, textWidth);
      Vector2 bubbleAPos = new Vector2(
        canvas.x + canvas.width * 0.28f - textWidth / 2,
        portraitY - dialogueAHeight - 50);
      DrawSpeechBubble(bubbleAPos, dialogueA, dialogueAHeight, textWidth, BubbleType.Normal);
    }

    // --- Bubble B ---
    if (dialogueB != null)
    {
      float dialogueBHeight = this.BitmapFont.GetTextHeight(dialogueB, textWidth);
      Vector2 bubbleBPos = new Vector2(
        canvas.x + canvas.width * 0.72f - textWidth / 2,
        portraitY - dialogueBHeight - 50);
      DrawSpeechBubble(bubbleBPos, dialogueB, dialogueBHeight, textWidth, BubbleType.Reversed);
    }
  }
}
