#nullable enable
using RimDialogue;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using RimDialogue.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

public class ConversationArgs : EventArgs
{
  public Conversation Conversation { get; }

  public ConversationArgs(Conversation conversation)
  {
    Conversation = conversation;
  }
}

public class GameComponent_ConversationTracker : GameComponent
{
  public static GameComponent_ConversationTracker Instance =>
    Current.Game.GetComponent<GameComponent_ConversationTracker>();

  public event EventHandler<ConversationArgs>? ConversationAdded;
  public event EventHandler<ConversationArgs>? ConversationRemoved;

  private Dictionary<string, string> additionalInstructions;
  private List<Conversation> conversations;
  private bool windowOpened = false;
  public DialogueMessageWindow? DialogueMessageWindow { get; private set; }
  public Vector2 MessageWindowSize = new Vector2(350f, Math.Max(UI.screenHeight - 300f, 200f));
  public Vector2 MessageWindowPosition = new Vector2(75f, 100f);
  public DialogueMessagesFixed? DialogueMessagesFixed { get; private set; }
  public GameComponent_ConversationTracker(Game game)
  {
    conversations = [];
    additionalInstructions = [];
  }
  public override void FinalizeInit()
  {
    base.FinalizeInit();

    if (conversations == null)
      conversations = [];
    if (additionalInstructions == null)
      additionalInstructions = [];
    if (DialogueMessagesFixed == null)
      DialogueMessagesFixed = new DialogueMessagesFixed();
  }

  public override void StartedNewGame()
  {
    base.StartedNewGame();
    additionalInstructions[InstructionsSet.ALL_PAWNS] = "RimDialogue.DefaultInstructions".Translate();
    GetScenarioInstructions(Find.Scenario?.name + "\r\n" + H.RemoveWhiteSpace(Find.Scenario?.description));
    GetInstructions();
  }

  public override void GameComponentUpdate()
  {
    // Wait until game is fully loaded and window hasn't been shown yet
    if (Settings.DialogueMessageInterface.Value == 1 && !windowOpened && Current.ProgramState == ProgramState.Playing)
    {
      DialogueMessageWindow = new DialogueMessageWindow();
      Find.WindowStack.Add(DialogueMessageWindow);
      windowOpened = true;
    }
    else if (Settings.DialogueMessageInterface.Value != 1 && DialogueMessageWindow != null)
    {
      DialogueMessageWindow.Close();
      DialogueMessageWindow = null;
      windowOpened = false;
    }
  }

  private async void GetInstructions()
  {
    try
    {
      var colonists = Find.CurrentMap.mapPawns.FreeColonists.ListFullCopy();
      foreach (var pawn in colonists)
      {
        await GetPawnInstructions(pawn);
        await Task.Delay(1000);
      }
    }
    catch (Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching pawn Instructions: " + ex.ToString());
    }
  }

  private async Task GetPawnInstructions(Pawn pawn)
  {
    var pawnData = H.MakePawnData(pawn, null);
    WWWForm form = new WWWForm();
    form.AddField("clientId", Settings.ClientId.Value);
    form.AddField("pawnJson", JsonUtility.ToJson(pawnData));
    var dialogueResponse = await DialogueRequest.Post("home/GetCharacterPrompt", form);
    if (dialogueResponse.text != null)
      additionalInstructions[pawn.ThingID] = dialogueResponse.text;
  }

  private async void GetScenarioInstructions(string scenarioText)
  {
    try
    {
      WWWForm form = new WWWForm();
      form.AddField("clientId", Settings.ClientId.Value);
      form.AddField("scenarioText", scenarioText);
      var dialogueResponse = await DialogueRequest.Post("home/GetScenarioPrompt", form);
      if (dialogueResponse.text != null)
        additionalInstructions[InstructionsSet.COLONISTS] = dialogueResponse.text;
    }
    catch (Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching ScenarioInstructions: " + ex.ToString());
    }
  }

  public void AddConversation(Pawn initiator, Pawn? recipient, string? interaction, string? text)
  {
    if (initiator == null || recipient == null || text == null || string.IsNullOrWhiteSpace(text))
      return;
    Conversation conversation = new Conversation(initiator, recipient, interaction, text);
    lock (conversations)
    {
      while (conversations.Count > Settings.MaxConversationsStored.Value)
      {
        Conversation removed = conversations[0];
        conversations.RemoveAt(0);
        ConversationRemoved?.Invoke(this, new ConversationArgs(removed));
      }
      conversations.Add(conversation);
    }
    ConversationAdded?.Invoke(this, new ConversationArgs(conversation));
  }

  public void AddAdditionalInstructions(Pawn pawn, string value)
  {
    AddAdditionalInstructions(pawn.ThingID, value);
  }

  public void AddAdditionalInstructions(string key, string value)
  {
    lock (additionalInstructions)
    {
      additionalInstructions[key] = value;
    }
  }

  public string GetInstructions(Pawn pawn) =>
    GetInstructions(pawn.ThingID);

  public string GetInstructions(string key)
  {
    lock (additionalInstructions)
    {
      if (additionalInstructions.TryGetValue(key, out string value))
        return value;
    }
    return string.Empty;
  }

  public List<Conversation> Conversations => conversations;
  public List<Conversation> GetConversationsByPawn(Pawn pawn)
  {
    if (pawn == null)
      return new List<Conversation>();
    lock (conversations)
    {
      return conversations.FindAll(convo => convo.InvolvesPawn(pawn));
    }
  }

  public List<Conversation> GetConversationsByColonist()
  {
    lock (conversations)
    {
      return conversations.FindAll(convo => convo.InvolvesColonist());
    }
  }

  private string? _version = null;

  public override void ExposeData()
  {
    base.ExposeData();
    _version = Scribe.mode is LoadSaveMode.Saving ? RimDialogue.Mod.Version : null;
    Scribe_Values.Look(ref _version, "Version");
    Scribe_Collections.Look(ref conversations, "conversations", LookMode.Deep);
    Scribe_Collections.Look(ref additionalInstructions, "additionalInstructions", LookMode.Value, LookMode.Value);
    Scribe_Values.Look(ref MessageWindowSize, "messageWindowSize", new Vector2(200f, 400f));
    Scribe_Values.Look(ref MessageWindowPosition, "messageWindowPosition", new Vector2(100f, 50f));
  }
}

public class Conversation : IExposable
{
  private Pawn? initiator;
  private Pawn? recipient;
  public string? text;
  public string? interaction;
  public int? timestamp;
  public Conversation() { }

  public Conversation(Pawn initiator, Pawn recipient, string? interaction, string text)
  {
    this.initiator = initiator;
    this.recipient = recipient;
    this.text = text;
    this.interaction = interaction;
    timestamp = Find.TickManager.TicksGame;
  }
  public Pawn? Initiator => initiator;
  public Pawn? Recipient => recipient;
  public string Participants => $"{Initiator?.Name?.ToStringShort ?? "Unknown"} ↔ {Recipient?.Name?.ToStringShort ?? "Unknown"}";
  public bool InvolvesPawn(Pawn pawn)
  {
    return pawn.thingIDNumber == initiator?.thingIDNumber || pawn.thingIDNumber == recipient?.thingIDNumber;
  }
  public bool InvolvesColonist()
  {
    return initiator != null && initiator.IsColonist || recipient != null && recipient.IsColonist;
  }
  public void ExposeData()
  {
    Scribe_References.Look(ref initiator, "initiator");
    Scribe_References.Look(ref recipient, "recipient");
    Scribe_Values.Look(ref text, "text");
    Scribe_Values.Look(ref interaction, "interaction");
    Scribe_Values.Look(ref timestamp, "timestamp");
  }
}
