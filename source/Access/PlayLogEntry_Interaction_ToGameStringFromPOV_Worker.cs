#nullable enable

using HarmonyLib;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;
using static System.Net.Mime.MediaTypeNames;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(PlayLogEntry_Interaction), "ToGameStringFromPOV_Worker")]
  public static class PlayLogEntry_Interaction_ToGameStringFromPOV_Worker
  {
    public static bool Prefix(
        PlayLogEntry_Interaction __instance,
        ref string __result,
        Pawn pov,
        bool forceLog,
        ref DialogueRequest? __state)
    {
      try
      {
        if (GameComponent_ConversationTracker.Instance != null && GameComponent_ConversationTracker.Instance.InteractionCache.TryGetValue(__instance.LogID, out var cached))
        {
          __result = cached;
          return false;
        }

        if (GameComponent_ConversationTracker.ExecutedLogEntries.Contains(__instance.LogID))
        {
          __result = string.Empty;
          return false;
        }

        var initiator = Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(__instance) as Pawn;
        var recipient = Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(__instance) as Pawn;
        var intDef = Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(__instance) as InteractionDef;

        if (Settings.VerboseLogging.Value)
          Mod.Log($"Entry {__instance.LogID} - {__instance.GetType().Name} {initiator} -> {recipient}");

        if (intDef == null)
          throw new Exception($"Entry {__instance.LogID} - PlayLogEntry_Interaction has a null InteractionDef reference.");

        if (initiator == null || recipient == null)
          throw new Exception($"Entry {__instance.LogID} - PlayLogEntry_Interaction has a null pawn reference.");

        GrammarRequest request = (GrammarRequest)Reflection.Verse_PlayLogEntry_Interaction_GenerateGrammarRequest.Invoke(__instance, null);

        try
        {
          __state = DialogueRequest.Create(
            __instance,
            initiator,
            recipient,
            __result,
            intDef);
          if (__state == null)
            throw new Exception("DialogueRequest.Create returned null.");
          request.Rules.AddRange(__state.Rules);
        }
        catch (Exception ex)
        {
          Mod.Error($"Entry {__instance.LogID} - An error occurred in PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.\r\n{ex}");
        }

        Rand.PushState();
        Rand.Seed = __instance.LogID;

        string text;
        if (pov == initiator)
        {
          request.IncludesBare.Add(intDef.logRulesInitiator);
          request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));

          request.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", recipient, request.Constants));
          text = GrammarResolver.Resolve("r_logentry", request, "interaction from initiator", forceLog);
        }
        else if (pov == recipient)
        {
          if (intDef.logRulesRecipient != null)
            request.IncludesBare.Add(intDef.logRulesRecipient);
          else
            request.IncludesBare.Add(intDef.logRulesInitiator);
          request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
          request.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", recipient, request.Constants));
          text = GrammarResolver.Resolve("r_logentry", request, "interaction from recipient", forceLog);
        }
        else
        {
          Log.ErrorOnce("Cannot display PlayLogEntry_Interaction from POV who isn't initiator or recipient.", 51251);
          text = __instance.ToString();
        }
        var extraSentencePacks = (List<RulePackDef>)Reflection.Verse_PlayLogEntry_Interaction_extraSentencePacks.GetValue(__instance);
        if (extraSentencePacks != null)
        {
          for (int i = 0; i < extraSentencePacks.Count; i++)
          {
            request.Clear();
            request.Includes.Add(extraSentencePacks[i]);
            request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
            request.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", recipient, request.Constants));
            text = text + " " + GrammarResolver.Resolve(extraSentencePacks[i].FirstRuleKeyword, request, "extraSentencePack", forceLog, extraSentencePacks[i].FirstUntranslatedRuleKeyword);
          }
        }
        __result = text;

        Rand.PopState();
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"Entry {__instance.LogID} - An error occurred in PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.\r\n{ex}", 3453439);
        return true;
      }
      return false;
    }

    public static void Postfix(PlayLogEntry_Interaction __instance, ref string __result, ref Pawn pov, ref bool forceLog, DialogueRequest? __state)
    {
      try
      {
        if (__state != null)
          __state.Execute(__result);
      }
      catch (Exception ex)
      {
        Mod.ErrorOnce($"Entry {__instance.LogID} - An error occurred in PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.\r\n{ex}", 45387931);
      }
    }
  }
}
