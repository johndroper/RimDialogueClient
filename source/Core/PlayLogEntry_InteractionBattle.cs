using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionDefs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core
{
  public class PlayLogEntry_InteractionBattle : PlayLogEntry_InteractionSinglePawn
  {

    public static void ImitateInteractionWithNoPawn(Pawn initiator, BattleLogEntry_InteractionDef intDef)
    {
      MoteMaker.MakeInteractionBubble(initiator, null, intDef.interactionMote, intDef.GetSymbol(), intDef.GetSymbolColor());
      Find.PlayLog.Add(new PlayLogEntry_InteractionBattle(intDef, initiator, null));
    }

    protected BattleLogEntry_InteractionDef InteractionDef { get; set; }

    public PlayLogEntry_InteractionBattle(BattleLogEntry_InteractionDef interactionDef, Pawn initiator, List<RulePackDef> extraSentencePacks) :
      base(interactionDef, initiator, extraSentencePacks)
    {
      InteractionDef = interactionDef;
    }

    protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
    {
      if (initiator == null)
      {
        Log.ErrorOnce("PlayLogEntry_InteractionBattle has a null pawn reference.", 34422);
        return "[" + intDef.label + " error: null pawn reference]";
      }
      Rand.PushState();
      Rand.Seed = logID;
      GrammarRequest request = GenerateGrammarRequest();
      request.Constants.AddRange(InteractionDef.Constants);
      string text;
      if (pov == initiator)
      {
        request.IncludesBare.Add(intDef.logRulesInitiator);
        request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
        text = GrammarResolver.Resolve("r_logentry", request, "interaction from initiator", forceLog);
      }
      else
      {
        Log.ErrorOnce("Cannot display PlayLogEntry_InteractionBattle from POV who isn't initiator.", 51251);
        text = ToString();
      }
      if (extraSentencePacks != null)
      {
        for (int i = 0; i < extraSentencePacks.Count; i++)
        {
          request.Clear();
          request.Includes.Add(extraSentencePacks[i]);
          request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
          text = text + " " + GrammarResolver.Resolve(extraSentencePacks[i].FirstRuleKeyword, request, "extraSentencePack", forceLog, extraSentencePacks[i].FirstUntranslatedRuleKeyword);
        }
      }
      Rand.PopState();

      if (Settings.OnlyColonists.Value && !initiator.IsColonist)
        return text;

      var dialogueRequest = DialogueRequest.Create(
        this,
        text,
        InteractionDef);
      if (Settings.VerboseLogging.Value) Mod.Log($"Entry {LogID} - New {dialogueRequest.GetType().Name} interaction: '{text}'");

      return text;
    }
  }
}
