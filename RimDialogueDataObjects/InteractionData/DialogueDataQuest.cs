namespace RimDialogue.Core.InteractionData
{
  public class DialogueDataQuest : DialogueData
  {
    public string QuestName = string.Empty;
    public string QuestDescription = string.Empty;
    public string[] QuestTags = [];
    public bool Charity = false;
    public string PeriodSinceAccepted = string.Empty;
    public string PeriodUntilExpiry = string.Empty;
    public string QuestState = string.Empty;
    public string AccepterPawn = string.Empty;

  }
}
