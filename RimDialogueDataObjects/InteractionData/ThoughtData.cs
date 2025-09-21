using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core.InteractionData
{
  public class ThoughtData : DialogueTargetData
  {
    public string Label = string.Empty;
    public string Description = string.Empty;
    public string PreceptLabel = string.Empty;
    public string PreceptDescription = string.Empty;
    public float MoodOffset = 0f;
  }
}
