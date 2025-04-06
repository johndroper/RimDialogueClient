#nullable enable
using RimDialogue;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;

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
    additionalInstructions[InstructionsSet.ALL_PAWNS] = "RimDialogue.DefaultInstructions".Translate();
    GetScenarioInstructions(Find.Scenario?.name + "\r\n" + H.RemoveWhiteSpace(Find.Scenario?.description));
    foreach (var pawn in Find.CurrentMap.mapPawns.FreeColonists)
    {
      GetPawnInstructions(pawn);
    }
  }

  private async void GetPawnInstructions(Pawn pawn)
  {
    try
    {
      var pawnData = H.MakePawnData(pawn, null);
      WWWForm form = new WWWForm();
      form.AddField("clientId", Settings.ClientId.Value);
      form.AddField("pawnJson", JsonUtility.ToJson(pawnData));
      var dialogueResponse = await DialogueRequest.Post("home/GetCharacterPrompt", form);
      if (dialogueResponse.text != null)
        additionalInstructions[pawn.ThingID] = dialogueResponse.text;
    }
    catch (Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching pawn Instructions: " + ex.ToString());
    }
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
    catch(Exception ex)
    {
      RimDialogue.Mod.Error("Error fetching ScenarioInstructions: " + ex.ToString());
    }
  }

  public void AddConversation(Pawn initiator, Pawn? recipient, string? text)
  {
    if (initiator == null || recipient == null || text == null || string.IsNullOrWhiteSpace(text))
      return;
    lock (conversations)
    {
      while (conversations.Count > Settings.MaxConversationsStored.Value)
        conversations.RemoveAt(0);
      conversations.Add(new Conversation(initiator, recipient, text));
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
    if (Scribe.mode is LoadSaveMode.LoadingVars && String.IsNullOrEmpty(_version))
    {
      // Migrate old version
      additionalInstructions["ALL_PAWNS"] =
        Find.Scenario?.name +
        "\r\n" +
        H.RemoveWhiteSpace(Find.Scenario?.description)
        + "\r\n" +
        (additionalInstructions.ContainsKey("ALL_PAWNS") ? additionalInstructions["ALL_PAWNS"] : null);
      RimDialogue.Mod.Log("Scenario prepended to instructions.");
    }
  }
}

public class Conversation : IExposable
{
  private Pawn? initiator;
  private Pawn? recipient;
  public string? text;
  public int? timestamp;
  public Conversation() { }

  public Conversation(Pawn initiator, Pawn recipient, string text)
  {
    this.initiator = initiator;
    this.recipient = recipient;
    this.text = text;
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
    Scribe_Values.Look(ref timestamp, "timestamp");
  }
}
