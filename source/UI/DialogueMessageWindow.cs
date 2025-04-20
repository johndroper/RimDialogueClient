#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDialogue.UI
{
  using RimDialogue;
  using RimWorld;
  using UnityEngine;
  using Verse;
  using Mod = RimDialogue.Mod;

  public class DialogueMessageWindow : Window, IExposable
  {
    public const float MinLifeTime = 2f;
    public const float MaxMessageCount = 5f;

    private static GUIStyle? _style;
    public static GUIStyle Style => _style ??= new GUIStyle(Verse.Text.CurFontStyle)
    {
      alignment = TextAnchor.MiddleCenter,
      clipping = TextClipping.Clip
    };

    private const float fadeSpeed = 0.5f;
    private const float scrollSpeed = 6f;
    private const float minAlpha = 0.2f;
    private const float maxAlpha = 0.85f;
    const float labelHeight = 20f;
    const float topMargin = 15f;
    const float bottomMargin = 4f;

    private Vector2 savedSize = new Vector2(200f, 400f);
    private Vector2 savedPosition = new Vector2(100f, 50f);
    private float currentAlpha = 0.2f;
    private float targetAlpha = 0.2f;


    private Vector2 conversationScrollPosition = Vector2.zero;
    private static string agoText = "RimDialogue.Ago".Translate().ToString();

    private float contentRectY = 0;

    private List<Conversation> newConversations = [];

    public DialogueMessageWindow()
    {
      this.doCloseButton = false;
      this.doCloseX = false;
      this.forcePause = false;
      this.draggable = true;
      this.absorbInputAroundWindow = false;
      this.closeOnClickedOutside = false;
      this.drawShadow = false;
      this.doWindowBackground = false;
      this.windowRect = new Rect(savedPosition.x, savedPosition.y, savedSize.x, savedSize.y);
      this.focusWhenOpened = false;
      this.resizeable = true;
      this.closeOnCancel = false;
      this.closeOnAccept = false;
      this.preventCameraMotion = false;
      this.layer = WindowLayer.GameUI;

      GameComponent_ConversationTracker.Instance.ConversationAdded += (s, e) =>
      {
        conversationContentRectHeight = null;
        newConversations.Add(e.Conversation);
      };
    }

    private Dictionary<Conversation, float> conversationHeights = [];

    public float GetConversationHeight(Conversation conversation)
    {
      if (conversationHeights.ContainsKey(conversation))
        return conversationHeights[conversation];
      else
      {
        var contentRectWidth = ContentRectWidth;
        var result = topMargin
            + (conversation.timestamp != null ? labelHeight : 0)
            + (conversation.interaction != null ? Text.CalcHeight(conversation.interaction, contentRectWidth) : 0) + 3f
            + Text.CalcHeight(conversation.text, contentRectWidth)
            + bottomMargin;
        conversationHeights.Add(conversation, result);
        if (Settings.VerboseLogging.Value) Mod.Log($"Conversation height: {result}, contentRectWidth: {contentRectWidth}, contentRectY: {contentRectY}");
        return result;
      }
    }

    public float GetAllHeight(List<Conversation> conversations)
    {
      var allHeight = conversations
        .Sum(conversation => GetConversationHeight(conversation));
      if (Settings.VerboseLogging.Value) Mod.Log($"All {conversations.Count} conversations height: {allHeight}");
      return allHeight;
    }

    public override Vector2 InitialSize => new Vector2(350f, UI.screenHeight - 200f);

    protected override void SetInitialSizeAndPosition()
    {
      Vector2 initialSize = InitialSize;
      windowRect = new Rect(75f, 100f, initialSize.x, initialSize.y);
      windowRect = windowRect.Rounded();
    }

    public float ScrollRectWidth => windowRect.width - 50f;
    public float ContentRectWidth => ScrollRectWidth - 16f;

    float? conversationContentRectHeight = null;
    float? lastWindowWidth;
    float? lastWindowHeight;

    public override void DoWindowContents(Rect inRect)
    {
      //Background
      var contents = new Rect(0, 0, windowRect.width, windowRect.height);
      if (Mouse.IsOver(contents))
        targetAlpha = maxAlpha;
      else
        targetAlpha = minAlpha;
      if (Math.Abs(currentAlpha - targetAlpha) > 0.01f)
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, RealTime.deltaTime * fadeSpeed);
      DrawTransparentBackground();

      var conversationsScrollRect = new Rect(10, 0, ScrollRectWidth, windowRect.height - 40);
      var contentRectWidth = ContentRectWidth;

      if (!lastWindowWidth.HasValue
        || !lastWindowHeight.HasValue
        || this.windowRect.width != lastWindowWidth
        || this.windowRect.height != lastWindowHeight)
      {
        conversationContentRectHeight = null;
        lastWindowWidth = this.windowRect.width;
        lastWindowHeight = this.windowRect.height;
      }

      var conversations = GameComponent_ConversationTracker.Instance.Conversations;
      if (!conversations.Any())
        return;

      Text.Font = GameFont.Small;
      GUI.color = Color.white;
      conversationContentRectHeight ??= GetAllHeight(conversations);

      if (newConversations.Any())
      {
        var height = GetAllHeight(newConversations);
        contentRectY -= height / 2;
        if (Settings.VerboseLogging.Value) Mod.Log($"New contentRectY: {contentRectY}");
        newConversations.Clear();
      }

      if (contentRectY < -1)
        contentRectY += RealTime.deltaTime * scrollSpeed;
      else
        contentRectY = 0;
      var conversationContentRect = new Rect(0, 0, contentRectWidth, conversationContentRectHeight.Value + contentRectY);
      Widgets.BeginScrollView(conversationsScrollRect, ref conversationScrollPosition, conversationContentRect);
      Widgets.BeginGroup(new Rect(0, contentRectY, conversationContentRect.width, conversationContentRect.height + Math.Abs(contentRectY)));
      float convoY = contentRectY;
      for (int i = conversations.Count - 1; i >= 0; i--)
      {
        convoY += topMargin;
        var conversation = conversations[i];
        if (conversation.timestamp != null)
        {
          var periodRect = new Rect(0, convoY, contentRectWidth, 25f);
          Widgets.Label(periodRect, (Find.TickManager.TicksGame - conversation.timestamp ?? 0).ToStringTicksToPeriod() + agoText);
          convoY += labelHeight;
        }
        if (conversation.interaction != null)
        {
          var interactionLabelHeight = Text.CalcHeight(conversation.interaction, contentRectWidth) + 3f;
          var interactionRect = new Rect(0, convoY, contentRectWidth, interactionLabelHeight);
          Widgets.Label(interactionRect, conversation.interaction);
          convoY += interactionLabelHeight;
        }
        string displayText = conversation.text ?? string.Empty;
        float textHeight = Text.CalcHeight(displayText, contentRectWidth);
        var convoRect = new Rect(0, convoY, contentRectWidth, textHeight);
        Widgets.Label(convoRect, displayText);
        convoY += textHeight;
        convoY += bottomMargin;
        Color previousColor = GUI.color;
        GUI.color = Widgets.SeparatorLineColor;
        Widgets.DrawLineHorizontal(0, convoY + bottomMargin, contentRectWidth);
        GUI.color = previousColor;
      }
      Widgets.EndGroup();
      Widgets.EndScrollView();
    }

    private void DrawTransparentBackground()
    {
      Color originalColor = GUI.color;
      GUI.color = new Color(0f, 0f, 0f, currentAlpha);
      Widgets.DrawBoxSolid(new Rect(0f, 0f, windowRect.width, windowRect.height), GUI.color);
      GUI.color = originalColor;
    }

    public void ExposeData()
    {
      Scribe_Values.Look(ref savedSize, "savedSize", new Vector2(200f, 400f));
      Scribe_Values.Look(ref savedPosition, "savedPosition", new Vector2(100f, 50f));
    }
  }
}
