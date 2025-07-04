# nullable enable
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
      if (initiator == null || initiator.DeadOrDowned)
        return;

      MoteMaker.MakeInteractionBubble(initiator, null, intDef.interactionMote, intDef.GetSymbol(), intDef.GetSymbolColor());
      Find.PlayLog.Add(new PlayLogEntry_InteractionBattle(intDef, initiator, null));
    }

    protected BattleLogEntry_InteractionDef InteractionDef;
    protected string? _Text = null;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PlayLogEntry_InteractionBattle()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public PlayLogEntry_InteractionBattle(BattleLogEntry_InteractionDef interactionDef, Pawn initiator, List<RulePackDef> extraSentencePacks) :
      base(interactionDef, initiator, extraSentencePacks)
    {
      InteractionDef = interactionDef;
    }

    protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
    {
      if (_Text != null)
        return _Text;

      if (initiator == null)
      {
        Log.ErrorOnce("PlayLogEntry_InteractionBattle has a null pawn reference.", 34422);
        return "[" + intDef.label + " error: null pawn reference]";
      }
      Rand.PushState();
      Rand.Seed = logID;
      GrammarRequest request = GenerateGrammarRequest();

      var dialogueRequest = DialogueRequest.Create(
        this,
        InteractionDef);

      request.Rules.AddRange(dialogueRequest.Rules);

      request.Constants.AddRange(InteractionDef.Constants);
      if (pov == initiator)
      {
        request.IncludesBare.Add(intDef.logRulesInitiator);
        request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
        _Text = GrammarResolver.Resolve("r_logentry", request, "interaction from initiator", forceLog);
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {LogID} - New battle interaction text: '{_Text}'.");
      }
      else
      {
        Log.ErrorOnce("Cannot display PlayLogEntry_InteractionBattle from POV who isn't initiator.", 51251);
        _Text = ToString();
      }
      if (extraSentencePacks != null)
      {
        for (int i = 0; i < extraSentencePacks.Count; i++)
        {
          request.Clear();
          request.Includes.Add(extraSentencePacks[i]);
          request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
          _Text = _Text + " " + GrammarResolver.Resolve(extraSentencePacks[i].FirstRuleKeyword, request, "extraSentencePack", forceLog, extraSentencePacks[i].FirstUntranslatedRuleKeyword);
        }
      }
      Rand.PopState();

      if (!DialogueRequest.TooSoon() && !DialogueRequest.TooSoonAll())
        dialogueRequest.Execute(_Text);

      if (Settings.OnlyColonists.Value && !initiator.IsColonist)
        return _Text;

      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {LogID} - New {dialogueRequest.GetType().Name} interaction: '{_Text}'");

      return _Text;
    }
    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look(ref _Text, "_Text");
    }
  }
}
