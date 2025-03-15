using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  [Serializable]
  public class DialogueDataCondition : DialogueData
  {
    public string Explanation = string.Empty;
    public string LabelCap = string.Empty;
    public string TooltipString = string.Empty;
    public string Duration = string.Empty;
    public int DurationTicks = 0;
    public bool Permanent = false;
  }
}
