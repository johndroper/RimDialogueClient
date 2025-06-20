#nullable enable
using RimDialogue.Access;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;
using static UnityEngine.GraphicsBuffer;

namespace RimDialogue.Core.InteractionDefs
{
  public class Thought_InteractionDef : InteractionDef
  {
    public Dictionary<string, string> Constants;

    protected List<Rule> rulesRaw = [];

    public Thought thought;

    public Thought_InteractionDef(
      InteractionDef baseDef,
      Thought thought,
      Pawn? otherPawn = null,
      Dictionary<string, string>? constants = null) 
    {
      this.thought = thought;

      if (constants != null)
        Constants = constants;
      else
        Constants = [];

      Constants.Add("hasOtherPawn", (otherPawn != null).ToString());

      if (otherPawn != null)
      {
        rulesRaw.AddRange(GrammarUtility.RulesForPawn("OTHER_PAWN", otherPawn, Constants));
        // if (Settings.VerboseLogging.Value) Mod.Log($"Rules added for '{otherPawn}'.");
      }

      this.rulesRaw.AddRange([
        new Rule_String("thought_label", thought.LabelCap.ToLower()),
        new Rule_String("thought_description", thought.Description),
        ]);

      this.label = baseDef.label;
      this.defName = baseDef.defName;
      this.interactionMote = baseDef.interactionMote;

      var baseRulesStrings = Reflection.Verse_RulePack_RulesStrings.GetValue(baseDef.logRulesInitiator);
      this.logRulesInitiator = new RulePack();
      Reflection.Verse_RulePack_RulesStrings.SetValue(this.logRulesInitiator, baseRulesStrings);
      Reflection.Verse_RulePack_RulesRaw.SetValue(this.logRulesInitiator, rulesRaw);

      var symbol = (string)Reflection.RimWorld_InteractionDef_Symbol.GetValue(baseDef);
      Reflection.RimWorld_InteractionDef_Symbol.SetValue(this, symbol);
    }
  }
}
