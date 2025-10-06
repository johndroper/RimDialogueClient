#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionDefs
{
  public class RangedImpact_InteractionDef : BattleLogEntry_InteractionDef
  {
    public RangedImpact_InteractionDef(
      InteractionDef baseDef,
      Pawn? targetPawn,
      Pawn? originalTargetPawn,
      string combatLogText,
      List<BodyPartRecord> damagedParts,
      List<bool> damagedPartsDestroyed,
      bool deflected,
      ThingDef? weaponDef,
      ThingDef? projectileDef,
      Dictionary<string, string>? constants = null) : base(baseDef, combatLogText, originalTargetPawn, constants)
    {
      Constants.AddRange(
        new Dictionary<string, string>
        {
            { DEFLECTED, deflected.ToString() },
            { WEAPON_EXISTS, (weaponDef != null).ToString() },
            { PROJECTILE_EXISTS, (projectileDef != null).ToString() },
        });

      if (originalTargetPawn != null)
      {
        // if (Settings.VerboseLogging.Value) Mod.Log($"Added rules for '{originalTargetPawn}'.");
        this.rulesRaw.AddRange(GrammarUtility.RulesForPawn("ORIGINAL_TARGET", originalTargetPawn));
      }

      if (targetPawn != null && targetPawn.RaceProps != null)
      {
        this.rulesRaw.AddRange(PlayLogEntryUtility.RulesForDamagedParts(
          "target_part",
          targetPawn.RaceProps.body,
          damagedParts,
          damagedPartsDestroyed,
          Constants));
      }

      if (weaponDef != null && projectileDef != null)
        this.rulesRaw.AddRange(PlayLogEntryUtility.RulesForOptionalWeapon("WEAPON", weaponDef, projectileDef));
    }
  }
}
