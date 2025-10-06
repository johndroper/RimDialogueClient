#nullable enable
using RimDialogue.Access;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionDefs
{
  public abstract class BattleLogEntry_InteractionDef : InteractionDef
  {
    public const string DEFLECTED = "deflected";
    public const string WEAPON_EXISTS = "weapon_exists";
    public const string PROJECTILE_EXISTS = "projectile_exists";

    public Dictionary<string, string> Constants;

    protected List<Rule> rulesRaw = [];


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BattleLogEntry_InteractionDef()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public BattleLogEntry_InteractionDef(
      InteractionDef baseDef,
      string combatLogText,
      Pawn? target,
      Dictionary<string, string>? constants)
    {
      if (constants != null)
        Constants = constants;
      else
        Constants = [];

      Target = target;
      CombatLogText = H.RemoveWhiteSpaceAndColor(combatLogText);

      this.defName = baseDef.defName;
      this.label = baseDef.label;
      this.interactionMote = baseDef.interactionMote;

      var baseRulesStrings = Reflection.Verse_RulePack_RulesStrings.GetValue(baseDef.logRulesInitiator);
      this.logRulesInitiator = new RulePack();
      Reflection.Verse_RulePack_RulesStrings.SetValue(this.logRulesInitiator, baseRulesStrings);

      if (target != null)
      {
        rulesRaw.AddRange(GrammarUtility.RulesForPawn("TARGET", target));
        // if (Settings.VerboseLogging.Value) Mod.Log($"Rules added for '{target}'.");
      }
      else
      {
        rulesRaw.Add(new Rule_String("TARGET_nameDef", "unknown"));
        rulesRaw.Add(new Rule_String("TARGET_kind", "unknown"));
      }

      Reflection.Verse_RulePack_RulesRaw.SetValue(this.logRulesInitiator, rulesRaw);

      var symbol = (string)Reflection.RimWorld_InteractionDef_Symbol.GetValue(baseDef);
      Reflection.RimWorld_InteractionDef_Symbol.SetValue(this, symbol);
    }

    public Pawn? Target { get; set; }
    public string CombatLogText { get; set; }
  }
}
