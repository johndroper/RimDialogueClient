namespace RimDialogue.Core
{
  public class LoginData
  {
    public string tier = string.Empty;
    public int maxPromptLength;
    public int maxResponseLength;
    public int maxOutputWords;
    public float rateLimit;
    public int rateLimitCacheMinutes;
    public int minRateLimitRequestCount;
    public string[] models = [];

    public string Tier
    {
      get => tier;
      set => tier = value;
    }

    public int MaxPromptLength
    {
      get => maxPromptLength;
      set => maxPromptLength = value;
    }
    public int MaxResponseLength
    {
      get => maxResponseLength;
      set => maxResponseLength = value;
    }
    public int MaxOutputWords
    {
      get => maxOutputWords;
      set => maxOutputWords = value;
    }
    public float RateLimit
    {
      get => rateLimit;
      set => rateLimit = value;
    }
    public int RateLimitCacheMinutes
    {
      get => rateLimitCacheMinutes;
      set => rateLimitCacheMinutes = value;
    }
    public int MinRateLimitRequestCount
    {
      get => minRateLimitRequestCount;
      set => minRateLimitRequestCount = value;
    }
    public string[] Models
    {
      get => models;
      set => models = value;
    }
  }
}
