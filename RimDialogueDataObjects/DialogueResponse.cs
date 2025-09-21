#nullable enable

using System;

namespace RimDialogue.Core
{
  [Serializable]
  public class DialogueResponse : PromptResponse
  {
    public bool rateLimited = false;
    public float rate = 0f;

    public bool RateLimited
    {
      get => rateLimited;
      set => rateLimited = value;
    }

    public float Rate
    {
      get => rate;
      set => rate = value;
    }
  }
}
