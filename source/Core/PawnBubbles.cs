using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core
{
  public class PawnBubbles
  {
    public PawnBubbles(Pawn pawn)
    {
      Pawn = pawn;
    }

    public Pawn Pawn { get; set; }
    public List<Bubble> Bubbles { get; set; } = new List<Bubble>();

  }
}
