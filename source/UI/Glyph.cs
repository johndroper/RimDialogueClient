using UnityEngine;

namespace RimDialogue.UI
{
  public class Glyph
  {
    public int id;
    public int x, y, width, height;
    public int xOffset, yOffset, xAdvance;

    public Rect GetUVRect(int atlasWidth, int atlasHeight)
    {
      return new Rect(
          x / (float)atlasWidth,
          1f - (y + height) / (float)atlasHeight,
          width / (float)atlasWidth,
          height / (float)atlasHeight
      );
    }
  }
}
