using RimDialogue;
using RimDialogue.Access;
using System;
using System.Collections.Generic;
using Verse;
using static System.Net.Mime.MediaTypeNames;
public class GameComponent_ConversationTracker : GameComponent
{
  private Dictionary<string, string> additionalInstructions;
  private List<Conversation> conversations;

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
  }

  public override void StartedNewGame()
  {
    base.StartedNewGame();
    additionalInstructions["ALL_PAWNS"] = Find.Scenario?.name + "\r\n" + Bubbles_Bubbler_Add.RemoveWhiteSpace(Find.Scenario?.description);
  }

  public void AddConversation(Pawn initiator, Pawn recipient, string text)
  {
    if (initiator == null || recipient == null || string.IsNullOrWhiteSpace(text))
      return;

    lock(conversations)
    {
      while (conversations.Count > Settings.MaxConversationsStored.Value)
        conversations.RemoveAt(0);
      conversations.Add(new Conversation(initiator, recipient, text));
    }
  }

  public void AddAdditionalInstructions(Pawn pawn, string value)
  {
    lock (additionalInstructions)
    {
      additionalInstructions[pawn?.ThingID ?? "ALL_PAWNS"] = value;
    }
  }

  public string GetAdditionalInstructions(Pawn pawn)
  {
    var thingId = pawn?.ThingID ?? "ALL_PAWNS";
    lock (additionalInstructions)
    {
      if (additionalInstructions.TryGetValue(thingId, out string value))
        return value;
    }
    return string.Empty;
  }

  public List<Conversation> Conversations => conversations;
  public List<Conversation> GetConversationsByPawn(Pawn pawn)
  {
    if (pawn == null)
      return new List<Conversation>();
    lock(conversations)
    {
      return conversations.FindAll(convo => convo.InvolvesPawn(pawn));
    }
  }

  private string _version = null;

  public override void ExposeData()
  {
    base.ExposeData();
    _version = Scribe.mode is LoadSaveMode.Saving ? RimDialogue.Mod.Version : null;
    Scribe_Values.Look(ref _version, "Version");
    Scribe_Collections.Look(ref conversations, "conversations", LookMode.Deep);
    Scribe_Collections.Look(ref additionalInstructions, "additionalInstructions", LookMode.Value, LookMode.Value);
    if (Scribe.mode is LoadSaveMode.LoadingVars && String.IsNullOrEmpty(_version))
    {
      // Migrate old version
      additionalInstructions["ALL_PAWNS"] =
        Find.Scenario?.name +
        "\r\n" +
        Bubbles_Bubbler_Add.RemoveWhiteSpace(Find.Scenario?.description)
        + "\r\n" +
        (additionalInstructions.ContainsKey("ALL_PAWNS") ? additionalInstructions["ALL_PAWNS"] : null);
      RimDialogue.Mod.Log("Scenario prepended to instructions.");
    }
  }
}

public class Conversation : IExposable
{
  private Pawn initiator;
  private Pawn recipient;
  public string text;

  public Conversation() { }

  public Conversation(Pawn initiator, Pawn recipient, string text)
  {
    this.initiator = initiator;
    this.recipient = recipient;
    this.text = text;
  }
  public Pawn Initiator => initiator;
  public Pawn Recipient => recipient;
  public string Participants => $"{Initiator?.Name?.ToStringShort ?? "Unknown"} ↔ {Recipient?.Name?.ToStringShort ?? "Unknown"}";
  public bool InvolvesPawn(Pawn pawn)
  {
    return pawn.thingIDNumber == initiator.thingIDNumber || pawn.thingIDNumber == recipient.thingIDNumber;
  }
  public void ExposeData()
  {
    Scribe_References.Look(ref initiator, "initiator");
    Scribe_References.Look(ref recipient, "recipient");
    Scribe_Values.Look(ref text, "text");
  }
}
