#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionRequests;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;

namespace RimDialogue.Core.InteractionData
{

  public abstract class DialogueRequest
  {
    public static DateTime LastDialogue = DateTime.MinValue;
    static Dictionary<int, DialogueRequest> Requests { get; set; } = [];

    public static DialogueRequest Create(ref PlayLogEntry_Interaction __instance, ref string interactionTemplate, InteractionDef interactionDef)
    {
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      if (Settings.VerboseLogging.Value) Mod.Log($"DialogueRequest: {interactionDef.defName}");
      switch (interactionDef.defName)
      {
        case "RecentIncidentChitchat":
          dialogueRequest = DialogueRequestIncident<DialogueDataIncident>.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecentBattleChitchat":
          dialogueRequest = DialogueRequestBattle_Recent.BuildFrom(__instance, interactionTemplate);
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
        case "SameIdeologyChitchat":
          dialogueRequest = DialogueRequestIdeology<DialogueData>.BuildFrom(__instance, interactionTemplate);
          break;
        case "SkillChitchat":
          dialogueRequest = DialogueRequestSkill.BuildFrom(__instance, interactionTemplate);
          break;
        case "BestSkillChitchat":
          dialogueRequest = DialogueRequestBestSkill.BuildFrom(__instance, interactionTemplate);
          break;
        case "WorstSkillChitchat":
          dialogueRequest = DialogueRequestWorstSkill.BuildFrom(__instance, interactionTemplate);
          break;
        case "ColonistChitchat":
          dialogueRequest = DialogueRequestColonist<DialogueTargetData>.BuildFrom(__instance, interactionTemplate);
          break;
        case "ColonyAnimalChitchat":
          dialogueRequest = DialogueRequestAnimal_Colony.BuildFrom(__instance, interactionTemplate);
          break;
        case "WildAnimalChitchat":
          dialogueRequest = DialogueRequestAnimal_Wild.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorHealthChitchat":
          dialogueRequest = DialogueRequestHealthInitiator.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientHealthChitchat":
          dialogueRequest = DialogueRequestHealthRecipient.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorApparelChitchat":
          dialogueRequest = DialogueRequestApparel_Initiator.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientApparelChitchat":
        case "SlightApparel":
          dialogueRequest = DialogueRequestApparel_Recipient.BuildFrom(__instance, interactionTemplate);
          break;
        case "UnsatisfiedNeedChitchat":
          dialogueRequest = DialogueRequestNeed<DialogueDataNeed>.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorFamilyChitchat":
          dialogueRequest = DialogueRequestInitiatorFamily.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientFamilyChitchat":
        case "SlightFamily":
          dialogueRequest = DialogueRequestRecipientFamily.BuildFrom(__instance, interactionTemplate);
          break;
        case "WeatherChitchat":
          dialogueRequest = DialogueRequestWeather.BuildFrom(__instance, interactionTemplate);
          break;
        case "RoomChitchat":
          dialogueRequest = DialogueRequestRoom.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorBedroomChitchat":
          dialogueRequest = DialogueRequestRoom_InitiatorBedroom.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientBedroomChitchat":
          dialogueRequest = DialogueRequestRoom_RecipientBedroom.BuildFrom(__instance, interactionTemplate);
          break;
        case "HostileFactionChitchat":
          dialogueRequest = DialogueRequestHostileFaction.BuildFrom(__instance, interactionTemplate);
          break;
        case "AlliedFactionChitchat":
          dialogueRequest = DialogueRequestAlliedFaction.BuildFrom(__instance, interactionTemplate);
          break;
        case "RoyalFactionChitchat":
          dialogueRequest = DialogueRequestRoyalFaction.BuildFrom(__instance, interactionTemplate);
          break;
        case "NeutralFactionChitchat":
          dialogueRequest = DialogueRequestNeutralFaction.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorWeaponChitchat":
          dialogueRequest = DialogueRequestWeapon_Initiator.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientWeaponChitchat":
        case "SlightWeapon":
          dialogueRequest = DialogueRequestWeapon_Recipient.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorBeardChitchat":
        case "InitiatorBodyTattooChitchat":
        case "InitiatorFaceTattooChitchat":
        case "InitiatorHairChitchat":
          dialogueRequest = DialogueRequestAppearance_Initiator.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientBeardChitchat":
        case "RecipientBodyTattooChitchat":
        case "RecipientFaceTattooChitchat":
        case "RecipientHairChitchat":
        case "SlightBeard":
        case "SlightBodyTattoo":
        case "SlightFaceTattoo":
        case "SlightHair":
          dialogueRequest = DialogueRequestAppearance_Recipient.BuildFrom(__instance, interactionTemplate);
          break;
        case "DeadColonistDeepTalk":
          dialogueRequest = DialogueRequestDeadColonist.BuildFrom(__instance, interactionTemplate);
          break;
        case "InitiatorBattleChitchat":
          dialogueRequest = DialogueRequestBattle_Initiator.BuildFrom(__instance, interactionTemplate);
          break;
        case "RecipientBattleChitchat":
          dialogueRequest = DialogueRequestBattle_Recipient.BuildFrom(__instance, interactionTemplate);
          break;
        default:
          if (Settings.VerboseLogging.Value) Mod.Log($"Default interaction def {interactionDef.defName} for log entry {__instance.LogID}.");
          dialogueRequest = new DialogueRequest<DialogueData>(__instance, interactionTemplate);
          break;
      }
      if (DateTime.Now.Subtract(LastDialogue) < TimeSpan.FromSeconds(Settings.MinTimeBetweenConversations.Value))
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Too soon since last dialogue. Current time: '{DateTime.Now}' Last dialogue time: {LastDialogue}.");
        return dialogueRequest;
      }
      LastDialogue = DateTime.Now;
      int ticksAbs = Find.TickManager.TicksAbs;
      if (ticksAbs - InteractionWorker_Dialogue.LastUsedTicksAll < Settings.MinDelayMinutesAll.Value * InteractionWorker_Dialogue.TicksPerMinute)
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Too soon since last dialogue. Current ticks: '{ticksAbs}' Last used ticks: {InteractionWorker_Dialogue.LastUsedTicksAll}.");
        return dialogueRequest;
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
        if (Settings.VerboseLogging.Value) Mod.Log($"Server URL: {serverUrl}");
        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
        {
          var asyncOperation = request.SendWebRequest();

          if (Settings.VerboseLogging.Value) Mod.Log($"Web request sent to {serverUrl}.");
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
            if (Settings.VerboseLogging.Value) Mod.Log($"Body returned:{body}.");
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
    public int minWords;
    public int initiatorOpinionOfRecipient;
    public int recipientOpinionOfInitiator;

    public DialogueRequest(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Dialogue Request started: {this.GetType().Name}");
      InteractionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(entry);
      Pawn? initiator, recipient;
      InteractionDef interactionDef;
      switch (entry)
      {
        case PlayLogEntry_Interaction interaction:
          if (Settings.VerboseLogging.Value) Mod.Log($"Interaction {entry.LogID} is 'PlayLogEntry_Interaction'");
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(interaction);
          recipient = (Pawn?)Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(interaction);
          interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(interaction);
          break;
        case PlayLogEntry_InteractionSinglePawn interaction:
          if (Settings.VerboseLogging.Value) Mod.Log($"Interaction {entry.LogID} is 'PlayLogEntry_InteractionSinglePawn'");
          initiator = (Pawn?)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(interaction);
          recipient = null;
          interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_InteractionDef.GetValue(interaction);
          break;
        default:
          throw new Exception("Unknown interaction type");
      }
      if (Settings.VerboseLogging.Value) Mod.Log($"Pawns fetched.");
      if (initiator is null || initiator.Map != Find.CurrentMap)
        throw new Exception("Initiator is null or not on the current map");
      if (recipient is null || recipient.Map != Find.CurrentMap)
        throw new Exception("Recipient is not null or not on the current map");
      this.Initiator = initiator;
      this.Recipient = recipient;
      if (Settings.VerboseLogging.Value) Mod.Log($"Building ChitChatData");
      clientId = Settings.ClientId.Value;
      var tracker = H.GetTracker();
      instructions = tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (initiator.IsColonist || recipient.IsColonist)
        instructions += "\r\n" + tracker.GetInstructions(InstructionsSet.COLONISTS);
      maxWords = Settings.MaxWords.Value;
      minWords = Settings.MinWords.Value;
      initiatorOpinionOfRecipient = initiator.relations.OpinionOf(recipient);
      recipientOpinionOfInitiator = recipient.relations.OpinionOf(initiator);
      if (Settings.VerboseLogging.Value) Mod.Log($"ChitChatData built.");
      initiatorData = H.MakePawnData(Initiator, tracker.GetInstructions(Initiator));
      recipientData = H.MakePawnData(Recipient, tracker.GetInstructions(Recipient));
    }

    public virtual void Build(DataT data)
    {
      data.Interaction = H.RemoveWhiteSpaceAndColor(GetInteraction());
      data.ClientId = clientId;
      data.Instructions = instructions;
      data.MaxWords = maxWords;
      data.MinWords = minWords;
      data.InitiatorOpinionOfRecipient = initiatorOpinionOfRecipient;
      data.RecipientOpinionOfInitiator = recipientOpinionOfInitiator;
    }

    public virtual async void Send(
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
        if (Settings.VerboseLogging.Value) Mod.Log($"DialogueData fetched for log entry {Entry.LogID}.");
        if (action == null)
          action = InteractionDef.defName;
        var interaction = GetInteraction();
        var dialogueResponse = await Post($"home/{action}", form);
        if (dialogueResponse != null)
        {
          if (dialogueResponse.rateLimited)
          {
            Mod.Log($"Log entry {Entry.LogID} was rate limited: {dialogueResponse.rate} requests per second.");
            if (!Settings.ShowInteractionBubbles.Value)
              Bubbles_Bubbler_Add.AddBubble(Initiator, Entry, interaction);
            //if (Settings.ShowDialogueMessages.Value && GameComponent_ConversationTracker.Instance.DialogueMessageWindow != null)
            //  GameComponent_ConversationTracker.Instance.DialogueMessageWindow.AddMessage(
            //    interaction,
            //    new LookTargets(Initiator));
            return;
          }
          if (InteractionDef.Worker is InteractionWorker_Dialogue)
            ((InteractionWorker_Dialogue)InteractionDef.Worker).SetLastUsedTicks();
          var tracker = H.GetTracker();
          tracker.AddConversation(Initiator, Recipient, interaction, dialogueResponse.text);
          if (Settings.VerboseLogging.Value) Mod.Log($"Conversation added for log entry {Entry.LogID}.");
          if (dialogueResponse.text == null)
            throw new Exception("Response text is null.");
          if (Settings.ShowDialogueBubbles.Value)
            Bubbles_Bubbler_Add.AddBubble(Initiator, Entry, dialogueResponse.text);
          //if (Settings.ShowDialogueMessages.Value && GameComponent_ConversationTracker.Instance.DialogueMessageWindow != null)
          //  GameComponent_ConversationTracker.Instance.DialogueMessageWindow.AddMessage(
          //    conversation,
          //    new LookTargets(Initiator));
          if (Settings.VerboseLogging.Value) Mod.Log($"GetChitChat Complete for log entry {Entry.LogID}.");
        }
      }
      catch (Exception ex)
      {
        Mod.ErrorV($"An error occurred in Send for log entry {Entry.LogID}.\r\n{ex}");
      }
    }

    public virtual string? Action
    {
      get
      {
        return "Dialogue";
      }
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        Action);
    }
  }
}
