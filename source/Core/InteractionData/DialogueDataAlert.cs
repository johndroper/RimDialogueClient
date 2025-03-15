using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueDataAlert : DialogueData
  {
    public string Explanation = string.Empty;
    public int InitiatorOpinionOfTarget = 0;
    public int RecipientOpinionOfTarget = 0;
  }
}
