using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
