#nullable enable 

using RimDialogue.Access;
using RimDialogue.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimDialogue.UI
{
  //displays one message at a time , with a fade out
  public class DialogueMessagesFixed
  {
    private static GUIStyle? _style;
    public static GUIStyle Style => _style ??= new GUIStyle(Verse.Text.CurFontStyle)
    {
      alignment = TextAnchor.MiddleCenter,
      clipping = TextClipping.Clip
    };

    public List<DialogueMessage> Messages = [];
    public const float MinLifeTime = 2f;
    public const float MaxMessageCount = 5f;

    public DialogueMessagesFixed()
    {
      GameComponent_ConversationTracker.Instance.ConversationAdded += (s, e) =>
      {
        if (Settings.DialogueMessageInterface.Value == 2 && e.Conversation.text != null)
          AddMessage(e.Conversation.interaction + "\r\n" + e.Conversation.text, null);
      };
    }

    public float LifeTime
    {
      get
      {
        float factor = Messages.Count / MaxMessageCount;
        if (factor < 1)
          return Settings.DialogueMessageLifetime.Value;
        var lifetime = Settings.DialogueMessageLifetime.Value / factor;
        if (lifetime < MinLifeTime)
          return MinLifeTime;
        return lifetime;
      }
    }

    public DialogueMessage? CurrentMessage { get; set; } = null;

    public void AddMessage(string message, LookTargets? targets)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Adding dialogue message {message}.");
      var newMessage = new DialogueMessage(message, targets);
      Messages.Add(newMessage);
    }

    public void DoGUI()
    {
      Text.Font = GameFont.Small;
      if (CurrentMessage == null || CurrentMessage.TimeLeft < 0)
      {
        if (Messages.Any())
        {
          CurrentMessage = Messages[0];
          Messages.RemoveAt(0);
          CurrentMessage.Start(LifeTime);
        }
        else
          CurrentMessage = null;
      }
      if (CurrentMessage != null)
        CurrentMessage.Draw();
    }
  }

  public class DialogueMessage
  {
    private float startingTime;
    private float width = Settings.DialogueMessageWidth.Value;
    private float height;
    private Rect rect;
    private string dialogue { get; set; }
    private LookTargets? LookTargets { get; set; }
    private float LifeTime { get; set; }
    public float Age => RealTime.LastRealTime - startingTime;
    public float TimeLeft => LifeTime - Age;

    List<Vector2> cachedDrawLocs;

    public DialogueMessage(string dialogue, LookTargets? targets)
    {

      this.dialogue = dialogue;
      LookTargets = targets;
      Text.Font = GameFont.Small;
      height = Text.CalcHeight(dialogue, width);
      cachedDrawLocs = (List<Vector2>)Reflection.RimWorld_ColonistBar_CachedDrawLocs.GetValue(Find.ColonistBar);
      float maxY = cachedDrawLocs.Max(entry => entry.y);
      rect = new Rect(Verse.UI.screenWidth / 2f - width / 2, maxY + Find.ColonistBar.Size.y + 10f, width + 12f, height + 4f);
      // if (Settings.VerboseLogging.Value) Mod.Log($"Message Rect: {rect}");
    }

    public void Start(float lifeTime)
    {
      this.LifeTime = lifeTime;
      startingTime = RealTime.LastRealTime;
      // if (Settings.VerboseLogging.Value) Mod.Log($"Message starting: {startingTime}");
    }

    public float Alpha
    {
      get
      {
        //fade in
        if (Age < 0.6f)
        {
          return Age / 0.6f;
        }

        //fade out
        if (TimeLeft < 0.6f)
        {
          return TimeLeft / 0.6f;
        }

        return 1f;
      }
    }
    private static bool ShouldDrawBackground
    {
      get
      {
        return true;
      }
    }

    public void Draw()
    {
      Text.Font = GameFont.Small;
      GUI.color = Color.white.ToTransparent(Alpha);
      GUI.Label(rect, dialogue, DialogueMessagesFixed.Style);
    }
  }
}
