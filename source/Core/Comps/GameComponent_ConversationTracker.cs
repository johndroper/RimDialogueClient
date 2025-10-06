#nullable enable
using RimDialogue;
using RimDialogue.Context;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using RimDialogue.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimWorld.ColonistBar;
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

  public static HashSet<int> ExecutedLogEntries = new HashSet<int>();
  public Dictionary<int, string> InteractionCache = new Dictionary<int, string>();

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

  //public override void LoadedGame()
  //{
  //  base.LoadedGame();
  //}

  public override void StartedNewGame()
  {
    base.StartedNewGame();
    additionalInstructions[InstructionsSet.ALL_PAWNS] = "RimDialogue.DefaultInstructions".Translate();
    GetScenarioInstructions(Find.Scenario?.name + Environment.NewLine + H.RemoveWhiteSpace(Find.Scenario?.description));
    GetInstructions();
  }

  public override void GameComponentUpdate()
  {
    base.GameComponentUpdate();
    try
    {
      //Wait until game is fully loaded and window hasn't been shown yet
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
    catch (Exception ex)
    {
      Log.ErrorOnce($"Error updating ConversationTracker: {ex}", 87438564);
    }
  }

  private void GetInstructions()
  {
    List<PawnData> pawns = new List<PawnData>();
    var colonists = Find.CurrentMap.mapPawns.FreeColonists.ListFullCopy();
    foreach (var pawn in colonists)
    {
      if (!String.IsNullOrWhiteSpace(GetInstructions(pawn)))
        continue; // Already have instructions for this pawn
      pawns.Add(pawn.MakeData(null, -1));
    }
    GetInstructions(pawns);
  }

  private async void GetInstructions(IEnumerable<PawnData> pawns)
  {
    try
    {
      foreach (var pawnData in pawns)
      {
        await GetPawnInstructions(pawnData);
        await Task.Delay(5000);
      }
    }
    catch (Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching pawn Instructions: " + ex.ToString());
    }
  }

  public async void GetInstructionsAsync(PawnData pawnData)
  {
    await GetPawnInstructions(pawnData);
  }

  public async Task GetPawnInstructions(PawnData pawnData)
  {
    WWWForm form = new WWWForm();
    form.AddField("clientId", Settings.ClientId.Value);
    form.AddField("pawnJson", JsonUtility.ToJson(pawnData));
    form.AddField("modelName", Settings.ModelName.Value);
    var dialogueResponse = await DialogueRequest.Post("home/GetCharacterPrompt", form, -1);
    if (dialogueResponse.text != null)
      additionalInstructions[pawnData.ThingID] = dialogueResponse.text;
  }

  public async void GetScenarioInstructions(string scenarioText)
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
    try
    {
      if (initiator == null || text == null || string.IsNullOrWhiteSpace(text))
        return;
      Conversation conversation = new Conversation(initiator, recipient, interaction, text);
#if !RW_1_5
      if (GameComponent_ContextTracker.Instance != null)
      {
        var context = TemporalContextCatalog.Create(conversation);
        if (context == null)
          return;
        GameComponent_ContextTracker.Instance.Add(context);
      }
#endif
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
    catch (Exception ex)
    {
      Mod.Error("Error adding conversation: " + ex.ToString());
    }
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
      return conversations.FindAll(convo => convo.Involves(pawn));
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

    var watch = Stopwatch.StartNew();
    _version = Scribe.mode is LoadSaveMode.Saving ? RimDialogue.Mod.Version : null;
    Scribe_Values.Look(ref _version, "Version");
    Scribe_Collections.Look(ref conversations, "conversations_v2", LookMode.Deep);
    if (conversations == null)
      conversations = new List<Conversation>();
    Scribe_Collections.Look(ref additionalInstructions, "additionalInstructions", LookMode.Value, LookMode.Value);
    Scribe_Values.Look(ref MessageWindowSize, "messageWindowSize", new Vector2(200f, 400f));
    Scribe_Values.Look(ref MessageWindowPosition, "messageWindowPosition", new Vector2(100f, 50f));
    Scribe_Collections.Look(ref InteractionCache, "interactionCache", LookMode.Value, LookMode.Value);
    if (InteractionCache == null)
      InteractionCache = new Dictionary<int, string>();
    watch.Stop();
    if (Scribe.mode == LoadSaveMode.LoadingVars && Settings.VerboseLogging.Value)
      Mod.Log($"Loaded {conversations.Count} conversations into tracker in {watch.Elapsed.TotalSeconds} seconds.");
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


