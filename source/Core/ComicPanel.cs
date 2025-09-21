 #nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimDialogue.Core
{

  [StaticConstructorOnStartup]
  public abstract class ComicPanel
  {
    private static readonly float edgeWidth = Mathf.Ceil(78f / 2f);
    private static readonly float topEdgeHeight = Mathf.Ceil(77f / 2f);
    private static readonly float bottomEdgeHeight = Mathf.Ceil(122f / 2f);

    private static Texture2D? _earth;
    private static Texture2D Earth => _earth ??= ContentFinder<Texture2D>.Get("RimDialogue/earth_texture");

    private static Texture2D? _sky;
    private static Texture2D Sky => _sky ??= ContentFinder<Texture2D>.Get("RimDialogue/sky_texture");

    private static Texture2D? _cornerTL;
    private static Texture2D CornerTL => _cornerTL ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TL");

    private static Texture2D? _cornerTR;
    private static Texture2D CornerTR => _cornerTR ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TR");

    private static Texture2D? _cornerBL;
    private static Texture2D CornerBL => _cornerBL ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BL");

    private static Texture2D? _cornerBR;
    private static Texture2D CornerBR => _cornerBR ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BR");

    private static Texture2D? _topCenter;
    private static Texture2D TopCenter => _topCenter ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TC");

    private static Texture2D? _topCenterWide;
    private static Texture2D TopCenterWide => _topCenterWide ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TCW");

    private static Texture2D? _bottomCenter;
    private static Texture2D BottomCenter => _bottomCenter ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BC");

    private static Texture2D? _bottomCenterWide;
    private static Texture2D BottomCenterWide => _bottomCenterWide ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BCW");

    private static Texture2D? _bottomCenterReversed;
    private static Texture2D BottomCenterReversed => _bottomCenterReversed ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BCR");

    private static Texture2D? _centerLeft;
    private static Texture2D CenterLeft => _centerLeft ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_CL");

    private static Texture2D? _centerLeftSmall;
    private static Texture2D CenterLeftSmall => _centerLeftSmall ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_CLS");

    private static Texture2D? _centerRight;
    private static Texture2D CenterRight => _centerRight ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_CR");

    private static Texture2D? _centerRightSmall;
    private static Texture2D CenterRightSmall => _centerRightSmall ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_CRS");

    private static Texture2D? _bubbleBackground;
    private static Texture2D BubbleBackground => _bubbleBackground ??= ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_background");

    private static Texture2D? _caption_box;
    private static Texture2D Caption_box => _caption_box ??= ContentFinder<Texture2D>.Get("RimDialogue/caption_box");


    private static GUIStyle? _style;
    public static GUIStyle style
    { 
      get
      {
        _style ??= new GUIStyle(GUI.skin.label)
        {
          wordWrap = true,
          alignment = TextAnchor.MiddleCenter,
          fontSize = 16,
          normal = { textColor = Color.black },
          padding = new RectOffset(0, 0, 0, 0)
        };
        return _style;
      }
    }
    public static Texture2D ConvertRenderTextureToTexture2D(RenderTexture rt)
    {
      RenderTexture currentRT = RenderTexture.active;
      RenderTexture.active = rt;

      Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
      tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
      tex.Apply();

      RenderTexture.active = currentRT;
      return tex;
    }

    private static readonly char[] ApostropheLikeCharacters = new[]
    {
        '\u2019', // ’ RIGHT SINGLE QUOTATION MARK
        '\u02BC', // ʼ MODIFIER LETTER APOSTROPHE
        '\u0060', // `  GRAVE ACCENT
        '\u00B4', // ´  ACUTE ACCENT
        '\u02BC', // ʼ  MODIFIER LETTER APOSTROPHE
        '\u02BB', // ʻ  MODIFIER LETTER TURNED COMMA
        '\u02BD', // ʽ  MODIFIER LETTER REVERSED COMMA
        '\u02BE', // ʾ  MODIFIER LETTER RIGHT HALF RING
        '\u02BF', // ʿ  MODIFIER LETTER LEFT HALF RING
        '\u2018', // ‘  LEFT SINGLE QUOTATION MARK
        '\u2019', // ’  RIGHT SINGLE QUOTATION MARK (often used as apostrophe)
        '\u201B', // ‛  SINGLE HIGH-REVERSED-9 QUOTATION MARK
        '\u2032', // ′  PRIME
        '\u275B', // ❛  HEAVY SINGLE TURNED COMMA QUOTATION MARK ORNAMENT
        '\u275C', // ❜  HEAVY SINGLE COMMA QUOTATION MARK ORNAMENT
        '\uFF07'  // ＇ FULLWIDTH APOSTROPHE
    };

    /// <summary>
    /// Replaces all apostrophe-like characters in the input string with the ASCII apostrophe (').
    /// </summary>
    public static string Clean(string input)
    {
      if (string.IsNullOrEmpty(input)) return input;
      return new string(input.Select(c => ApostropheLikeCharacters.Contains(c) ? '\'' : c).ToArray());
    }

    public enum BubbleType
    {
      Normal,
      Reversed,
      Wide
    }

    public static void DrawCaption(Rect rect, string text)
    {
      text = Clean(text);

      var bitmapFont = BitmapFont.Get((FontFace)Settings.BitmapFont.Value);

      // Add some padding for the caption box
      float padding = 10f;
      float textWidth = rect.width - (padding * 2);
      float textHeight = bitmapFont.GetTextHeight(text, textWidth);
      
      // Create a properly sized box for the caption
      float captionBoxHeight = textHeight + (padding * 2);
      
      // Center the caption box in the provided rect
      Rect captionBoxRect = new Rect(
        rect.x,
        rect.y,
        rect.width,
        captionBoxHeight
      );
      
      // Draw the caption box texture
      Graphics.DrawTexture(captionBoxRect, Caption_box);
      
      // Calculate text position inside the caption box
      float textY = captionBoxRect.y + padding;
      
      // Draw the text inside the caption box
      bitmapFont.DrawText(
        text,
        new Vector2(captionBoxRect.x + padding, textY),
        textWidth);
    }

    public static void DrawSpeechBubble(Vector2 bubbleAPos, string text, float textHeight, float textWidth, BubbleType bubbleType)
    {
      text = Clean(text);

      Vector2 bubbleSize = new Vector2(
          textWidth + edgeWidth * 0.333f * 2,
          textHeight + (topEdgeHeight + bottomEdgeHeight) * 0.75f);

      int width = Mathf.CeilToInt(bubbleSize.x);
      int height = Mathf.CeilToInt(bubbleSize.y);

      Rect bubbleRect = new Rect(bubbleAPos.x, bubbleAPos.y, width, height);

       // Corners
      Graphics.DrawTexture(new Rect(bubbleRect.x, bubbleRect.y, edgeWidth + 1, topEdgeHeight + 1), CornerTL);
      Graphics.DrawTexture(new Rect(bubbleRect.xMax - edgeWidth, bubbleRect.y, edgeWidth + 1, topEdgeHeight + 1), CornerTR);
      Graphics.DrawTexture(new Rect(bubbleRect.x, bubbleRect.yMax - bottomEdgeHeight, edgeWidth + 1, bottomEdgeHeight + 1), CornerBL);
      Graphics.DrawTexture(new Rect(bubbleRect.xMax - edgeWidth, bubbleRect.yMax - bottomEdgeHeight, edgeWidth + 1, bottomEdgeHeight + 1), CornerBR);

      // Top/bottom edges
      Texture2D bottomCenter, topCenter;
      switch (bubbleType)
      {
        case BubbleType.Wide:
          topCenter = TopCenterWide;
          bottomCenter = BottomCenterWide;
          break;
        case BubbleType.Reversed:
          topCenter = TopCenter;
          bottomCenter = BottomCenterReversed;
          break;
        default:
          topCenter = TopCenter;
          bottomCenter = BottomCenter;
          break;
      }

      //Top and Bottom
      Graphics.DrawTexture(new Rect(bubbleRect.x + edgeWidth, bubbleRect.y, bubbleRect.width - 2 * edgeWidth + 1, topEdgeHeight + 1), topCenter);
      Graphics.DrawTexture(new Rect(bubbleRect.x + edgeWidth, bubbleRect.yMax - bottomEdgeHeight, bubbleRect.width - 2 * edgeWidth + 1, bottomEdgeHeight + 1), bottomCenter);

      Texture2D leftSide, rightSide;
      if (textHeight > 275)
      {
        leftSide = CenterLeft;
        rightSide = CenterRight;
      }
      else
      {
        leftSide = CenterLeftSmall;
        rightSide = CenterRightSmall;
      }

      // Sides
      Graphics.DrawTexture(new Rect(bubbleRect.x, bubbleRect.y + topEdgeHeight, edgeWidth + 1, bubbleRect.height - (topEdgeHeight + bottomEdgeHeight)), leftSide);
      Graphics.DrawTexture(new Rect(bubbleRect.xMax - edgeWidth, bubbleRect.y + topEdgeHeight, edgeWidth + 1, bubbleRect.height - (topEdgeHeight + bottomEdgeHeight)), rightSide);

      //BubbleBackground
      Graphics.DrawTexture(new Rect(bubbleRect.x + edgeWidth - 1, bubbleRect.y + topEdgeHeight - 1, bubbleRect.width - 2 * edgeWidth + 2, bubbleRect.height - (topEdgeHeight + bottomEdgeHeight) + 2), BubbleBackground);

      var bitmapFont = BitmapFont.Get((FontFace)Settings.BitmapFont.Value);
      bitmapFont.DrawText(
        text,
        new Vector2(bubbleRect.x + edgeWidth - 20, bubbleRect.y + topEdgeHeight - 20),
        textWidth);
    }

    public const int PortraitWidth = 96;
    public const int PortraitHeight = 128;

    protected string? Title;
    protected float SkyHeight;
    protected List<ComicPanelItem> BackgroundItems;

    public readonly BitmapFont BitmapFont;
    protected Texture2D? _texture;

    public ComicPanel(string? title, float skyHeight, BitmapFont bitmapFont, List<ComicPanelItem> backgroundItems)
    {
      Title = title.RemoveColor();
      SkyHeight = skyHeight;
      BackgroundItems = backgroundItems;
      BitmapFont = bitmapFont;
    }

    public virtual void DrawBackground(Rect canvas)
    {
      // Draw sky area
      Rect skyRect = new Rect(canvas.x, canvas.y, canvas.width, SkyHeight);
      Graphics.DrawTexture(skyRect, Sky);

      // Draw earth area
      Rect earthRect = new Rect(canvas.x, canvas.y + SkyHeight, canvas.width, canvas.height - SkyHeight);
      Graphics.DrawTexture(earthRect, Earth);

      // Draw background items
      foreach (var item in BackgroundItems)
      {
        var rect = new Rect(
            item.Rect.x + canvas.x,
            item.Rect.y + canvas.y,
            item.Rect.width,
            item.Rect.height
        );
        Graphics.DrawTexture(rect, item.Texture);
      }
    }

    public Texture2D GetTexture(Rect canvas)
    {
      Mod.Log($"Getting texture for {canvas}");

      int texWidth = Mathf.CeilToInt(canvas.width);
      int texHeight = Mathf.CeilToInt(canvas.height);

      RenderTexture rt = new RenderTexture(texWidth, texHeight, 0);
      RenderTexture.active = rt;
      GL.PushMatrix();
      GL.LoadPixelMatrix(0, texWidth, texHeight, 0);
      GL.Clear(true, true, Color.clear);

      DrawBackground(canvas);
      if (Title != null && !string.IsNullOrWhiteSpace(Title))
        DrawCaption(new Rect(canvas.x + 10, canvas.y + 10, canvas.width - 20, 0f), Title);
      DrawInternal(canvas);

      Texture2D finalTex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
      finalTex.ReadPixels(new Rect(0, 0, texWidth, texHeight), 0, 0);
      finalTex.Apply();

      GL.PopMatrix();
      RenderTexture.active = null;
      rt.Release();
      UnityEngine.Object.Destroy(rt);

      return finalTex;
    }

    public Texture2D? Texture
    {
      get
      {
        return _texture;
      }
    }

    public void Draw(Rect canvas)
    {
      _texture ??= GetTexture(canvas);
      GUI.DrawTexture(canvas, _texture, ScaleMode.StretchToFill, true, 1f);
    }

    protected abstract void DrawInternal(Rect canvas);

  }
}
