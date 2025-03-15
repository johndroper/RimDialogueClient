using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueData
  {
    public string Interaction = string.Empty;
    public string ClientId = string.Empty;
    public string Instructions = string.Empty;
    public int MaxWords = -1;
    public int InitiatorOpinionOfRecipient = 0;
    public int RecipientOpinionOfInitiator = 0;
  }


}
