#nullable enable

using System;

namespace Bubbles.Core
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
