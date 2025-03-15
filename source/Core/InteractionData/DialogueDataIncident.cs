#nullable enable
using System;

namespace RimDialogue.Core.InteractionData
{
  [Serializable]
  public class DialogueDataIncident : DialogueData
  {
    public string Explanation = string.Empty;
    public int InitiatorOpinionOfTarget = 0;
    public int RecipientOpinionOfTarget = 0;
  }
}
