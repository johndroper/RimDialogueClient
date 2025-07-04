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
    var pawnData = pawn.MakeData(null, -1);
    WWWForm form = new WWWForm();
    form.AddField("clientId", Settings.ClientId.Value);
    form.AddField("pawnJson", JsonUtility.ToJson(pawnData));
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
      var dialogueResponse = await DialogueRequest.Post("home/GetScenarioPrompt", form, -1);
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


public class Conversation : IExposable, IEquatable<Conversation>
{
  private Pawn initiator;
  private Pawn? recipient;
  public string? text;
  public string? interaction;
  public int? timestamp;
#pragma warning disable CS8618 
  public Conversation() { }
#pragma warning restore CS8618 

  public Conversation(Pawn initiator, Pawn? recipient, string? interaction, string text)
  {
    this.initiator = initiator;
    this.recipient = recipient;
    this.text = text;
    this.interaction = interaction;
    timestamp = Find.TickManager.TicksGame;
  }
  public Pawn Initiator => initiator;
  public Pawn? Recipient => recipient;
  public string Participants => $"{Initiator.Name?.ToStringShort ?? "Unknown"}" + Recipient != null ? $" ↔ {Recipient?.Name?.ToStringShort ?? "Unknown"}" : "";
  public bool InvolvesPawn(Pawn pawn)
  {
    return pawn.thingIDNumber == initiator.thingIDNumber || pawn.thingIDNumber == recipient?.thingIDNumber;
  }
  public bool InvolvesColonist()
  {
    return initiator != null && initiator.IsColonist || recipient != null && recipient.IsColonist;
  }

  public override string ToString()
  {
    return $"{initiator.Name?.ToStringShort ?? "Unknown"}" + (recipient != null ? $" ↔ {recipient.Name?.ToStringShort ?? "Unknown"}" : "") + $" ({interaction ?? "No Interaction"}): {text}";
  }

  override public int GetHashCode()
  {
    return this.text?.GetHashCode() ?? base.GetHashCode();
  }

  public override bool Equals(object obj)
  {
    return this.text?.Equals((obj as Conversation)?.text) ?? base.Equals(obj);
  }

  public static string[] BreakWord(string text, int maxChars)
  {
    if (string.IsNullOrEmpty(text) || maxChars <= 0)
      return new string[] { text };
    List<string> words = new List<string>();
    for (int i = 0; i < text.Length; i += maxChars)
    {
      int length = Math.Min(maxChars, text.Length - i);
      words.Add(text.Substring(i, length));
    }
    return words.ToArray();
  }

  public static IEnumerable<string> BreakWords(IEnumerable<string> words, int maxCharsPerWord)
  {
    foreach (var line in words)
    {
      if (line.Length > maxCharsPerWord)
      {
        foreach (var wrappedLine in BreakWord(line, maxCharsPerWord))
        {
          yield return wrappedLine;
        }
      }
      else
      {
        yield return line;
      }
    }
  }

  private static string[] SplitLine(string line, int maxWordsPerLine)
  {
    var words = line.Split([' ', '\n', '\u200B'], StringSplitOptions.RemoveEmptyEntries);
    words = BreakWords(words, 10).ToArray();

    int wordsPerLine = (int)Math.Floor(Math.Max(1, (maxWordsPerLine * 4) / words.Average(word => word.Length)));
    if (wordsPerLine > maxWordsPerLine)
      wordsPerLine = maxWordsPerLine;
    int lineCount = (int)Math.Ceiling((float)words.Length / (float)wordsPerLine);
    List<string> fragments = [];
    for (int i = 0; i < lineCount; i++)
    {
      var fragment = string.Join(" ", words.Skip(i * wordsPerLine).Take(wordsPerLine));
      if (i > 0)
        fragment = "..." + fragment;
      if (i < lineCount - 1)
        fragment += "...";
      fragments.Add(fragment);
    }
    return fragments.ToArray();
  }


  static Regex regex = new Regex(@"^(?<name>\w+):\s*[""“*](?<line>.+)[""”*]\W*$", RegexOptions.Multiline);
  public static Line[] ParseLines(string text)
  {
    if (string.IsNullOrEmpty(text))
      return [];
    var lines = new List<Line>();
    var matches = regex.Matches(text);
    if (matches.Count == 0)
    {
      var soloLines = text
        .Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
      foreach(var soloLine in soloLines)
      {
        if (soloLine.Length > 300)
        {
          var splitLines = SplitLine(soloLine, 40);
          
          for(int i = 0; i < splitLines.Length; i++)
          {
            var splitLine = splitLines[i];
            lines.Add(new Line("unknown", splitLine));
          }
        }
        else
          lines.Add(new Line("unknown", soloLine));
      }
      return lines.ToArray();
    }
    
    foreach (Match match in matches)
    {
      if (match.Success)
      {
        var name = match.Groups["name"].Value;
        var line = match.Groups["line"].Value.Trim();
        if (line.Length > 150)
        {
          var splitLines = SplitLine(line, 20);
          foreach (var splitLine in splitLines)
          {
            lines.Add(new Line(name, splitLine));
          }
        }
        else
          lines.Add(new Line(name, line));
      }
    }
    return lines.ToArray();
  }

  Line[]? lines;
  public Line[] Lines
  {
    get
    {
      if (text == null)
        return [];
      lines ??= ParseLines(text);
      return lines;
    }
  }

  public void ExposeData()
  {
    Scribe_References.Look(ref initiator, "initiator");
    Scribe_References.Look(ref recipient, "recipient");
    Scribe_Values.Look(ref text, "text");
    Scribe_Values.Look(ref interaction, "interaction");
    Scribe_Values.Look(ref timestamp, "timestamp");
  }

  public bool Equals(Conversation other)
  {
    if (other == null) return false;
    return this.text?.Equals(other.text) ?? false;
  }
}
