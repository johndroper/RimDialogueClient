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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;
using Verse.Grammar;
using static RimWorld.ColonistBar;
using static System.Net.Mime.MediaTypeNames;

namespace RimDialogue.Core.InteractionData
{

  public abstract class DialogueRequest
  {
    public static DateTime LastDialogue = DateTime.MinValue;
    public static Dictionary<int, DialogueRequest> Requests { get; set; } = [];

    //public static DialogueRequest? Create(
    //  PlayLogEntry_Interaction __instance,
    //  Pawn initiator,
    //  Pawn recipient,
    //  string interaction,
    //  InteractionDef interactionDef)
    //{
    //  if (Requests.ContainsKey(__instance.LogID))
    //    return Requests[__instance.LogID];
    //  DialogueRequest? dialogueRequest;
    //  if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - DialogueRequest: '{interactionDef.defName}' - '{interaction}'");

    //  switch (interactionDef.defName)
    //  {
    //    case "RecentIncidentChitchat":
    //      dialogueRequest = new DialogueRequestIncident<DialogueDataIncident>(
    //        __instance,
    //        interactionDef,
    //        initiator,
    //        recipient);
    //      break;
    //    case "RecentBattleChitchat":
    //      dialogueRequest = new DialogueRequestBattle_Recent(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "GameConditionChitchat":
    //      dialogueRequest = new DialogueRequestCondition(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "MessageChitchat":
    //      dialogueRequest = new DialogueRequestMessage(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "AlertChitchat":
    //      dialogueRequest = new DialogueRequestAlert<DialogueDataAlert>(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "SameIdeologyChitchat":
    //      dialogueRequest = new DialogueRequestIdeology<DialogueData>(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "SkillChitchat":
    //      dialogueRequest = new DialogueRequestSkill(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "BestSkillChitchat":
    //      dialogueRequest = new DialogueRequestBestSkill(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "WorstSkillChitchat":
    //      dialogueRequest = new DialogueRequestWorstSkill(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "ColonistChitchat":
    //      dialogueRequest = new DialogueRequestColonist<DialogueTargetData>(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "ColonyAnimalChitchat":
    //      dialogueRequest = new DialogueRequestAnimal_Colony(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "WildAnimalChitchat":
    //      dialogueRequest = new DialogueRequestAnimal_Wild(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorHealthChitchat":
    //      dialogueRequest = new DialogueRequestHealthInitiator(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientHealthChitchat":
    //      dialogueRequest = new DialogueRequestHealthRecipient(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorApparelChitchat":
    //      dialogueRequest = new DialogueRequestApparel_Initiator(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientApparelChitchat":
    //    case "SlightApparel":
    //      dialogueRequest = new DialogueRequestApparel_Recipient(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "UnsatisfiedNeedChitchat":
    //      dialogueRequest = new DialogueRequestNeed<DialogueDataNeed>(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorFamilyChitchat":
    //      dialogueRequest = new DialogueRequestInitiatorFamily(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientFamilyChitchat":
    //    case "SlightFamily":
    //      dialogueRequest = new DialogueRequestRecipientFamily(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RoomChitchat":
    //      dialogueRequest = new DialogueRequestRoom(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorBedroomChitchat":
    //      dialogueRequest = new DialogueRequestRoom_InitiatorBedroom(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientBedroomChitchat":
    //      dialogueRequest = new DialogueRequestRoom_RecipientBedroom(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "HostileFactionChitchat":
    //      dialogueRequest = new DialogueRequestHostileFaction(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "AlliedFactionChitchat":
    //      dialogueRequest = new DialogueRequestAlliedFaction(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RoyalFactionChitchat":
    //      dialogueRequest = new DialogueRequestRoyalFaction(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "NeutralFactionChitchat":
    //      dialogueRequest = new DialogueRequestNeutralFaction(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorWeaponChitchat":
    //      dialogueRequest = new DialogueRequestWeapon_Initiator(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientWeaponChitchat":
    //    case "SlightWeapon":
    //      dialogueRequest = new DialogueRequestWeapon_Recipient(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorBeardChitchat":
    //    case "InitiatorBodyTattooChitchat":
    //    case "InitiatorFaceTattooChitchat":
    //    case "InitiatorHairChitchat":
    //      dialogueRequest = new DialogueRequestAppearance_Initiator(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientBeardChitchat":
    //    case "RecipientBodyTattooChitchat":
    //    case "RecipientFaceTattooChitchat":
    //    case "RecipientHairChitchat":
    //    case "SlightBeard":
    //    case "SlightBodyTattoo":
    //    case "SlightFaceTattoo":
    //    case "SlightHair":
    //      dialogueRequest = new DialogueRequestAppearance_Recipient(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "DeadColonistDeepTalk":
    //      dialogueRequest = new DialogueRequestDeadColonist(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "InitiatorBattleChitchat":
    //      dialogueRequest = new DialogueRequestBattle_Initiator(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "RecipientBattleChitchat":
    //      dialogueRequest = new DialogueRequestBattle_Recipient(__instance, interactionDef, initiator, recipient);
    //      break;
    //    case "WeatherChitchat":
    //      dialogueRequest = new DialogueRequestWeather(__instance, interactionDef, initiator, recipient);
    //      break;
    //    default:
    //      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Default interaction def '{interactionDef.defName}'.");
    //      if (recipient == null)
    //        dialogueRequest = new DialogueRequestSinglePawn<DialogueData>(
    //          __instance,
    //          interactionDef,
    //          initiator,
    //          false);
    //      else
    //        dialogueRequest = new DialogueRequestTwoPawn<DialogueData>(
    //          __instance,
    //          interactionDef,
    //          initiator,
    //          recipient,
    //          false);
    //      break;
    //  }
    //  if (dialogueRequest == null)
    //    return null;

    //  Requests.Add(__instance.LogID, dialogueRequest);
    //  return dialogueRequest;
    //}

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
        thoughtInteractionDef,
        thoughtInteractionDef.thought.LabelCap,
        thoughtInteractionDef.thought.Description,
        thoughtInteractionDef.thought.MoodOffset(),
        sourcePrecept?.Label,
        sourcePrecept?.Description,
        otherPawn);
    }

    public static DialogueRequest Create(
      PlayLogEntry_InteractionSinglePawn __instance,
      InteractionDef interactionDef)
    {
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - DialogueRequest InteractionSinglePawn: '{interactionDef.defName}'");
      switch (interactionDef)
      {
        case BattleLogEntry_InteractionDef battleLogEntryInteractionDef:
          dialogueRequest = new DialogueRequestBattleLogEntry(
            __instance,
            interactionDef,
            battleLogEntryInteractionDef.CombatLogText,
            battleLogEntryInteractionDef.Target);
          break;
        case Thought_InteractionDef thoughtInteractionDef:
          dialogueRequest = CreateThoughtRequest(__instance, thoughtInteractionDef);
          break;
        default:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Default interaction def '{interactionDef.defName}'.");
          dialogueRequest = new DialogueRequestSinglePawn<DialogueData>(
            __instance,
            interactionDef);
          break;
      }
      Requests.Add(__instance.LogID, dialogueRequest);

      return dialogueRequest;
    }

    public static DialogueRequest Create(
      PlayLogEntry_InteractionWithMany __instance,
      string result,
      InteractionDef interactionDef)
    {
      if (Requests.ContainsKey(__instance.LogID))
        return Requests[__instance.LogID];
      DialogueRequest dialogueRequest;
      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - DialogueRequest InteractionWithMany: '{interactionDef.defName}'");
      switch (interactionDef)
      {
        default:
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {__instance.LogID} - Default interaction def '{interactionDef.defName}'.");
          dialogueRequest = new DialogueRequestMany<DialogueData>(
            __instance,
            interactionDef);
          break;
      }
      Requests.Add(__instance.LogID, dialogueRequest);

      return dialogueRequest;
    }


    public static bool TooSoon()
    {
      var timeSinceLastDialogue = DateTime.Now.Subtract(LastDialogue);
      if (timeSinceLastDialogue < TimeSpan.FromSeconds(Settings.MinTimeBetweenConversations.Value))
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Too soon for dialogue. Last dialogue was {timeSinceLastDialogue} ago. Min seconds between conversations: {Settings.MinTimeBetweenConversations.Value}.");
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
        //if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Server URL: '{serverUrl}'.");
        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
        {
          var asyncOperation = request.SendWebRequest();

          if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Web request sent to {serverUrl}.");
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
            if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Web request returned body length '{body.Length}'.");
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

    public abstract IDictionary<string, string> Constants
    {
      get;
    }

    public abstract bool KnownType { get; }

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

    public WeatherDef? Weather = Find.CurrentMap.weatherManager?.CurWeatherPerceived;
    public BiomeDef? Biome = Find.CurrentMap.Biome;
    public Season Season = GenLocalDate.Season(Find.CurrentMap);
    public float OutdoorTemp = Find.CurrentMap.mapTemperature.OutdoorTemp;

    protected GameComponent_ConversationTracker _tracker = H.GetTracker();

    public DialogueRequest(LogEntry entry) : base(entry)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Dialogue Request started: {this.GetType().Name}");
      clientId = Settings.ClientId.Value;
      maxWords = Settings.MaxWords.Value;
      minWords = Settings.MinWords.Value;
    }

    //this isn't async but some overrides are
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public virtual async Task BuildData(DataT data)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
      data.Interaction = H.RemoveWhiteSpaceAndColor(Interaction);
      data.ClientId = clientId;
      data.Instructions = Instructions;
      data.MaxWords = maxWords;
      data.MinWords = minWords;
      data.LanguageEnglish = LanguageDatabase.activeLanguage?.FriendlyNameEnglish ?? LanguageDatabase.defaultLanguage?.FriendlyNameEnglish ?? "English";
      data.LanguageNative = LanguageDatabase.activeLanguage?.FriendlyNameNative ?? LanguageDatabase.defaultLanguage?.FriendlyNameNative ?? "English";
      data.ModelName = Settings.ModelName.Value;
            
      if (Weather != null)
      {
        data.WeatherLabel = Weather.label;
        data.WeatherDescription = Weather.description;
      }
      if (Biome != null)
      {
        data.BiomeLabel = Biome.label;
        data.BiomeDescription = Biome.description;
      }
      data.Season = this.Season.ToString();
      data.OutdoorTemp = this.OutdoorTemp;
      data.HourOfDay = GenDate.HourOfDay(Find.TickManager.TicksAbs, Find.CurrentMap.Tile);
      data.TotalColonyTime = Find.TickManager.TicksAbs.ToStringTicksToPeriod();
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

    public virtual async Task Send(
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
        if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - Dialogue data fetched.");
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
          {
            AddConversation(interaction, dialogueResponse.text);
            if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - Conversation added.");
          }
          if (dialogueResponse.text == null)
            throw new Exception("Response text is null.");
          if (Settings.ShowDialogueBubbles.Value)
            Bubbles_Bubbler_Add.AddBubble(Initiator, Entry, dialogueResponse.text);
          if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - Complete.");
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
    public async override void Execute(string interaction)
    {
      try
      {
        if (GameComponent_ConversationTracker.ExecutedLogEntries.Contains(this.Entry.LogID) ||
          TooSoon() ||
          TooSoonAll() ||
          Settings.IsFiltered(this.Interaction) ||
          (Settings.OnlyColonists.Value && !this.Initiator.IsColonist))
          return;
        if (Settings.VerboseLogging.Value) Mod.Log($"Entry {Entry.LogID} - Execute DialogueRequest: '{InteractionDef.defName}' - '{interaction}'");
        LastDialogue = DateTime.Now;
        var match = CleanInteraction.Match(interaction);
        if (match.Success && match.Groups.Count > 1)
          interaction = CleanInteraction.Replace(interaction, match.Groups[0].Value);
        _interaction = interaction;
        var dialogueData = new DataT();
        await BuildData(dialogueData);
        await Send(
          [
            new("chitChatJson", dialogueData)
          ],
          Action);
        GameComponent_ConversationTracker.Instance.InteractionCache[Entry.LogID] = interaction;
        GameComponent_ConversationTracker.ExecutedLogEntries.Add(Entry.LogID);
      }
      catch (Exception ex)
      {
        Mod.ErrorV($"Entry {Entry.LogID} - An error occurred in Execute.\r\n{ex}");
      }
    }
  }
}
