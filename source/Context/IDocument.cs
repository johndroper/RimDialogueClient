using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RimDialogue.Context
{
  public interface IDocument
  {
    public string Text { get; }

    public int Tick { get; }

    public float Weight { get; }

    public int AccessCount { get; }

    public void Accessed();
  }
}
