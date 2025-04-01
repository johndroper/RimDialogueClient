using System;

namespace RimDialogue.Core.InteractionData
{
  [Serializable]
  public class DialogueDataBattle : DialogueData
  {
    public string Name = string.Empty;
    public string Duration = string.Empty;
    public string Importance = string.Empty;
    public string[] Entries = [];
    public string[] Participants = [];
  }
}
