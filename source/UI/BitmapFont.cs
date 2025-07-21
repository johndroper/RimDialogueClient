using Bubbles;
using RimDialogue.Core;
using RimDialogue.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

public enum FontFace
{
  Calibri,
  NotoSansSC
}

[StaticConstructorOnStartup]
public class BitmapFont
{
  private static Texture2D _calibriAtlas;
  private static Texture2D CalibriAtlas => _calibriAtlas ??= ContentFinder<Texture2D>.Get("RimDialogue/Calibri_0");
  private static readonly string calibriPath = Path.Combine(RimDialogue.Mod.FontPath, "calibri.fnt");
  public static readonly BitmapFont Calibri = BitmapFont.Load(calibriPath, CalibriAtlas);

  private static Texture2D _notoSansSCAtlas;
  private static Texture2D NotoSansSCAtlas => _notoSansSCAtlas ??= ContentFinder<Texture2D>.Get("RimDialogue/Noto_Sans_SC_0");
  private static readonly string notoSansSCPath = Path.Combine(RimDialogue.Mod.FontPath, "Noto_Sans_SC.fnt");
  public static readonly BitmapFont NotoSansSC = BitmapFont.Load(notoSansSCPath, NotoSansSCAtlas);

  public static BitmapFont Get(FontFace fontFace)
  {
    return fontFace switch
    {
      FontFace.Calibri => Calibri,
      FontFace.NotoSansSC => NotoSansSC,
      _ => Calibri
    };
  }

  public Dictionary<int, Glyph> glyphs = new();
  public int lineHeight;
  public Texture2D atlas;
  public int atlasWidth, atlasHeight;

  public static BitmapFont Load(string fntPath, Texture2D atlas)
  {
    var font = new BitmapFont();
    font.atlas = atlas;
    font.atlasWidth = atlas.width;
    font.atlasHeight = atlas.height;

    foreach (string line in File.ReadAllLines(fntPath))
    {
      if (line.StartsWith("char "))
      {
        var glyph = new Glyph();
        foreach (var part in line.Split(' '))
        {
          if (string.IsNullOrWhiteSpace(part)) continue;
          var kv = part.Split('=');
          if (kv.Length != 2) continue;

          string key = kv[0];
          string val = kv[1];

          switch (key)
          {
            case "id": glyph.id = int.Parse(val); break;
            case "x": glyph.x = int.Parse(val); break;
            case "y": glyph.y = int.Parse(val); break;
            case "width": glyph.width = int.Parse(val); break;
            case "height": glyph.height = int.Parse(val); break;
            case "xoffset": glyph.xOffset = int.Parse(val); break;
            case "yoffset": glyph.yOffset = int.Parse(val); break;
            case "xadvance": glyph.xAdvance = int.Parse(val); break;
          }
        }
        font.glyphs[glyph.id] = glyph;
      }
      else if (line.StartsWith("common "))
      {
        foreach (var part in line.Split(' '))
        {
          if (string.IsNullOrWhiteSpace(part)) continue;
          var kv = part.Split('=');
          if (kv.Length != 2) continue;

          if (kv[0] == "lineHeight")
            font.lineHeight = int.Parse(kv[1]);
        }
      }
    }

    return font;
  }

  public float GetWordWidth(string word)
  {
    float wordWidth = 0f;
    foreach (char c in word)
    {
      if (glyphs.TryGetValue(c, out var glyph))
      {
        wordWidth += glyph.xAdvance;
      }
    }
    return wordWidth;
  }


  public float GetTextHeight(
      string text,
      float maxWidth)
  {
    if (string.IsNullOrEmpty(text)) return 0f;

    float spaceWidth = glyphs.TryGetValue(' ', out var spaceGlyph) ? spaceGlyph.xAdvance : 4f;

    string[] words = text.Split([' ', '\n', '\u200B'], StringSplitOptions.RemoveEmptyEntries);
    words = Conversation.BreakWords(words, 10).ToArray();
    float currentLineWidth = 0f;
    int lineCount = 1;

    foreach (string word in words)
    {
      float wordWidth = GetWordWidth(word);

      if (currentLineWidth + wordWidth + spaceWidth > maxWidth)
      {
        lineCount++;
        currentLineWidth = wordWidth + spaceWidth;
      }
      else
      {
        currentLineWidth += wordWidth + spaceWidth;
      }
    }

    RimDialogue.Mod.Log($"Calculated text height with max width {maxWidth}: {lineCount} lines, line height {lineHeight}.");

    return lineCount * lineHeight;
  }

  public void DrawText(
      string text,
      Vector2 startPos,
      float maxWidth)
  {
    if (string.IsNullOrEmpty(text)) return;

    Vector2 cursor = startPos;
    string[] words = text.Split([' ', '\n', '\u200B'], StringSplitOptions.RemoveEmptyEntries);
    words = Conversation.BreakWords(words, 10).ToArray();
    float spaceWidth = glyphs.TryGetValue(' ', out var spaceGlyph) ? spaceGlyph.xAdvance : 4f;

    List<string> lines = new List<string>();
    string currentLine = "";
    float currentLineWidth = 0f;

    foreach (string word in words)
    {
      float wordWidth = 0f;
      foreach (char c in word)
      {
        if (glyphs.TryGetValue(c, out var glyph))
        {
          wordWidth += glyph.xAdvance;
        }
      }

      if (currentLineWidth + wordWidth + spaceWidth > maxWidth)
      {
        lines.Add(currentLine.TrimEnd());
        currentLine = word + " ";
        currentLineWidth = wordWidth + spaceWidth;
      }
      else
      {
        currentLine += word + " ";
        currentLineWidth += wordWidth + spaceWidth;
      }
    }

    if (!string.IsNullOrWhiteSpace(currentLine))
      lines.Add(currentLine.TrimEnd());

    foreach (string line in lines)
    {
      Vector2 lineCursor = new Vector2(cursor.x, cursor.y);
      foreach (char c in line)
      {
        if (!glyphs.TryGetValue(c, out var glyph))
        {
          RimDialogue.Mod.Warning($"Character '{c}' {(int)c} not found.");
          glyph = spaceGlyph;
        }

        Rect dest = new Rect(
            lineCursor.x + glyph.xOffset,
            lineCursor.y + glyph.yOffset,
            glyph.width,
            glyph.height);

        Rect uv = glyph.GetUVRect(atlasWidth, atlasHeight);
        Graphics.DrawTexture(dest, atlas, uv, 0, 0, 0, 0);
        lineCursor.x += glyph.xAdvance;
      }
      cursor.y += lineHeight;
    }
  }
}
