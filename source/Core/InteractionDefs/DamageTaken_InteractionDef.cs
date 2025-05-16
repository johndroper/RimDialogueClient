#nullable enable
using RimDialogue.Access;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionDefs
{
  public class DamageTaken_InteractionDef : BattleLogEntry_InteractionDef
  {
    public DamageTaken_InteractionDef(
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
