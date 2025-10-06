#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionDefs
{
  public class RangedFire_InteractionDef : BattleLogEntry_InteractionDef
  {
    public RangedFire_InteractionDef(
      InteractionDef baseDef,
      Pawn target,
      string combatLogText,
      ThingDef? weaponDef,
      ThingDef? projectileDef,
      Dictionary<string, string>? constants = null) :
        base(baseDef, combatLogText, target, constants)
    {
      Constants.AddRange(
        new Dictionary<string, string>
        {
          { WEAPON_EXISTS, (weaponDef != null).ToString() },
          { PROJECTILE_EXISTS, (projectileDef != null).ToString() },
        });

      if (weaponDef != null)
        this.rulesRaw.AddRange(
          PlayLogEntryUtility.RulesForOptionalWeapon("WEAPON", weaponDef, projectileDef));
    }
  }
}
