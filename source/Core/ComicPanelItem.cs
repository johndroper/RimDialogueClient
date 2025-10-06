using UnityEngine;

namespace RimDialogue.Core
{
  public class ComicPanelItem
  {
    public Texture2D Texture { get; set; }
    public Rect Rect { get; set; }

    public ComicPanelItem(Texture2D texture, Rect rect)
    {
      Texture = texture;
      Rect = rect;
    }
  }
}
