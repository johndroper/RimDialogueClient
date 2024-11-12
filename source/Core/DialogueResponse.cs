#nullable enable

using System;

namespace RimDialogue.Core
{
  [Serializable]
  public class DialogueResponse
  {
    public string? text;

    public string? Text
    {
      get => text;
      set => text = value;
    }
  }
}
