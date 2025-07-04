#nullable enable

using RimDialogue.Access;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionDefs;
using RimDialogue.Core.InteractionRequests;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;
using Verse.Grammar;
using DialogueData = RimDialogue.Core.InteractionData.DialogueData;

namespace RimDialogue.Core.InteractionData
{

  public abstract class DialogueRequest
  {
    public static DateTime LastDialogue = DateTime.MinValue;
    public static Dictionary<int, DialogueRequest> Requests { get; set; } = [];

    public static DialogueRequest Create(PlayLogEntry_Interaction __instance, string interaction, InteractionDef interactionDef)
    {
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - DialogueRequest: '{interactionDef.defName}'");
      switch (interactionDef.defName)
      {
        case "RecentIncidentChitchat":
          dialogueRequest = DialogueRequestIncident<DialogueDataIncident>.BuildFrom(__instance);
          break;
        case "RecentBattleChitchat":
          dialogueRequest = DialogueRequestBattle_Recent.BuildFrom(__instance);
          break;
        case "GameConditionChitchat":
          dialogueRequest = DialogueRequestCondition.BuildFrom(__instance);
          break;
        case "MessageChitchat":
          dialogueRequest = DialogueRequestMessage.BuildFrom(__instance);
          break;
        case "AlertChitchat":
          dialogueRequest = DialogueRequestAlert<DialogueDataAlert>.BuildFrom(__instance);
          break;
        case "SameIdeologyChitchat":
          dialogueRequest = DialogueRequestIdeology<DialogueData>.BuildFrom(__instance);
          break;
        case "SkillChitchat":
          dialogueRequest = DialogueRequestSkill.BuildFrom(__instance);
          break;
        case "BestSkillChitchat":
          dialogueRequest = DialogueRequestBestSkill.BuildFrom(__instance);
          break;
        case "WorstSkillChitchat":
          dialogueRequest = DialogueRequestWorstSkill.BuildFrom(__instance);
          break;
        case "ColonistChitchat":
          dialogueRequest = DialogueRequestColonist<DialogueTargetData>.BuildFrom(__instance);
          break;
        case "ColonyAnimalChitchat":
          dialogueRequest = DialogueRequestAnimal_Colony.BuildFrom(__instance);
          break;
        case "WildAnimalChitchat":
          dialogueRequest = DialogueRequestAnimal_Wild.BuildFrom(__instance);
          break;
        case "InitiatorHealthChitchat":
          dialogueRequest = DialogueRequestHealthInitiator.BuildFrom(__instance);
          break;
        case "RecipientHealthChitchat":
          dialogueRequest = DialogueRequestHealthRecipient.BuildFrom(__instance);
          break;
        case "InitiatorApparelChitchat":
          dialogueRequest = DialogueRequestApparel_Initiator.BuildFrom(__instance);
          break;
        case "RecipientApparelChitchat":
        case "SlightApparel":
          dialogueRequest = DialogueRequestApparel_Recipient.BuildFrom(__instance);
          break;
        case "UnsatisfiedNeedChitchat":
          dialogueRequest = DialogueRequestNeed<DialogueDataNeed>.BuildFrom(__instance);
          break;
        case "InitiatorFamilyChitchat":
          dialogueRequest = DialogueRequestInitiatorFamily.BuildFrom(__instance);
          break;
        case "RecipientFamilyChitchat":
        case "SlightFamily":
          dialogueRequest = DialogueRequestRecipientFamily.BuildFrom(__instance);
          break;
        case "WeatherChitchat":
          dialogueRequest = DialogueRequestWeather.BuildFrom(__instance);
          break;
        case "RoomChitchat":
          dialogueRequest = DialogueRequestRoom.BuildFrom(__instance);
          break;
        case "InitiatorBedroomChitchat":
          dialogueRequest = DialogueRequestRoom_InitiatorBedroom.BuildFrom(__instance);
          break;
        case "RecipientBedroomChitchat":
          dialogueRequest = DialogueRequestRoom_RecipientBedroom.BuildFrom(__instance);
          break;
        case "HostileFactionChitchat":
          dialogueRequest = DialogueRequestHostileFaction.BuildFrom(__instance);
          break;
        case "AlliedFactionChitchat":
          dialogueRequest = DialogueRequestAlliedFaction.BuildFrom(__instance);
          break;
        case "RoyalFactionChitchat":
          dialogueRequest = DialogueRequestRoyalFaction.BuildFrom(__instance);
          break;
        case "NeutralFactionChitchat":
          dialogueRequest = DialogueRequestNeutralFaction.BuildFrom(__instance);
          break;
        case "InitiatorWeaponChitchat":
          dialogueRequest = DialogueRequestWeapon_Initiator.BuildFrom(__instance);
          break;
        case "RecipientWeaponChitchat":
        case "SlightWeapon":
          dialogueRequest = DialogueRequestWeapon_Recipient.BuildFrom(__instance);
          break;
        case "InitiatorBeardChitchat":
        case "InitiatorBodyTattooChitchat":
        case "InitiatorFaceTattooChitchat":
        case "InitiatorHairChitchat":
          dialogueRequest = DialogueRequestAppearance_Initiator.BuildFrom(__instance);
          break;
        case "RecipientBeardChitchat":
        case "RecipientBodyTattooChitchat":
        case "RecipientFaceTattooChitchat":
        case "RecipientHairChitchat":
        case "SlightBeard":
        case "SlightBodyTattoo":
        case "SlightFaceTattoo":
        case "SlightHair":
          dialogueRequest = DialogueRequestAppearance_Recipient.BuildFrom(__instance);
          break;
        case "DeadColonistDeepTalk":
          dialogueRequest = DialogueRequestDeadColonist.BuildFrom(__instance);
          break;
        case "InitiatorBattleChitchat":
          dialogueRequest = DialogueRequestBattle_Initiator.BuildFrom(__instance);
          break;
        case "RecipientBattleChitchat":
          dialogueRequest = DialogueRequestBattle_Recipient.BuildFrom(__instance);
          break;
        default:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Default interaction def '{interactionDef.defName}'.");
          dialogueRequest = new DialogueRequestTwoPawn<DialogueData>(__instance);
          break;
      }
      Requests.Add(__instance.LogID, dialogueRequest);

      return dialogueRequest;
    }

    public static DialogueRequest CreateThoughtRequest(PlayLogEntry_InteractionSinglePawn __instance, Thought_InteractionDef thoughtInteractionDef)
    {
      Pawn ? otherPawn = null;
      Precept? sourcePrecept = thoughtInteractionDef.thought.sourcePrecept;

      switch (thoughtInteractionDef.thought)
      {
        case Thought_Memory thoughtMemory:
          otherPawn = thoughtMemory.otherPawn; 
          break;
        case Thought_Situational thoughtSituational:
          break;
      }

      return new DialogueRequestThought(
        __instance,
        thoughtInteractionDef.thought.LabelCap,
        thoughtInteractionDef.thought.Description,
        thoughtInteractionDef.thought.MoodOffset(),
        sourcePrecept?.Label,
        sourcePrecept?.Description,
        otherPawn);
    }

    public static DialogueRequest Create(PlayLogEntry_InteractionSinglePawn __instance, InteractionDef interactionDef)
    {
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - DialogueRequest SinglePawn: '{interactionDef.defName}'");
      switch (interactionDef)
      {
        case BattleLogEntry_InteractionDef battleLogEntryInteractionDef:
          dialogueRequest = new DialogueRequestBattleLogEntry(
            __instance,
            battleLogEntryInteractionDef.CombatLogText,
            battleLogEntryInteractionDef.Target);
          break;
        case Thought_InteractionDef thoughtInteractionDef:
          dialogueRequest = CreateThoughtRequest(__instance, thoughtInteractionDef);
          break;
        default:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Default interaction def '{interactionDef.defName}'.");
          dialogueRequest = new DialogueRequestSinglePawn<DialogueData>(__instance);
          break;
      }
      Requests.Add(__instance.LogID, dialogueRequest);

      return dialogueRequest;
    }

    public static DialogueRequest Create(PlayLogEntry_InteractionWithMany __instance, InteractionDef interactionDef)
    {
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - DialogueRequest SinglePawn: '{interactionDef.defName}'");
      switch (interactionDef)
      {
        default:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Default interaction def '{interactionDef.defName}'.");
          dialogueRequest = new DialogueRequestMany<DialogueData>(__instance);
          break;
      }
      Requests.Add(__instance.LogID, dialogueRequest);

      return dialogueRequest;
    }


    public static bool TooSoon()
    {
      if (DateTime.Now.Subtract(LastDialogue) < TimeSpan.FromSeconds(Settings.MinTimeBetweenConversations.Value))
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Too soon for dialogue. Last dialogue was {LastDialogue}.");
        return true;
      }
      return false;
    }

    public static bool TooSoonAll()
    {
      if (Find.TickManager.TicksAbs - InteractionWorker_Dialogue.LastUsedTicksAll < Settings.MinDelayMinutesAll.Value * InteractionWorker_Dialogue.TicksPerMinute)
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Too soon for dialogue for all. Last dialogue was {InteractionWorker_Dialogue.LastUsedTicksAll} ticks ago.");
        return true;
      }
      return false;
    }

    public static async Task<DialogueResponse> Post(string action, WWWForm form, int entryId)
    {
      var serverUri = new Uri(Settings.ServerUrl.Value);
      var serverUrl = serverUri.GetLeftPart(UriPartial.Authority) + "/" + action;
      try
      {
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Server URL: '{serverUrl}'.");
        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
        {
          var asyncOperation = request.SendWebRequest();

          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Web request sent to {serverUrl}.");
          while (!asyncOperation.isDone)
          {
            await Task.Yield();
          }
          if (request.isHttpError || request.isNetworkError)
          {
            throw new Exception($"Entry {entryId} - Network error: {request.error}");
          }
          else
          {
            while (!request.downloadHandler.isDone) { await Task.Yield(); }
            var body = request.downloadHandler.text;
            // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Body: '{body}'.");
            return JsonUtility.FromJson<DialogueResponse>(body);
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Entry {entryId} - Error posting to '{serverUrl}'.", ex);
      }
    }

    public LogEntry Entry { get; set; }

    protected string _interaction = string.Empty;
    public string Interaction => _interaction;

    public DialogueRequest(LogEntry entry)
    {
      Entry = entry;
    }

    public abstract Rule[] Rules
    {
      get;
    }

    public abstract void Execute(string interaction);
  }

  public abstract class DialogueRequest<DataT> : DialogueRequest where DataT : DialogueData, new()
  {
    //public InteractionDef InteractionDef { get; set; }
    //public Pawn Initiator { get; set; }
    //public PawnData? initiatorData;
    
    public string clientId;
    public int maxWords;
    public int minWords;

    protected GameComponent_ConversationTracker _tracker = H.GetTracker();

    public DialogueRequest(LogEntry entry) : base(entry)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Dialogue Request started: {this.GetType().Name}");
      clientId = Settings.ClientId.Value;
      maxWords = Settings.MaxWords.Value;
      minWords = Settings.MinWords.Value;
    }

    public virtual void BuildData(DataT data)
    {
      data.Interaction = H.RemoveWhiteSpaceAndColor(Interaction);
      data.ClientId = clientId;
      data.Instructions = Instructions;
      data.MaxWords = maxWords;
      data.MinWords = minWords;
      data.LanguageEnglish = LanguageDatabase.activeLanguage?.FriendlyNameEnglish ?? LanguageDatabase.defaultLanguage?.FriendlyNameEnglish ?? "English";
      data.LanguageNative = LanguageDatabase.activeLanguage?.FriendlyNameNative ?? LanguageDatabase.defaultLanguage?.FriendlyNameNative ?? "English";
      data.ModelName = Settings.ModelName.Value;
    }

    public virtual void BuildForm(WWWForm form)
    {
      form.AddField("initiatorJson", JsonUtility.ToJson(InitiatorData));
    }

    public abstract Pawn Initiator
    {
      get;
    }

    public abstract PawnData InitiatorData
    {
      get;
    }

    public abstract InteractionDef InteractionDef
    {
      get;
    }
    public abstract string Instructions
    {
      get;
    }

    public virtual async void Send(
      List<KeyValuePair<string, object?>> datae,
      string? action = null)
    {
      try
      {
        WWWForm form = new WWWForm();
        BuildForm(form);

        foreach (var data in datae)
        {
          try
          {
            if (data.Value != null)
              form.AddField(data.Key, JsonUtility.ToJson(data.Value));
          }
          catch (Exception ex)
          {
            Mod.ErrorV($"Entry {Entry.LogID} - Error adding field '{data.Key}' to form: {ex}");
          }
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - Dialogue data fetched.");
        if (action == null)
          action = InteractionDef.defName;
        var interaction = Interaction;
        var dialogueResponse = await Post($"home/{action}", form, this.Entry.LogID);
        if (dialogueResponse != null)
        {
          if (dialogueResponse.rateLimited)
          {
            Mod.Log($"Entry {Entry.LogID} was rate limited: {dialogueResponse.rate} requests per second.");
            if (!Settings.ShowInteractionBubbles.Value)
              Bubbles_Bubbler_Add.AddBubble(Initiator, Entry, interaction);
            return;
          }
          if (InteractionDef.Worker is InteractionWorker_Dialogue)
            ((InteractionWorker_Dialogue)InteractionDef.Worker).SetLastUsedTicks();

          if (dialogueResponse.text != null)
            AddConversation(interaction, dialogueResponse.text);

          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - Conversation added.");
          if (dialogueResponse.text == null)
            throw new Exception("Response text is null.");
          if (Settings.ShowDialogueBubbles.Value)
            Bubbles_Bubbler_Add.AddBubble(Initiator, Entry, dialogueResponse.text);
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - GetChitChat Complete.");
        }
      }
      catch (Exception ex)
      {
        Mod.ErrorV($"Entry {Entry.LogID} - An error occurred in Send.\r\n{ex}");
      }
    }

    public abstract void AddConversation(string interaction, string text);
    

    public virtual string? Action
    {
      get
      {
        return "Dialogue";
      }
    }

    private static Regex CleanInteraction = new(@"\(\*Name\)(\w+)\(\/Name\)", RegexOptions.Compiled);
    public override void Execute(string interaction)
    {
      LastDialogue = DateTime.Now;
      var match = CleanInteraction.Match(interaction);
      if (match.Success && match.Groups.Count > 1)
        interaction = CleanInteraction.Replace(interaction, match.Groups[0].Value);
      _interaction = interaction;
      var dialogueData = new DataT();
      BuildData(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        Action);
    }
  }
}
