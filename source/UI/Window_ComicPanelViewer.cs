using Bubbles.Core;
using RimDialogue.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimDialogue.UI
{
  public class Window_ComicPanelViewer : Window
  {
    private readonly Conversation Conversation;
    private Vector2 scrollPosition = Vector2.zero;

    public override Vector2 InitialSize => new Vector2(600f, 500f);

    private List<ComicPanel> _Panels = new List<ComicPanel>();

    private const float PanelHeight = 400f;
    private const float PanelWidth = 550f;
    private const float PanelSpacing = 10f;
    private const float SaveButtonHeight = 30f;
    private const float SaveButtonWidth = 120f;
    private const float TextFieldHeight = 25f;
    private const float BottomUIHeight = 70f; // Space for textfield + button + margins

    private string customFilePath = "";
    private string filePathBuffer = "";

    public float SkyHeight { get; private set; } = PanelHeight - ComicPanel.PortraitHeight;

    private BitmapFont BitmapFont;

    public Window_ComicPanelViewer(BitmapFont bitmapFont, Conversation conversation)
    {
      this.Conversation = conversation;
      this.closeOnClickedOutside = false;
      this.draggable = true;
      this.doCloseButton = false;
      this.doCloseX = true;
      this.absorbInputAroundWindow = false;
      this.preventCameraMotion = false;
      this.BitmapFont = bitmapFont;

      InitializeDefaultFilePath();

      List<ComicPanelItem> backgroundItems = GetBackgroundItems(PanelWidth, PanelHeight - ComicPanel.PortraitHeight);

      if (!Conversation.Lines.Any())
        Mod.Error("Conversation has no lines.");
      if (Conversation.Recipient == null)
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Creating single pawn comic panels for conversation with initiator {Conversation.Initiator.Name} ({Conversation.Initiator.thingIDNumber})");
        for (int i = 0; i < Conversation.Lines.Length; i++)
        {
          _Panels.Add(new SinglePawnComicPanel(Conversation.Initiator, bitmapFont, Conversation.Lines[i].Text, SkyHeight, backgroundItems));
          if (Settings.VerboseLogging.Value) Mod.Log($"SinglePawnComicPanel panel added for line {i}.");
        }
      }
      else
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Creating two pawn comic panels for conversation between {Conversation.Initiator.Name} ({Conversation.Initiator.thingIDNumber}) and {Conversation.Recipient.Name} ({Conversation.Recipient.thingIDNumber})");


        for (int i = 0; i < Conversation.Lines.Length; i++)
        {
          if (Conversation.Lines[i].Name == Conversation.Recipient.Name.ToStringShort)
          {
            _Panels.Add(
              new TwoPawnComicPanel(
                Conversation.Initiator,
                null,
                Conversation.Recipient,
                Conversation.Lines[i].Text,
                SkyHeight,
                bitmapFont,
                backgroundItems));
          }
          else
          {
            if (i + 1 < Conversation.Lines.Length)
            {
              if (Conversation.Lines[i].Name == Conversation.Lines[i + 1].Name)
              {
                _Panels.Add(
                  new TwoPawnComicPanel(
                    Conversation.Initiator,
                    Conversation.Lines[i].Text,
                    Conversation.Recipient,
                    null,
                    SkyHeight,
                    bitmapFont,
                    backgroundItems));
              }
              else
              {
                _Panels.Add(
                  new TwoPawnComicPanel(
                    Conversation.Initiator,
                    Conversation.Lines[i].Text,
                    Conversation.Recipient,
                    Conversation.Lines[i + 1].Text,
                    SkyHeight,
                    bitmapFont,
                    backgroundItems));
                i++;
              }
            }
            else
              _Panels.Add(
                new TwoPawnComicPanel(
                  Conversation.Initiator,
                  Conversation.Lines[i].Text,
                  Conversation.Recipient,
                  null,
                  SkyHeight,
                  bitmapFont,
                  backgroundItems));
          }
        }
      }
    }

    public List<ComicPanelItem> GetBackgroundItems(float width, float skyHeight)
    {
      List<ComicPanelItem> items = new List<ComicPanelItem>();
      List<Thing> treeThings = new List<Thing>();
      var trees = Find.CurrentMap.listerThings.AllThings
        .Where(t => t.def.category == ThingCategory.Plant && t.def.plant.IsTree);
      int treesToFetch = Rand.Range(10, 20);
      while (treeThings.Count < treesToFetch)
      {
        treeThings.Add(trees.RandomElement());
      }
      float x = Rand.Range(-10, 100);
      float y = skyHeight - 30;
      float size = 32;
      for (var row = 0; row < 4; row++)
      {
        foreach (var tree in treeThings)
        {
          items.Add(new ComicPanelItem(
            (Texture2D)tree.Graphic.MatSingle.mainTexture,
            new Rect(x, y, size, size)));
          x += Rand.Range(10, 100);
          if (x > width)
            x = Rand.Range(-20, 100);
        }
        y += 5;
        size += 8;
      }
      return items;
    }

    private void InitializeDefaultFilePath()
    {
      try
      {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string participantNames = Conversation.Recipient != null
          ? $"{Conversation.Initiator.Name.ToStringShort}_{Conversation.Recipient.Name.ToStringShort}"
          : Conversation.Initiator.Name.ToStringShort;

        foreach (char c in Path.GetInvalidFileNameChars())
        {
          participantNames = participantNames.Replace(c, '_');
        }

        string filename = $"{participantNames}_{timestamp}.png";

        string screenshotsPath = Path.Combine(GenFilePaths.ScreenshotFolderPath, "RimDialogue");
        customFilePath = Path.Combine(screenshotsPath, filename);
        filePathBuffer = customFilePath;
      }
      catch (Exception ex)
      {
        Mod.Error($"Failed to initialize default file path: {ex}");
        customFilePath = "RimDialogue_Comic.png";
        filePathBuffer = customFilePath;
      }
    }

    List<Texture2D> panelTextures;
    public override void WindowOnGUI()
    {
      if (panelTextures == null)
      {
        panelTextures = new List<Texture2D>();
        foreach (var panel in _Panels)
        {
          var panelTexture = panel.Texture ?? panel.GetTexture(new Rect(0, 0, PanelWidth, PanelHeight));
          panelTextures.Add(panelTexture);
        }
      }
      base.WindowOnGUI();
    }


    public override void DoWindowContents(Rect inRect)
    {
      try
      {
        if (panelTextures == null)
          return;

        // Reserve space for bottom UI (textfield + save button)
        Rect scrollRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - BottomUIHeight);

        float totalHeight = _Panels.Count * (PanelHeight + PanelSpacing);
        Rect viewRect = new Rect(0, 0, scrollRect.width, totalHeight);

        Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect, true);

        float curY = 0;
        foreach (var panel in panelTextures)
        {
          Rect panelRect = new Rect(0, curY, viewRect.width, PanelHeight);
          GUI.DrawTexture(panelRect, panel, ScaleMode.StretchToFill, true, 1f);
          curY += PanelHeight + PanelSpacing;
        }

        Widgets.EndScrollView();

        float bottomY = inRect.height - BottomUIHeight + 5f;

        Rect pathLabelRect = new Rect(10f, bottomY, 100f, 20f);
        Widgets.Label(pathLabelRect, "Save Path:");

        Rect pathTextRect = new Rect(10f, bottomY + 20f, inRect.width - SaveButtonWidth - 30f, TextFieldHeight);
        string newPath = Widgets.TextField(pathTextRect, filePathBuffer);
        if (newPath != filePathBuffer)
        {
          filePathBuffer = newPath;
          customFilePath = newPath;
        }

        // Save as PNG button
        Rect saveButtonRect = new Rect(inRect.width - SaveButtonWidth - 10f, pathTextRect.y, SaveButtonWidth, SaveButtonHeight);
        if (Widgets.ButtonText(saveButtonRect, "Save as PNG"))
        {
          SavePanelsToPNG(viewRect, newPath);
        }

        //var oldColor = GUI.color;
        //GUI.color = Color.white;
        //Widgets.Label(new Rect(50, 50, 200, 200), new GUIContent($"Panels: {_Panels.Count}"));
        //GUI.color = oldColor;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    private void SavePanelsToPNG(Rect viewRect, string filePath)
    {
      try
      {
        int width = (int)PanelWidth;
        int height = _Panels.Count * ((int)PanelHeight + (int)PanelSpacing) - (int)PanelSpacing;
        if (height <= 0) height = (int)PanelHeight;

        List<Texture2D> panelTextures = new List<Texture2D>();
        foreach (var panel in _Panels)
        {
          var panelTexture = panel.Texture ?? panel.GetTexture(new Rect(0, 0, PanelWidth, PanelHeight));
          panelTextures.Add(panelTexture);
        }

        RenderTexture rt = new RenderTexture(width, height, 0);
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, width, height, 0);
        GL.Clear(true, true, Color.black);

        float curY = 0;
        foreach (var panelTexture in panelTextures)
        {
          Rect destRect = new Rect(0, curY, panelTexture.width, panelTexture.height);
          Graphics.DrawTexture(destRect, panelTexture);
          curY += PanelHeight + PanelSpacing;
        }

        Texture2D finalTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        finalTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        finalTex.Apply();

        GL.PopMatrix();
        RenderTexture.active = null;
        rt.Release();
        UnityEngine.Object.Destroy(rt);

        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
          Directory.CreateDirectory(directory);

        byte[] pngData = ImageConversion.EncodeToPNG(finalTex);
        File.WriteAllBytes(filePath, pngData);
      }
      catch (Exception ex)
      {
        Mod.Error($"Failed to save comic panels: {ex}");
      }
    }
  }
}
