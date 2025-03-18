#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;
using static System.Net.Mime.MediaTypeNames;

namespace RimDialogue.Core.InteractionData
{

  public abstract class DialogueRequest 
  {
    static Dictionary<int, DialogueRequest> Requests { get; set; } = [];

    public static DialogueRequest Create(ref PlayLogEntry_Interaction __instance, ref string interactionTemplate, InteractionDef interactionDef)
    {
      InteractionWorker_DialogueAlert.lastUsedTicks = Find.TickManager.TicksAbs;
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      switch (interactionDef.defName)
      {
        case "RecentIncidentChitchat":
          dialogueRequest = DialogueRequestIncident<DialogueDataIncident>.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecentBattleChitchat":
          dialogueRequest = DialogueRequestBattle<DialogueDataBattle>.BuildFrom(__instance, interactionTemplate);
          break;
        case "GameConditionChitchat":
          dialogueRequest = DialogueRequestCondition<DialogueDataCondition>.BuildFrom(__instance, interactionTemplate);
          break;
        case "MessageChitchat":
          dialogueRequest = DialogueRequestMessage.BuildFrom(__instance, interactionTemplate);
          break;
        case "AlertChitchat":
          dialogueRequest = DialogueRequestAlert<DialogueDataAlert>.BuildFrom(__instance, interactionTemplate);
          break;
        default:
          Mod.LogV($"Default interaction def {interactionDef.defName} for log entry {__instance.LogID}.");
          dialogueRequest = new DialogueRequest<DialogueData>(__instance, interactionTemplate);
          break;
      }
      dialogueRequest.Execute();
      return dialogueRequest;
    }

    public static async Task<DialogueResponse> Post(string action, WWWForm form)
    {
      var serverUri = new Uri(Settings.ServerUrl.Value);
      var serverUrl = serverUri.GetLeftPart(UriPartial.Authority) + "/" + action;
      try
      {
        Mod.LogV($"Server URL: {serverUrl}");
        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
        {
          var asyncOperation = request.SendWebRequest();

          Mod.LogV($"Web request sent to {serverUrl}.");
          while (!asyncOperation.isDone)
          {
            await Task.Yield();
          }
          if (request.isNetworkError || request.isHttpError)
          {
            throw new Exception($"Network error: {request.error}");
          }
          else
          {
            while (!request.downloadHandler.isDone) { await Task.Yield(); }
            var body = request.downloadHandler.text;
            Mod.LogV($"Body returned:{body}.");
            return JsonUtility.FromJson<DialogueResponse>(body);
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Error posting to '{serverUrl}'", ex);
      }
    }

    public LogEntry Entry { get; set; }
    public string InteractionTemplate { get; set; } = string.Empty;

    public DialogueRequest(LogEntry entry, string interactionTemplate)
    {
      Entry = entry;
      InteractionTemplate = interactionTemplate;
    }

    public virtual string GetInteraction()
    {
      return InteractionTemplate;
    }

    public abstract void Execute();
  }

  public class DialogueRequest<DataT> : DialogueRequest where DataT : DialogueData, new()
  {
    public InteractionDef InteractionDef { get; set; }
    public Pawn Initiator { get; set; }
    public Pawn Recipient { get; set; }

    public PawnData? initiatorData;
    public PawnData? recipientData;

    public string clientId;
    public string instructions;
    public int maxWords;
    public int initiatorOpinionOfRecipient;
    public int recipientOpinionOfInitiator;

    public DialogueRequest(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      InteractionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(entry);
      Pawn? initiator, recipient;
      InteractionDef interactionDef;
      switch (entry)
      {
        case PlayLogEntry_Interaction interaction:
          Mod.LogV($"Interaction {entry.LogID} is 'PlayLogEntry_Interaction'");
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interaction);
          recipient = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(interaction);
          interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(interaction);
          break;
        case PlayLogEntry_InteractionSinglePawn interaction:
          Mod.LogV($"Interaction {entry.LogID} is 'PlayLogEntry_InteractionSinglePawn'");
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(interaction);
          recipient = null;
          interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_InteractionDef.GetValue(interaction);
          break;
        default:
          throw new Exception("Unknown interaction type");
      }
      Mod.LogV($"Pawns fetched.");
      if (initiator is null || initiator.Map != Find.CurrentMap)
      {
        throw new Exception("Initiator is null or not on the current map");
      }
      if (recipient is null || recipient.Map != Find.CurrentMap)
      {
        throw new Exception("Recipient is not null or not on the current map");
      }
      this.Initiator = initiator;
      this.Recipient = recipient;
      Mod.LogV($"Building ChitChatData");
      clientId = Settings.ClientId.Value;
      var tracker = H.GetTracker();
      instructions = tracker.GetInstructions(null) + " " + Settings.SpecialInstructions;
      maxWords = Settings.MaxWords.Value;
      initiatorOpinionOfRecipient = initiator.relations.OpinionOf(recipient);
      recipientOpinionOfInitiator = recipient.relations.OpinionOf(initiator);
      Mod.LogV($"ChitChatData built.");

      initiatorData = H.MakePawnData(Initiator, tracker.GetInstructions(Initiator));
      recipientData = H.MakePawnData(Recipient, tracker.GetInstructions(Recipient));
    }

    public virtual void Build(DataT data)
    {
      data.Interaction = H.RemoveWhiteSpaceAndColor(GetInteraction());
      data.ClientId = clientId;
      data.Instructions = instructions;
      data.MaxWords = maxWords;
      data.InitiatorOpinionOfRecipient = initiatorOpinionOfRecipient;
      data.RecipientOpinionOfInitiator = recipientOpinionOfInitiator;
    }

    public async void Send(
      List<KeyValuePair<string, object?>> datae,
      string? action = null)
    {
      try
      {
        WWWForm form = new WWWForm();
        form.AddField("initiatorJson", JsonUtility.ToJson(initiatorData));
        form.AddField("recipientJson", JsonUtility.ToJson(recipientData));
        foreach (var data in datae)
        {
          if (data.Value != null)
            form.AddField(data.Key, JsonUtility.ToJson(data.Value));
        }
        Mod.LogV($"DialogueData fetched for log entry {Entry.LogID}.");
        if (action == null)
          action = InteractionDef.defName;
        var dialogueResponse = await Post($"home/{action}", form);
        if (dialogueResponse != null)
        {
          if (dialogueResponse.rateLimited)
          {
            Mod.Log("Log entry {entry.LogID} was rate limited.");
            return;
          }
          var tracker = H.GetTracker();
          tracker.AddConversation(Initiator, Recipient, dialogueResponse.text);
          Mod.LogV($"Conversation added for log entry {Entry.LogID}.");
          if (dialogueResponse.text == null)
            throw new Exception("Response text is null.");

          if (Settings.ShowDialogueBubbles.Value)
            Bubbles_Bubbler_Add.AddBubble(Initiator, Entry, dialogueResponse.text);

          if (Settings.ShowDialogueMessages.Value)
            DialogueMessages.AddMessage(
              dialogueResponse.text,
              new LookTargets(Initiator));

          Mod.LogV($"GetChitChat Complete for log entry {Entry.LogID}.");
        }
      }
      catch (Exception ex)
      {
        Mod.ErrorV($"An error occurred in Send for log entry {Entry.LogID}.\r\n{ex}");
      }
    }
    public override void Execute()
    {
      if (!InteractionWorker_Dialogue.IsEnabled)
        return;
      var dialogueData = new DataT();
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "Dialogue");
    }
  }
}
