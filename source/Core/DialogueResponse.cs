#nullable enable

using System;

namespace RimDialogue.Core
{
  [Serializable]
  public class DialogueResponse
  {
    public string? text;
    public bool rateLimited = false;

    public string? Text
    {
      get => text;
      set => text = value;
    }

    public bool RateLimited
    {
      get => rateLimited;
      set => rateLimited = value;
    }
  }
}
