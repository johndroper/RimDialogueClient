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
  private readonly string dialogueA;
  private readonly Pawn pawnB;
  private readonly string dialogueB;

  public TwoPawnComicPanel(Pawn pawnA, string dialogueA, Pawn pawnB, string dialogueB, float skyHeight, List<ComicPanelItem> BackgroundItems) : base(skyHeight, BackgroundItems)
  {
    this.pawnA = pawnA;
    this.dialogueA = dialogueA;
    this.pawnB = pawnB;
    this.dialogueB = dialogueB;
  }

  Texture2D portraitATex;
  Texture2D portraitBTex;
  protected override void DrawInternal(Rect canvas)
  {
    
    const int portraitWidth = 96;
    const int portraitHeight = 128;
    const float textWidth = 200f;
    
    float portraitY = canvas.y + canvas.height - portraitHeight - 20;

    // Portraits
    if (portraitATex == null)
    {
      if (RimDialogue.Settings.VerboseLogging.Value) RimDialogue.Mod.Log($"Fetching portrait for {pawnA.Name} ({pawnA.thingIDNumber})");
      RenderTexture portraitART = PortraitsCache.Get(pawnA, ColonistBarColonistDrawer.PawnTextureSize, Rot4.East);
      portraitATex = ConvertRenderTextureToTexture2D(portraitART);
    }
    GUI.DrawTexture(new Rect(canvas.x + canvas.width * 0.33f - portraitWidth / 2f, portraitY, portraitWidth, portraitHeight), portraitATex);
    if (portraitBTex == null)
    {
      RimDialogue.Mod.Log($"Fetching portrait for {pawnB.Name} ({pawnB.thingIDNumber})");
      RenderTexture portraitBRT = PortraitsCache.Get(pawnB, ColonistBarColonistDrawer.PawnTextureSize, Rot4.West);
      portraitBTex = ConvertRenderTextureToTexture2D(portraitBRT);
    }
    GUI.DrawTexture(new Rect(canvas.x + canvas.width * 0.71f - portraitWidth / 2f, portraitY, portraitWidth, portraitHeight), portraitBTex);

    // Speech bubble positions
    float dialogueAHeight = GetTextHeight(dialogueA, textWidth);
    Vector2 bubbleAPos = new Vector2(canvas.x + canvas.width * 0.28f - textWidth / 2, portraitY - dialogueAHeight - 50);
    var bubbleARect = DrawSpeechBubble(bubbleAPos, dialogueA, dialogueAHeight, textWidth, BubbleType.Normal);

    if (dialogueB != null)
    {
      float dialogueBHeight = GetTextHeight(dialogueB, textWidth);
      Vector2 bubbleBPos = new Vector2(canvas.x + canvas.width * 0.72f - textWidth / 2, portraitY - dialogueBHeight - 50);
      var bubbleBRect = DrawSpeechBubble(bubbleBPos, dialogueB, dialogueBHeight, textWidth, BubbleType.Reversed);
    }
  }
}
