//#nullable enable
//using RimDialogue.Core.InteractionData;
//using RimDialogue.Core.InteractionRequests;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Verse;
//using Verse.Grammar;
//using static UnityEngine.Networking.UnityWebRequest;

//namespace RimDialogue.Core.PlayLogEntries
//{
//  public class PlayLogEntry_DialogueInteraction : PlayLogEntry_Interaction
//  {
//    public InteractionDef InteractionDef;
//    public Pawn Initiator;
//    public Pawn Recipient;
//    public string? Text;
//    public bool Executed = false;

//#pragma warning disable CS8618 
//    public PlayLogEntry_DialogueInteraction()
//#pragma warning restore CS8618 
//    {

//    }

//    public PlayLogEntry_DialogueInteraction(
//      InteractionDef intDef,
//      Pawn initiator,
//      Pawn recipient,
//      List<RulePackDef> extraSentencePacks) : base(
//        intDef,
//        initiator,
//        recipient,
//        extraSentencePacks)
//    {
//      InteractionDef = intDef;
//      Initiator = initiator;
//      Recipient = recipient;
//    }

//    //public virtual DialogueRequest Request => new DialogueRequestTwoPawn(
//    //  InteractionDef,
//    //  Initiator,
//    //  Recipient);

//    public virtual Rule[] Rules
//    {
//      get
//      {
//        return [];
//      }
//    }

//    protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
//    {
//      if (Text != null)
//        return Text;

//      if (GameComponent_ConversationTracker.ExecutedLogEntries.Contains(this.LogID))
//        return string.Empty;

//      if (Settings.VerboseLogging.Value)
//        Mod.Log($"Entry {this.LogID} - {this.GetType().Name} {Initiator} -> {Recipient}");

//      if (InteractionDef == null)
//        throw new Exception($"Entry {this.LogID} - PlayLogEntry_Interaction has a null InteractionDef reference.");

//      if (Initiator == null || Recipient == null)
//        throw new Exception($"Entry {this.LogID} - PlayLogEntry_Interaction has a null pawn reference.");

//      GrammarRequest grammarRequest = this.GenerateGrammarRequest();

//      grammarRequest.Rules.AddRange(this.Rules);

//      Rand.PushState();
//      Rand.Seed = this.LogID;

//      if (pov == Initiator)
//      {
//        grammarRequest.IncludesBare.Add(InteractionDef.logRulesInitiator);
//        grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", Initiator, grammarRequest.Constants));
//        grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", Recipient, grammarRequest.Constants));
//        Text = GrammarResolver.Resolve("r_logentry", grammarRequest, "interaction from initiator", forceLog);
//      }
//      else if (pov == Recipient)
//      {
//        if (InteractionDef.logRulesRecipient != null)
//          grammarRequest.IncludesBare.Add(InteractionDef.logRulesRecipient);
//        else
//          grammarRequest.IncludesBare.Add(InteractionDef.logRulesInitiator);
//        grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", Initiator, grammarRequest.Constants));
//        grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", Recipient, grammarRequest.Constants));
//        Text = GrammarResolver.Resolve("r_logentry", grammarRequest, "interaction from recipient", forceLog);
//      }
//      else
//      {
//        Log.ErrorOnce("Cannot display PlayLogEntry_Interaction from POV who isn't initiator or recipient.", 51251);
//      }
//      if (extraSentencePacks != null)
//      {
//        for (int i = 0; i < extraSentencePacks.Count; i++)
//        {
//          grammarRequest.Clear();
//          grammarRequest.Includes.Add(extraSentencePacks[i]);
//          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", Initiator, grammarRequest.Constants));
//          grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", Recipient, grammarRequest.Constants));
//          Text = Text + " " + GrammarResolver.Resolve(extraSentencePacks[i].FirstRuleKeyword, grammarRequest, "extraSentencePack", forceLog, extraSentencePacks[i].FirstUntranslatedRuleKeyword);
//        }
//      }
//      Rand.PopState();
//      return Text ?? string.Empty;
//    }
//  }
//}

