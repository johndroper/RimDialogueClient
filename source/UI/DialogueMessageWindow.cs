#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDialogue.UI
{
  using RimDialogue;
  using RimDialogue.Core;
  using RimWorld;
  using UnityEngine;
  using Verse;
  using Verse.Sound;
  using Mod = RimDialogue.Mod;


  public class ConversationLabel
  {
    public const float LabelHeight = 20f;
    public const float TopMargin = 15f;
    public const float BottomMargin = 4f;

    private static string agoText = "RimDialogue.Ago".Translate().ToString();

    public Conversation Conversation { get; set; }
    public ConversationLabel(Conversation conversation)
    {
      Conversation = conversation;
    }

    private float lastContentRectWidth = 0;
    private float? _height;

    public float GetHeight(float contentRectWidth)
    {
      if (lastContentRectWidth != contentRectWidth)
      {
        _height = null;
        _interactionHeight = null;
        _textHeight = null;
      }
      _height ??= TopMargin
        + (Conversation.timestamp != null ? LabelHeight : 0)
        + GetInteractionHeight(contentRectWidth)
        + GetTextHeight(contentRectWidth)
        + BottomMargin;
      return _height.Value;
    }

    private float? _interactionHeight;
    public float GetInteractionHeight(float contentRectWidth)
    {
      Text.Font = GameFont.Small;
      if (Conversation.interaction == null)
        return 0f;
        _interactionHeight ??= Text.CalcHeight(Conversation.interaction, contentRectWidth) + 3f;
      return _interactionHeight.Value;
    }

    private float? _textHeight;
    public float GetTextHeight(float contentRectWidth)
    {
      Text.Font = GameFont.Small;
      _textHeight ??= Text.CalcHeight(Conversation.text, contentRectWidth);
      return _textHeight.Value;
    }

    public float Draw(float currentY, float contentRectWidth)
    {
      GUI.color = Widgets.SeparatorLineColor;
      Widgets.DrawLineHorizontal(0, currentY, contentRectWidth);
      Text.Font = GameFont.Small;
      GUI.color = Color.white;

      string text = Conversation.text ?? string.Empty;
      float textHeight = GetTextHeight(contentRectWidth);
      currentY -= textHeight + BottomMargin;
      var textRect = new Rect(0, currentY, contentRectWidth, textHeight);
      Widgets.Label(textRect, text);
      if (Conversation.interaction != null)
      {
        var interactionLabelHeight = GetInteractionHeight(contentRectWidth);
        currentY -= interactionLabelHeight;
        var interactionRect = new Rect(0, currentY, contentRectWidth, interactionLabelHeight);
        Widgets.Label(interactionRect, Conversation.interaction);
      }
      if (Conversation.timestamp != null)
      {
        currentY -= LabelHeight;
        var periodRect = new Rect(0, currentY, contentRectWidth, LabelHeight);
        Widgets.Label(periodRect, (Find.TickManager.TicksGame - Conversation.timestamp ?? 0).ToStringTicksToPeriod() + agoText);
      }

      var copyButtonRect = new Rect(contentRectWidth - 90, currentY, 40, LabelHeight);
      if (Widgets.ButtonText(copyButtonRect, "Copy"))
      {
        GUIUtility.systemCopyBuffer = Conversation.text ?? string.Empty;
        SoundDefOf.Click.PlayOneShotOnCamera();
      }

      var memeButtonRect = new Rect(contentRectWidth - 40, currentY, 40, LabelHeight);
      if (Widgets.ButtonText(memeButtonRect, "Meme"))
      {
        Find.WindowStack.Add(new Window_ComicPanelViewer(Conversation));
      }

      currentY -= TopMargin;
      return currentY;
    }
  }

  public class DialogueMessageWindow : Window
  {
    public const float MinLifeTime = 2f;
    public const float MaxMessageCount = 5f;

    private const float fadeSpeed = 0.5f;
    private const float minAlpha = 0.2f;
    private const float maxAlpha = 0.85f;

    private float currentAlpha = 0.2f;
    private float targetAlpha = 0.2f;

    private float scrollPosition = 0f;

    public List<ConversationLabel> ConversationLabels { get; set; }

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
      this.focusWhenOpened = false;
      this.resizeable = true;
      this.closeOnCancel = false;
      this.closeOnAccept = false;
      this.preventCameraMotion = false;
      this.layer = WindowLayer.GameUI;

      var savedSize = GameComponent_ConversationTracker.Instance.MessageWindowSize;
      var savedPosition = GameComponent_ConversationTracker.Instance.MessageWindowPosition;
      this.windowRect = new Rect(savedPosition.x, savedPosition.y, savedSize.x, savedSize.y);

      ConversationLabels = GameComponent_ConversationTracker.Instance.Conversations
        .Select(conversation => new ConversationLabel(conversation))
        .ToList();

      GameComponent_ConversationTracker.Instance.ConversationAdded += (s, e) =>
      {
        var newLabel = new ConversationLabel(e.Conversation);
        //_newConversations.Add(newLabel);
        ConversationLabels.Add(newLabel);
      };

      GameComponent_ConversationTracker.Instance.ConversationRemoved += (s, e) =>
      {
        var removedHeight = ConversationLabels.Where(label => label.Conversation == e.Conversation).Sum(label => label.GetHeight(ContentRectWidth));
        scrollPosition += removedHeight;
        ConversationLabels.RemoveAll(label => label.Conversation == e.Conversation);
      };
    }

    public float GetAllHeight(List<ConversationLabel> conversationLabels)
    {
      var contentRectWidth = ContentRectWidth;
      var allHeight = conversationLabels
        .Sum(conversationLabel => conversationLabel.GetHeight(contentRectWidth));
      //if (Settings.VerboseLogging.Value) Mod.Log($"All {conversations.Count} conversations height: {allHeight}");
      return allHeight;
    }

    public override void WindowUpdate()
    {
      base.WindowUpdate();
    }

    protected override void SetInitialSizeAndPosition()
    {
      var savedSize = GameComponent_ConversationTracker.Instance.MessageWindowSize;
      var savedPosition = GameComponent_ConversationTracker.Instance.MessageWindowPosition;
      windowRect = new Rect(savedPosition.x, savedPosition.y, savedSize.x, savedSize.y);
      windowRect = windowRect.Rounded();
      if (ConversationLabels.Any())
        scrollPosition = windowRect.height - GetAllHeight(ConversationLabels);
      else
        scrollPosition = windowRect.height;
    }

    public float ContentRectWidth => windowRect.width - 50f;

    Vector2? LastWindowSize;
    Vector2? LastWindowPosition;

    public override void DoWindowContents(Rect inRect)
    {
      //Background
      var contents = new Rect(0, 0, windowRect.width, windowRect.height);
      bool mouseOver = Mouse.IsOver(contents);
      if (mouseOver)
        targetAlpha = maxAlpha;
      else
        targetAlpha = minAlpha;
      if (Math.Abs(currentAlpha - targetAlpha) > 0.01f)
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, RealTime.deltaTime * fadeSpeed);
      DrawTransparentBackground();

      Text.Font = GameFont.Small;
      GUI.color = Color.white;
      var contentRectWidth = ContentRectWidth;
      var conversationContentRectHeight = GetAllHeight(ConversationLabels);

      if (this.windowRect.position != LastWindowPosition)
      {
        LastWindowPosition = this.windowRect.position;
        //save the window position for later serialization
        GameComponent_ConversationTracker.Instance.MessageWindowPosition = new Vector2(this.windowRect.x, this.windowRect.y);
      }

      if (LastWindowSize != this.windowRect.size)
      {
        LastWindowSize = this.windowRect.size;
        //save the window size for later serialization
        GameComponent_ConversationTracker.Instance.MessageWindowSize = new Vector2(this.windowRect.width, this.windowRect.height);
        if (this.windowRect.height < scrollPosition)
          scrollPosition = this.windowRect.height;
        if (this.windowRect.height > scrollPosition + conversationContentRectHeight)
          scrollPosition += this.windowRect.height - (scrollPosition + conversationContentRectHeight);
      }

      if (!ConversationLabels.Any())
        return;

      var conversationContentRect = new Rect(10, scrollPosition - ConversationLabel.TopMargin, contentRectWidth, conversationContentRectHeight);
      Widgets.BeginGroup(conversationContentRect);
      float convoY = conversationContentRect.height;
      Color previousColor = GUI.color;
      for (int i = ConversationLabels.Count - 1; i >= 0; i--)
      {
        convoY = ConversationLabels[i].Draw(convoY, conversationContentRect.width);
        if (convoY < 0)
          break;
      }
      Widgets.EndGroup();

      if (!mouseOver && conversationContentRect.y + conversationContentRect.height > windowRect.height - 40)
        scrollPosition -= RealTime.deltaTime * Math.Max(1f, Find.TickManager.TickRateMultiplier) * Settings.MessageScrollSpeed.Value * (conversationContentRect.height > this.windowRect.height ? Math.Max(1f, (conversationContentRect.height + scrollPosition + 500) / 500) : 1) ;

      var titleBarRect = new Rect(0, 0, this.windowRect.width, 30);
      Widgets.DrawBoxSolid(titleBarRect, Color.white.ToTransparent(.85f));
      GUI.color = Color.black;
      var titleLabelRect = new Rect(titleBarRect.x, titleBarRect.y, 50, 20);
      Widgets.Label(titleLabelRect, "RimDialogue");

      var debugInfoRect1 = new Rect(titleBarRect.x + 50, titleBarRect.y, 30, 20);
      Widgets.Label(debugInfoRect1, scrollPosition.ToString("N0"));
      var debugInfoRect2 = new Rect(debugInfoRect1.x + debugInfoRect1.width + 10, titleBarRect.y, 30, 20);
      Widgets.Label(debugInfoRect2, conversationContentRectHeight.ToString("N0"));

      GUI.color = previousColor;
    }

    private void DrawTransparentBackground()
    {
      Color originalColor = GUI.color;
      GUI.color = new Color(0f, 0f, 0f, currentAlpha);
      Widgets.DrawBoxSolid(new Rect(0f, 0f, windowRect.width, windowRect.height), GUI.color);
      GUI.color = originalColor;
    }
  }
}
