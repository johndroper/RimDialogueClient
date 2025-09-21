#nullable enable
using Bubbles.Core;
using RimDialogue.Core;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimDialogue.UI
{
  public class Window_ComicPanelViewer : Window
  {
    List<Texture2D>? panelTextures;
    private readonly Conversation Conversation;
    private Vector2 scrollPosition = Vector2.zero;

    public const float PanelHeight = 425f;
    public const float PanelWidth = 565f;
    public const float PanelSpacing = 10f;
    public const float SaveButtonHeight = 30f;
    public const float SaveButtonWidth = 120f;
    public const float CopyButtonHeight = 30f;
    public const float TextFieldHeight = 25f;
    public const float BottomUIHeight = 70f;

    public override Vector2 InitialSize => new Vector2(PanelWidth + 50f, PanelHeight + 115f);

    private List<ComicPanel> _Panels = new List<ComicPanel>();

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

      try
      {
        var initiator = Conversation.Initiator;
        var recipient = Conversation.Recipient;

        if (initiator == null)
          throw new Exception("Conversation has no initiator.");

        InitializeDefaultFilePath();

        List<ComicPanelItem> backgroundItems = GetBackgroundItems(PanelWidth, PanelHeight - ComicPanel.PortraitHeight);

        if (!Conversation.Lines.Any())
          Mod.Error("Conversation has no lines.");
        if (recipient == null)
        {
          if (Settings.VerboseLogging.Value) Mod.Log($"Creating single pawn comic panels for conversation with initiator {initiator.ToString() ?? "Unknown"} ({initiator.thingIDNumber.ToString() ?? "Unknown"})");
          for (int i = 0; i < Conversation.Lines.Length; i++)
          {
            _Panels.Add(new SinglePawnComicPanel(
              i == 0 ? Conversation.Interaction : null,
              initiator,
              bitmapFont,
              Conversation.Lines[i].Text,
              SkyHeight,
              backgroundItems));
            if (Settings.VerboseLogging.Value)
              Mod.Log($"SinglePawnComicPanel panel added for line {i}.");
          }
        }
        else
        {
          if (Settings.VerboseLogging.Value) Mod.Log($"Creating two pawn comic panels for conversation between {Conversation.Initiator?.ToString() ?? "Unknown"} ({Conversation.Initiator?.thingIDNumber.ToString() ?? "Unknown"}) and {Conversation.Recipient?.ToString() ?? "Unknown"} ({Conversation.Recipient?.thingIDNumber.ToString() ?? "Unknown"})");
          for (int i = 0; i < Conversation.Lines.Length; i++)
          {
            if (Conversation.Lines[i].Name == recipient.Name.ToStringShort)
            {
              _Panels.Add(
                new TwoPawnComicPanel(
                  i == 0 ? Conversation.Interaction : null,
                  initiator,
                  null,
                  recipient,
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
                      i == 0 ? Conversation.Interaction : null,
                      initiator,
                      Conversation.Lines[i].Text,
                      recipient,
                      null,
                      SkyHeight,
                      bitmapFont,
                      backgroundItems));
                }
                else
                {
                  _Panels.Add(
                    new TwoPawnComicPanel(
                      i == 0 ? Conversation.Interaction : null,
                      initiator,
                      Conversation.Lines[i].Text,
                      recipient,
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
                    i == 0 ? Conversation.Interaction : null,
                    initiator,
                    Conversation.Lines[i].Text,
                    recipient,
                    null,
                    SkyHeight,
                    bitmapFont,
                    backgroundItems));
            }
          }
        }
      }
      catch (Exception ex)
      {
        Mod.Error($"Failed to create comic panels: {ex}");
      }
    }

    public List<ComicPanelItem> GetBackgroundItems(float width, float skyHeight)
    {
      List<ComicPanelItem> items = new List<ComicPanelItem>();
      List<Thing> treeThings = new List<Thing>();
      var trees = Find.CurrentMap.listerThings.AllThings
        .Where(t => t.def.category == ThingCategory.Plant && t.def.plant.IsTree);
      if (trees.Any())
      {
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
      }
      return items;
    }

    private void InitializeDefaultFilePath()
    {
      try
      {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string participantNames = Conversation.Recipient != null
          ? $"{Conversation.Initiator?.Name.ToStringShort ?? "RimDialogue.Unknown".Translate()}_{Conversation.Recipient?.Name.ToStringShort ?? "RimDialogue.Unknown".Translate()}"
          : Conversation.Initiator?.Name.ToStringShort ?? "RimDialogue.Unknown".Translate();

        foreach (char c in Path.GetInvalidFileNameChars())
        {
          participantNames = participantNames.Replace(c, '_');
        }

        string filename = $"{participantNames}_{timestamp}.png";

        string screenshotsPath = Settings.DefaultSavePath.Value;
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

        Rect scrollRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - BottomUIHeight);

        float totalHeight = _Panels.Count * (PanelHeight + PanelSpacing);
        Rect viewRect = new Rect(0, 0, scrollRect.width - 20, totalHeight);

        Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect, true);

        float curY = 0;
        foreach (var panel in panelTextures)
        {
          Rect panelRect = new Rect(0, curY, viewRect.width, PanelHeight);
          GUI.DrawTexture(panelRect, panel, ScaleMode.StretchToFill, true, 1f);
          curY += PanelHeight + PanelSpacing;
        }

        Widgets.EndScrollView();

        //Rect pathLabelRect = new Rect(10f, curY, 100f, 20f);
        //Widgets.Label(pathLabelRect, "Save Path:");

        Rect pathTextRect = new Rect(0, scrollRect.height, inRect.width - SaveButtonWidth - 10f, TextFieldHeight);
        string newPath = Widgets.TextField(pathTextRect, filePathBuffer);
        if (newPath != filePathBuffer)
        {
          filePathBuffer = newPath;
          customFilePath = newPath;
        }

        // Save as PNG button
        Rect saveButtonRect = new Rect(inRect.width - SaveButtonWidth, pathTextRect.y, SaveButtonWidth, SaveButtonHeight);
        if (Widgets.ButtonText(saveButtonRect, "Save as PNG"))
        {
          SavePanelsToPNG(viewRect, newPath);
          Settings.DefaultSavePath.Value = Path.GetDirectoryName(customFilePath);
          SoundDefOf.Tick_High.PlayOneShotOnCamera();
        }

        // Copy to Clipboard button (Windows only)
        if (PlatformCheck.IsWindows())
        {
          Rect copyButtonRect = new Rect(0, pathTextRect.y + SaveButtonHeight + 5, inRect.width, CopyButtonHeight);
          if (Widgets.ButtonText(copyButtonRect, "Copy Image To Clipboard"))
          {
            CopyToClipboard(viewRect);
            SoundDefOf.Tick_High.PlayOneShotOnCamera();
          }
        }
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public Texture2D ToTexture(Rect viewRect)
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
      GL.Clear(true, true, UnityEngine.Color.black);

      float curY = 0;
      foreach (var panelTexture in panelTextures)
      {
        Rect destRect = new Rect(0, curY, panelTexture.width, panelTexture.height);
        UnityEngine.Graphics.DrawTexture(destRect, panelTexture);
        curY += PanelHeight + PanelSpacing;
      }

      Texture2D finalTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
      finalTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
      finalTex.Apply();

      GL.PopMatrix();
      RenderTexture.active = null;
      rt.Release();
      UnityEngine.Object.Destroy(rt);

      return finalTex;
    }


    public void CopyToClipboard(Rect viewRect)
    {
      var texture = ToTexture(viewRect);
      Win32Clipboard.CopyImage(texture);
    }

    private void SavePanelsToPNG(Rect viewRect, string filePath)
    {
      try
      {
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
          Directory.CreateDirectory(directory);
        var texture = ToTexture(viewRect);
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, pngData);
      }
      catch (Exception ex)
      {
        Mod.Error($"Failed to save comic panels: {ex}");
      }
    }
  }
}
