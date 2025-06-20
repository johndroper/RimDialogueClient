using Bubbles;
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

public class SinglePawnComicPanel : ComicPanel
{
  private readonly Pawn pawn;
  private readonly string dialogue;

  public SinglePawnComicPanel(Pawn pawn, string dialogue, float skyHeight, List<ComicPanelItem> backgroundItems)
      : base(skyHeight, backgroundItems)
  {
    this.pawn = pawn;
    this.dialogue = dialogue;
  }

  private Texture2D portraitTex;

  protected override void DrawInternal(Rect canvas)
  {
    const float textWidth = 400f;

    // --- Get or render the pawn portrait ---
    if (portraitTex == null)
    {
      if (RimDialogue.Settings.VerboseLogging.Value)
        RimDialogue.Mod.Log($"Fetching portrait for {pawn.Name} ({pawn.thingIDNumber})");

      RenderTexture portraitRT = PortraitsCache.Get(
        pawn,
        ColonistBarColonistDrawer.PawnTextureSize,
        Rot4.South);
      portraitTex = ConvertRenderTextureToTexture2D(portraitRT);
    }

    float portraitY = canvas.y + canvas.height - PortraitHeight - 20;
    Rect portraitRect = new Rect(
        canvas.width * 0.5f - PortraitWidth / 2f,
        portraitY,
        PortraitWidth,
        PortraitHeight
    );
    Graphics.DrawTexture(portraitRect, portraitTex);

    // --- Create and draw the speech bubble ---
    float textHeight = GetTextHeight(dialogue, textWidth);
    Vector2 bubblePos = new Vector2(
      canvas.x + canvas.width * 0.5f - textWidth / 2,
      portraitY - textHeight - 50);

    DrawSpeechBubble(bubblePos, dialogue, textHeight, textWidth, BubbleType.Wide);
  }
}
