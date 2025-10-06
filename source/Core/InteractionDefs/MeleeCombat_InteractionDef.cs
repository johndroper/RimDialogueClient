#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionDefs
{
  public class MeleeCombat_InteractionDef : BattleLogEntry_InteractionDef
  {
    public MeleeCombat_InteractionDef(
      InteractionDef baseDef,
      Pawn target,
      string combatLogText,
      List<BodyPartRecord> damagedParts,
      List<bool> damagedPartsDestroyed,
      bool deflected,
      Dictionary<string, string>? constants = null) : base(baseDef, combatLogText, target, constants)
    {
      Constants.AddRange(
        new Dictionary<string, string>
        {
            { DEFLECTED, deflected.ToString() },
            { WEAPON_EXISTS, false.ToString() },
            { PROJECTILE_EXISTS, false.ToString() },
        });

      this.rulesRaw.AddRange(PlayLogEntryUtility.RulesForDamagedParts(
        "target_part",
        target.RaceProps.body,
        damagedParts,
        damagedPartsDestroyed,
        Constants));
    }
  }
}
