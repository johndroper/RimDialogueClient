#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core
{
  public class PromptResponse
  {
    public string? text;

    public string? Text
    {
      get => text;
      set => text = value;
    }

  }
}
