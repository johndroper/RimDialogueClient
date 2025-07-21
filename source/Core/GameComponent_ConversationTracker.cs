#nullable enable
using RimDialogue;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using RimDialogue.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Mod = RimDialogue.Mod;

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
  public Vector2 MessageWindowSize = new Vector2(350f, Math.Max(UI.screenHeight - 400f, 200f));
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
        if (!String.IsNullOrWhiteSpace(GetInstructions(pawn)))
          continue; // Already have instructions for this pawn
        await GetPawnInstructions(pawn);
        await Task.Delay(5000);
      }
    }
    catch (Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching pawn Instructions: " + ex.ToString());
    }
  }

  private async Task GetPawnInstructions(Pawn pawn)
  {
    var pawnData = pawn.MakeData(null, -1);
    WWWForm form = new WWWForm();
    form.AddField("clientId", Settings.ClientId.Value);
    form.AddField("pawnJson", JsonUtility.ToJson(pawnData));
    form.AddField("modelName", Settings.ModelName.Value);
    var dialogueResponse = await DialogueRequest.Post("home/GetCharacterPrompt", form, -1);
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
      form.AddField("modelName", Settings.ModelName.Value);
      var dialogueResponse = await DialogueRequest.Post("home/GetScenarioPrompt", form, -1);
      if (dialogueResponse.text != null)
        additionalInstructions[InstructionsSet.COLONISTS] = dialogueResponse.text;
      await Task.Delay(5000);
    }
    catch (Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching ScenarioInstructions: " + ex.ToString());
    }
  }

  public void AddConversation(Pawn initiator, Pawn? recipient, string? interaction, string? text)
  {
    if (initiator == null || text == null || string.IsNullOrWhiteSpace(text))
      return;
    Conversation conversation = new Conversation(initiator, recipient, interaction, text);
    Conversation? removed = null;
    lock (conversations)
    {
      while (conversations.Count > Settings.MaxConversationsStored.Value)
      {
        removed = conversations[0];
        conversations.RemoveAt(0);
      }
      conversations.Add(conversation);
    }
    if (removed != null)
      ConversationRemoved?.Invoke(this, new ConversationArgs(removed));
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

public class Line
{
  public string Text;
  public string Name;
  public Line(string name, string text)
  {
    this.Name = name;
    this.Text = text;
  }
}


