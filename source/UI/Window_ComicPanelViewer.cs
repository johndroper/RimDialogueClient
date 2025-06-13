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

    //try using screencapture instead of RenderTexture
    //automate scrolling the box and capturing the scrollview

    public Window_ComicPanelViewer(Conversation conversation)
    {
      this.Conversation = conversation;
      this.closeOnClickedOutside = false;
      this.draggable = true;
      this.doCloseButton = false;
      this.doCloseX = true;
      this.absorbInputAroundWindow = false;
      this.preventCameraMotion = false;

      // Initialize default file path
      InitializeDefaultFilePath();

      List<ComicPanelItem> backgroundItems = GetBackgroundItems(PanelWidth, PanelHeight - ComicPanel.PortraitHeight);

      if (!Conversation.Lines.Any())
        Mod.Error("Conversation has no lines.");
      if (Conversation.Recipient == null)
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Creating single pawn comic panels for conversation with initiator {Conversation.Initiator.Name} ({Conversation.Initiator.thingIDNumber})");
        for (int i = 0; i < Conversation.Lines.Length; i++)
        {
          _Panels.Add(new SinglePawnComicPanel(Conversation.Initiator, Conversation.Lines[i].Text, SkyHeight, backgroundItems));
        }
      }
      else
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Creating two pawn comic panels for conversation between {Conversation.Initiator.Name} ({Conversation.Initiator.thingIDNumber}) and {Conversation.Recipient.Name} ({Conversation.Recipient.thingIDNumber})");
        for (int i = 0; i < Conversation.Lines.Length; i += 2)
        {
          _Panels.Add(
            new TwoPawnComicPanel(
              Conversation.Initiator,
              Conversation.Lines[i].Text,
              Conversation.Recipient,
              i + 1 < Conversation.Lines.Length ? Conversation.Lines[i + 1].Text : null,
              SkyHeight,
              backgroundItems));
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
          ? $"{Conversation.Initiator.Name}_{Conversation.Recipient.Name}"
          : Conversation.Initiator.Name.ToStringShort;

        // Clean filename of invalid characters
        foreach (char c in Path.GetInvalidFileNameChars())
        {
          participantNames = participantNames.Replace(c, '_');
        }

        string filename = $"RimDialogue_Comic_{participantNames}_{timestamp}.png";

        // Default to Screenshots folder
        string screenshotsPath = Path.Combine(GenFilePaths.SaveDataFolderPath, "Screenshots");
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

    private bool _captureScrollView = false;

    public override void WindowOnGUI()
    {
      base.WindowOnGUI();
      if(render)
        SavePanelsToPNG(customFilePath);
      render = false;
    }

    bool render = false;
    public override void DoWindowContents(Rect inRect)
    {
      try
      {
        // Reserve space for bottom UI (textfield + save button)
        Rect scrollRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - BottomUIHeight);

        float totalHeight = _Panels.Count * (PanelHeight + PanelSpacing);
        Rect viewRect = new Rect(0, 0, scrollRect.width, totalHeight);

        Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect, true);

        float curY = 0;
        foreach (var panel in _Panels)
        {
          Rect panelRect = new Rect(0, curY, viewRect.width, PanelHeight);
          panel.Draw(panelRect);
          curY += PanelHeight + PanelSpacing;
        }

        Widgets.EndScrollView();

        // Bottom UI area
        float bottomY = inRect.height - BottomUIHeight + 5f;

        // File path label
        Rect pathLabelRect = new Rect(10f, bottomY, 100f, 20f);
        Widgets.Label(pathLabelRect, "Save Path:");

        // File path text field
        Rect pathTextRect = new Rect(10f, bottomY + 20f, inRect.width - SaveButtonWidth - 30f, TextFieldHeight);
        string newPath = Widgets.TextField(pathTextRect, filePathBuffer);
        if (newPath != filePathBuffer)
        {
          filePathBuffer = newPath;
          customFilePath = newPath;
        }

        // Save as PNG button
        float buttonY = inRect.height - BottomUIHeight + 20f + TextFieldHeight + 10f;
        Rect saveButtonRect = new Rect(inRect.width - SaveButtonWidth - 10f, buttonY, SaveButtonWidth, SaveButtonHeight);
        if (Widgets.ButtonText(saveButtonRect, "Save as PNG"))
        {
          render = true;
          
        }
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    private void SavePanelsToPNG(string filePath)
    {
      try
      {
        int width = (int)PanelWidth;
        int height = _Panels.Count * ((int)PanelHeight + (int)PanelSpacing) - (int)PanelSpacing;
        if (height <= 0) height = (int)PanelHeight;

        // Create the final composite texture
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Fill with transparent or white background
        Color[] clearPixels = new Color[width * height];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = Color.clear;
        result.SetPixels(clearPixels);

        // Render and composite each panel
        float curY = 0;
        int x = 0;
        foreach (var panel in _Panels)
        {
          Texture2D panelTex = panel.RenderToTexture(width, (int)PanelHeight);
          byte[] testData = ImageConversion.EncodeToPNG(result);
          File.WriteAllBytes("C:\\Users\\madja\\panel_" + x + ".png", testData);
          Color[] panelPixels = panelTex.GetPixels();
          result.SetPixels(0, (int)curY, width, (int)PanelHeight, panelPixels);
          UnityEngine.Object.Destroy(panelTex);
          curY += PanelHeight + PanelSpacing;
          x++;
        }
        result.Apply();

        // Ensure directory exists
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
          Directory.CreateDirectory(directory);

        // Save as PNG
        byte[] pngData = ImageConversion.EncodeToPNG(result);
        File.WriteAllBytes(filePath, pngData);

        UnityEngine.Object.Destroy(result);
      }
      catch (Exception ex)
      {
        Mod.Error($"Failed to save comic panels: {ex}");
      }
    }
  }
}
