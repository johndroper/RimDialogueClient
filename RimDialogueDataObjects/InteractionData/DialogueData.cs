namespace RimDialogue.Core.InteractionData
{
  public class DialogueData
  {
    public string LanguageEnglish = "English";
    public string LanguageNative = "English";
    public string ModelName = "Default";
    public string Interaction = string.Empty;
    public string ClientId = string.Empty;
    public string Instructions = string.Empty;
    public int MaxWords = -1;
    public int MinWords = -1;
    public int InitiatorOpinionOfRecipient = 0;
    public int RecipientOpinionOfInitiator = 0;

    public string TotalColonyTime = string.Empty;
    public int HourOfDay = 0;
    public string WeatherLabel = string.Empty;
    public string WeatherDescription = string.Empty;
    public string BiomeLabel = string.Empty;
    public string BiomeDescription = string.Empty;
    public string Season = string.Empty;
    public float OutdoorTemp = float.MinValue;

    public string[] Context = [];
    public string[] TemporalContext = [];

    //public string[] GlobalContext = [];
    //public string[] InitiatorContext = [];
    //public string[] RecipientContext = [];

  }
}
