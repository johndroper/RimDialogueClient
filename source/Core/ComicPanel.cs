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
  public abstract class ComicPanel
  {
    private static readonly Color BubbleBackgroundColor = ColorUtility.TryParseHtmlString("#e7e8e8", out var col) ? col : new Color(0.91f, 0.91f, 0.91f, 1f);
    private static readonly float edgeWidth = Mathf.Ceil(78f / 2f);
    private static readonly float topEdgeHeight = Mathf.Ceil(77f / 2f);
    private static readonly float bottomEdgeHeight = Mathf.Ceil(122f / 2f);
    private static readonly Texture2D CornerTL = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TL");
    private static readonly Texture2D CornerTR = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TR");
    private static readonly Texture2D CornerBL = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BL");
    private static readonly Texture2D CornerBR = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BR");
    private static readonly Texture2D TopCenter = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TC");
    private static readonly Texture2D TopCenterWide = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_TCW");
    private static readonly Texture2D BottomCenter = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BC");
    private static readonly Texture2D BottomCenterWide = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BCW");
    private static readonly Texture2D BottomCenterReversed = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_BCR");
    private static readonly Texture2D CenterLeft = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_CL");
    private static readonly Texture2D CenterRight = ContentFinder<Texture2D>.Get("RimDialogue/dialogue_bubble_CR");

    public static readonly GUIStyle style = new GUIStyle(GUI.skin.label)
    {
      wordWrap = true,
      alignment = TextAnchor.MiddleCenter,
      fontSize = 16,
      normal = { textColor = Color.black },
      padding = new RectOffset(0, 0, 0, 0)
    };
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

    public static void SaveScreenRegionAsPNG(Rect screenRect, string filename)
    {
      Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();

      Texture2D regionTexture = new Texture2D((int)screenRect.width, (int)screenRect.height);

      Color[] pixels = screenTexture.GetPixels(
          (int)screenRect.x,
          Screen.height - (int)screenRect.y - (int)screenRect.height,
          (int)screenRect.width,
          (int)screenRect.height
      );

      regionTexture.SetPixels(pixels);
      regionTexture.Apply();

      byte[] pngData = ImageConversion.EncodeToPNG(regionTexture);

      string path = Path.Combine(GenFilePaths.SaveDataFolderPath, filename + ".png");
      File.WriteAllBytes(path, pngData);

      UnityEngine.Object.Destroy(screenTexture);
      UnityEngine.Object.Destroy(regionTexture);
    }


    public static float GetTextHeight(string text, float textWidth)
    {
      return style.CalcHeight(new GUIContent(text), textWidth);
    }

    public enum BubbleType
    {
      Normal,
      Reversed,
      Wide
    }

    public static Rect DrawSpeechBubble(Vector2 position, string text, float textHeight, float textWidth, BubbleType bubbleType)
    {
      Vector2 bubbleSize = new Vector2(
          textWidth + edgeWidth * 0.333f * 2,
          textHeight + (topEdgeHeight + bottomEdgeHeight) * 0.75f);

      Rect bubbleRect = new Rect(position, bubbleSize);

      Rect centerRect = new Rect(
          bubbleRect.x + (bubbleRect.width - textWidth) / 2,
          bubbleRect.y + (bubbleRect.height - textHeight) / 2 - 12,
          textWidth,
          textHeight
      );

      GUI.DrawTexture(new Rect(bubbleRect.x, bubbleRect.y, edgeWidth, topEdgeHeight), CornerTL);
      GUI.DrawTexture(new Rect(bubbleRect.xMax - edgeWidth, bubbleRect.y, edgeWidth, topEdgeHeight), CornerTR);
      GUI.DrawTexture(new Rect(bubbleRect.x, bubbleRect.yMax - bottomEdgeHeight, edgeWidth, bottomEdgeHeight), CornerBL);
      GUI.DrawTexture(new Rect(bubbleRect.xMax - edgeWidth, bubbleRect.yMax - bottomEdgeHeight, edgeWidth, bottomEdgeHeight), CornerBR);

      Texture2D bottomCenter, topCenter;
      switch(bubbleType)
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

      GUI.DrawTexture(new Rect(bubbleRect.x + edgeWidth, bubbleRect.y, bubbleRect.width - 2 * edgeWidth + 1, topEdgeHeight + 1), topCenter);
      GUI.DrawTexture(new Rect(bubbleRect.x + edgeWidth, bubbleRect.yMax - bottomEdgeHeight, bubbleRect.width - 2 * edgeWidth + 1, bottomEdgeHeight + 1), bottomCenter);
      GUI.DrawTexture(new Rect(bubbleRect.x, bubbleRect.y + topEdgeHeight, edgeWidth, bubbleRect.height - (topEdgeHeight + bottomEdgeHeight)), CenterLeft);
      GUI.DrawTexture(new Rect(bubbleRect.xMax - edgeWidth, bubbleRect.y + topEdgeHeight, edgeWidth, bubbleRect.height - (topEdgeHeight + bottomEdgeHeight)), CenterRight);

      Color prevColor = GUI.color;
      GUI.color = BubbleBackgroundColor;
      GUI.DrawTexture(centerRect, BaseContent.WhiteTex);
      GUI.color = prevColor;

      GUI.Label(centerRect, text, style);

      return bubbleRect;
    }

    public const int PortraitWidth = 96;
    public const int PortraitHeight = 128;

    protected float SkyHeight;
    protected List<ComicPanelItem> BackgroundItems = null;

    public ComicPanel(float skyHeight, List<ComicPanelItem> backgroundItems)
    {
      SkyHeight = skyHeight;
      BackgroundItems = backgroundItems;
    }

    //public List<ComicPanelItem> GetBackgroundItems(Rect canvas, float skyHeight)
    //{
    //  List<ComicPanelItem> items = new List<ComicPanelItem>();
    //  List<Thing> treeThings = new List<Thing>();
    //  var trees = Find.CurrentMap.listerThings.AllThings
    //    .Where(t => t.def.category == ThingCategory.Plant && t.def.plant.IsTree);
    //  int treesToFetch = Rand.Range(10, 20);
    //  while (treeThings.Count < treesToFetch)
    //  {
    //    treeThings.Add(trees.RandomElement());
    //  }
    //  float x = canvas.x + Rand.Range(-10, 100);
    //  float y = canvas.y + skyHeight - 30f;
    //  float size = 32;
    //  for (var row = 0; row < 4; row++)
    //  {
    //    foreach (var tree in treeThings)
    //    {
    //      items.Add(new ComicPanelItem(
    //        (Texture2D)tree.Graphic.MatSingle.mainTexture,
    //        new Rect(x, y, size, size)));
    //      x += Rand.Range(10, 100);
    //      if (x > canvas.width)
    //        x = Rand.Range(-20, 100);
    //    }
    //    y += 5;
    //    size += 8;
    //  }
    //  return items;
    //}
    public static Color skyColor = new Color(0.5294f, 0.8078f, 0.9215f, 1f);
    public static Color earthColor = new Color(0.5882f, 0.2941f, 0, 1f);

    public virtual void DrawBackground(Rect canvas)
    {
      Color originalColor = GUI.color;
      Widgets.DrawBoxSolid(new Rect(canvas.x, canvas.y, canvas.width, SkyHeight), skyColor);
      Widgets.DrawBoxSolid(new Rect(canvas.x, canvas.y + SkyHeight, canvas.width, canvas.height - SkyHeight), earthColor);
      GUI.color = originalColor;

      foreach (var item in BackgroundItems)
      {
        var rect = new Rect(
          item.Rect.x + canvas.x,
          item.Rect.y + canvas.y,
          item.Rect.width,
          item.Rect.width);
        GUI.DrawTexture(rect, item.Texture);
      }
    }

    public void Draw(Rect canvas)
    {
      DrawBackground(canvas);
      DrawInternal(canvas);
    }

    protected abstract void DrawInternal(Rect canvas);

    public virtual void DrawToTexture(Texture2D texture, Rect canvas)
    {
      // Set up a temporary RenderTexture and draw the panel into it, then copy to the provided Texture2D
      RenderTexture rt = new RenderTexture((int)canvas.width, (int)canvas.height, 24);
      RenderTexture.active = rt;
      GL.Clear(true, true, Color.clear);

      var oldMatrix = GUI.matrix;
      try
      {
        GUIUtility.RotateAroundPivot(0, Vector2.zero);
        Draw(canvas);
        texture.ReadPixels(new Rect(0, 0, (int)canvas.width, (int)canvas.height), 0, 0);
        texture.Apply();
      }
      finally
      {
        GUI.matrix = oldMatrix;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(rt);
      }
    }

    public virtual Texture2D RenderToTexture(int width, int height)
    {
      // Create a RenderTexture and Texture2D
      RenderTexture rt = new RenderTexture(width, height, 24);
      RenderTexture.active = rt;
      GL.Clear(true, true, Color.clear);

      // Save and restore GUI matrix
      var oldMatrix = GUI.matrix;
      try
      {
        // Draw the panel as if on screen
        Draw(new Rect(0, 0, width, height));
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        return tex;
      }
      finally
      {
        GUI.matrix = oldMatrix;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(rt);
      }
    }
  }
}
