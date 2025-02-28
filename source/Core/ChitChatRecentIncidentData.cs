#nullable enable

using System;

namespace RimDialogue.Core
{
  [Serializable]
  public class ChitChatRecentIncidentData
  {
    public string interaction = String.Empty;
    public string letterTitle = String.Empty;
    public string letterText = String.Empty;
    public string scenario = string.Empty;
    public string scenarioInstructions = string.Empty;
    public int initiatorOpinionOfRecipient = 0;
    public int recipientOpinionOfInitiator = 0;
    public int maxWords = -1;

    public ChitChatRecentIncidentData()
    {

    }

    public ChitChatRecentIncidentData(
      int initiatorOpinionOfRecipient,
      int recipientOpinionOfInitiator,
      string interaction,
      string scenarioInstructions,
      string letterTitle,
      string letterText,
      int maxWords)
    {
      this.interaction = interaction;
      this.scenario = scenarioInstructions;
      this.scenarioInstructions = scenarioInstructions;
      this.letterTitle = letterTitle;
      this.letterText = letterText;
      this.maxWords = maxWords;
      this.initiatorOpinionOfRecipient = initiatorOpinionOfRecipient;
      this.recipientOpinionOfInitiator = recipientOpinionOfInitiator;
    }
  }
}
