using System;

namespace RimDialogue.Core.InteractionData
{
  [Serializable]
  public class DialogueDataBattle : DialogueData
  {
    public string Name;
    public string Duration;
    public string Importance;
    public string[] Entries = [];
    public string[] Participants = [];
  }
}
