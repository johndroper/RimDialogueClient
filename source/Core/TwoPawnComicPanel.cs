#nullable enable
using RimDialogue.Core;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

public class TwoPawnComicPanel : ComicPanel
{
  private readonly Pawn pawnA;
  private readonly string? dialogueA;
  private readonly Pawn? pawnB;
  private readonly string? dialogueB;

  public TwoPawnComicPanel(string? title, Pawn pawnA, string? dialogueA, Pawn? pawnB, string? dialogueB, float skyHeight, BitmapFont font, List<ComicPanelItem> BackgroundItems) : base(title, skyHeight, font, BackgroundItems)
  {
    this.pawnA = pawnA;
    this.dialogueA = dialogueA;
    this.pawnB = pawnB;
    this.dialogueB = dialogueB;
  }

  Texture2D? portraitATex;
  Texture2D? portraitBTex;

  const float DrawScale = 150;
  protected override void DrawInternal(Rect canvas)
  {
    if (pawnA == null || pawnB == null)
    {
      RimDialogue.Mod.Error("One or both pawns are null.");
      return;
    }

    float textWidth = (canvas.width - 100f) / 2f;
    float portraitY = canvas.y + canvas.height - PortraitHeight - 20;

    var offset = new Vector3(0f, -2f, 0f);

    // --- Portrait A ---
    if (portraitATex == null)
    {
      if (RimDialogue.Settings.VerboseLogging.Value)
        RimDialogue.Mod.Log($"Fetching portrait for {pawnA.Name} ({pawnA.thingIDNumber})");

      RimDialogue.Mod.Log($"pawnA DrawSize:{pawnA.DrawSize}");

      RenderTexture portraitART = PortraitsCache.Get(pawnA, pawnA.DrawSize * DrawScale, Rot4.East, cameraZoom: 0.8f);
      portraitATex = ConvertRenderTextureToTexture2D(portraitART);
    }

    var pawnAWidth = pawnB.DrawSize.x * DrawScale;
    Rect portraitARect = new Rect(
        canvas.width * 0.32f - pawnAWidth / 2f,
        portraitY,
        pawnAWidth,
        pawnA.DrawSize.y * DrawScale
    );
    Graphics.DrawTexture(portraitARect, portraitATex);

    // --- Portrait B ---
    if (portraitBTex == null)
    {
      //RimDialogue.Mod.Log($"Fetching portrait for {pawnB?.Name?.ToStringShort ?? "Null"} ({pawnB?.thingIDNumber.ToString() ?? "Null"})");
      RimDialogue.Mod.Log($"pawnB DrawSize:{pawnB.DrawSize}");
      RenderTexture portraitBRT = PortraitsCache.Get(pawnB, pawnB.DrawSize * DrawScale, Rot4.West, cameraZoom: 0.8f);
      portraitBTex = ConvertRenderTextureToTexture2D(portraitBRT);
    }

    var pawnBWidth = pawnB.DrawSize.x * DrawScale;

    Rect portraitBRect = new Rect(
        canvas.width * 0.68f - pawnBWidth / 2f,
        portraitY,
        pawnBWidth,
        pawnB.DrawSize.y * DrawScale
    );
    Graphics.DrawTexture(portraitBRect, portraitBTex);

    // --- Bubble A ---
    if (dialogueA != null)
    {
      float dialogueAHeight = this.BitmapFont.GetTextHeight(dialogueA, textWidth);
      Vector2 bubbleAPos = new Vector2(
        canvas.x + 20,//canvas.width * 0.28f - textWidth / 2,
        portraitY - dialogueAHeight - 50);
      DrawSpeechBubble(bubbleAPos, dialogueA, dialogueAHeight, textWidth, BubbleType.Normal);
    }

    // --- Bubble B ---
    if (dialogueB != null)
    {
      float dialogueBHeight = this.BitmapFont.GetTextHeight(dialogueB, textWidth);
      Vector2 bubbleBPos = new Vector2(
        canvas.x + canvas.width / 2 + 10,  //canvas.width * 0.72f - textWidth / 2,
        portraitY - dialogueBHeight - 50);
      DrawSpeechBubble(bubbleBPos, dialogueB, dialogueBHeight, textWidth, BubbleType.Reversed);
    }
  }
}
