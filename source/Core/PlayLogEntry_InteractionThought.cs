#nullable enable
using RimDialogue.Core.InteractionData;
using RimDialogue.Core.InteractionDefs;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core
{
    public class PlayLogEntry_InteractionThought : PlayLogEntry_InteractionSinglePawn
    {
        protected Thought_InteractionDef InteractionDef;
        protected string? _Text = null;

        public static void ImitateInteractionWithNoPawn(Pawn initiator, Thought_InteractionDef intDef)
        {
          if (initiator == null || initiator.DeadOrDowned)
            return;

          MoteMaker.MakeInteractionBubble(initiator, null, intDef.interactionMote, intDef.GetSymbol(), intDef.GetSymbolColor());
          Find.PlayLog.Add(new PlayLogEntry_InteractionThought(intDef, initiator, null));
        }

#pragma warning disable CS8618
    public PlayLogEntry_InteractionThought()
        {
        }
#pragma warning restore CS8618

        public PlayLogEntry_InteractionThought(Thought_InteractionDef interactionDef, Pawn initiator, List<RulePackDef> extraSentencePacks)
            : base(interactionDef, initiator, extraSentencePacks)
        {
            InteractionDef = interactionDef;
        }

        protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
        {
            if (_Text != null)
                return _Text;

            if (initiator == null)
            {
                Log.ErrorOnce("PlayLogEntry_InteractionThought has a null pawn reference.", 34423);
                return "[" + intDef.label + " error: null pawn reference]";
            }
            Rand.PushState();
            Rand.Seed = logID;
            GrammarRequest request = GenerateGrammarRequest();
            request.Constants.AddRange(InteractionDef.Constants);
            if (pov == initiator)
            {
                request.IncludesBare.Add(intDef.logRulesInitiator);
                request.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", initiator, request.Constants));
                // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {LogID} - Generating thought interaction text for '{initiator.Name}' with thought interaction def '{InteractionDef.defName}'.");
                _Text = GrammarResolver.Resolve("r_logentry", request, "thought interaction from initiator", forceLog);
            }
            else
            {
                Log.ErrorOnce("Cannot display PlayLogEntry_InteractionThought from POV who isn't initiator.", 51252);
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

            if (Settings.OnlyColonists.Value && !initiator.IsColonist)
                return _Text;

            // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {LogID} - Generating thought interaction text for '{initiator.Name}' with thought interaction def '{InteractionDef.defName}'.");
            var dialogueRequest = DialogueRequest.Create(
                this,
                _Text,
                InteractionDef);
            // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {LogID} - New {dialogueRequest.GetType().Name} thought interaction: '{_Text}'");

            return _Text;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _Text, "_Text");
        }
    }
}
