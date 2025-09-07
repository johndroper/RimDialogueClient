using HarmonyLib;
using RimDialogue.Context;
using RimDialogue.Core;
using RimDialogue.Core.InteractionDefs;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch(typeof(BattleLog), nameof(BattleLog.Add))]
  public static class Patch_BattleLog_Add
  {
    public static Dictionary<Thing, int> _lastInteractionTicks = [];

    public static bool TooSoon(Thing thing)
    {
      if (_lastInteractionTicks.TryGetValue(thing, out int lastInteraction)
        && Find.TickManager.TicksAbs - lastInteraction < Settings.MinDelayMinutes.Value * InteractionWorker_Dialogue.TicksPerMinute)
      {
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {thing} - Too soon to add interaction.");
        _lastInteractionTicks[thing] = Find.TickManager.TicksAbs;
        return true;
      }
      return false;
    }



     public static bool IsValid(Pawn pawn)
    {
      if (VersionControl.CurrentVersion.Major  == 1 && VersionControl.CurrentVersion.Minor >= 6)
      {
        return pawn != null && !(bool)Reflection.IsAnimal.GetValue(pawn) && !pawn.DeadOrDowned;
      }
      else
      {
        return pawn != null && !(bool)Reflection.IsNonMutantAnimal.GetValue(pawn) && !pawn.DeadOrDowned;
      }
    }

    public static void Postfix(LogEntry entry)
    {
      if (entry == null) return;
      if (!Find.BattleLog.Battles.Any())
        return;
      GameComponent_ContextTracker.Instance.Add(entry);
      Battle currentBattle = Find.BattleLog.Battles.First();
      switch (entry)
      {
        case BattleLogEntry_MeleeCombat meleeEntry:
          CreateMeleeCombatInteraction(entry, meleeEntry);
          break;
        case BattleLogEntry_RangedFire rangedFireEntry:
          CreateRangedFireInteraction(entry, rangedFireEntry);
          break;
        case BattleLogEntry_RangedImpact rangedImpactEntry:
          CreateRangedImpactInteraction(entry, rangedImpactEntry);
          CreateImHitInteraction(entry, rangedImpactEntry);
          break;
        case BattleLogEntry_DamageTaken damageTakenEntry:
          CreateDamageTakenInteraction(entry, damageTakenEntry);
          CreateImHitInteraction(entry, damageTakenEntry);
          break;
        case BattleLogEntry_ExplosionImpact explosionImpact:
          break;
        case BattleLogEntry_StateTransition stateTransitionEntry:
          break;
        case BattleLogEntry_AbilityUsed abilityUsed:
          break;
        case BattleLogEntry_ItemUsed itemUsed:
          break;
        default:
          break;
      }
    }

    public static async void CreateImHitInteraction(
      LogEntry entry,
      BattleLogEntry_DamageTaken damageTakenEntry)
    {
      try
      {
        await Task.Yield();
        if (!Rand.Chance(Settings.ImHitChance.Value))
          return;
        Pawn initiator = (Pawn)Reflection.Verse_BattleLogEntry_DamageTaken_InitiatorPawn.GetValue(damageTakenEntry);
        Pawn recipient = (Pawn)Reflection.Verse_BattleLogEntry_DamageTaken_RecipientPawn.GetValue(damageTakenEntry);
        if (recipient == null || !IsValid(recipient) || TooSoon(recipient))
          return;
        var damagedParts = (List<BodyPartRecord>)Reflection.Verse_LogEntry_DamageResult_DamagedParts.GetValue(damageTakenEntry);
        var damagedPartsDestroyed = (List<bool>)Reflection.Verse_LogEntry_DamageResult_DamagedPartsDestroyed.GetValue(damageTakenEntry);
        var deflected = (bool)Reflection.Verse_LogEntry_DamageResult_Deflected.GetValue(damageTakenEntry);

        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Im Hit interaction.");
        PlayLogEntry_InteractionBattle.ImitateInteractionWithNoPawn(
          recipient,
          new DamageTaken_InteractionDef(
            DefDatabase<InteractionDef>.GetNamed("ImHit"),
            initiator,
            H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(recipient)),
            damagedParts,
            damagedPartsDestroyed,
            deflected));
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public static async void CreateImHitInteraction(
      LogEntry entry,
      BattleLogEntry_RangedImpact rangedImpactEntry)
    {
      try
      {
        await Task.Yield();
        if (!Rand.Chance(Settings.ImHitChance.Value))
          return;
        Pawn targetPawn = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_RecipientPawn.GetValue(rangedImpactEntry);
        if (targetPawn == null || !IsValid(targetPawn) || TooSoon(targetPawn))
          return;
        Pawn initiatorPawn = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_InitiatorPawn.GetValue(rangedImpactEntry);
        //Thing initiatorThing = (Thing)Reflection.Verse_BattleLogEntry_RangedImpact_InitiatorThing.GetValue(rangedImpactEntry);
        //Thing targetThing = (Thing)Reflection.Verse_BattleLogEntry_RangedImpact_RecipientThing.GetValue(rangedImpactEntry);
        Pawn originalTargetPawn = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_OriginalTargetPawn.GetValue(rangedImpactEntry);
        //Thing originalTargetThing = (Thing)Reflection.Verse_BattleLogEntry_RangedImpact_OriginalTargetThing.GetValue(rangedImpactEntry);
        ThingDef weaponDef = (ThingDef)Reflection.Verse_BattleLogEntry_RangedImpact_WeaponDef.GetValue(rangedImpactEntry);
        ThingDef projectileDef = (ThingDef)Reflection.Verse_BattleLogEntry_RangedImpact_ProjectileDef.GetValue(rangedImpactEntry);
        var damagedParts = (List<BodyPartRecord>)Reflection.Verse_LogEntry_DamageResult_DamagedParts.GetValue(rangedImpactEntry);
        var damagedPartsDestroyed = (List<bool>)Reflection.Verse_LogEntry_DamageResult_DamagedPartsDestroyed.GetValue(rangedImpactEntry);
        var deflected = (bool)Reflection.Verse_LogEntry_DamageResult_Deflected.GetValue(rangedImpactEntry);
        if (initiatorPawn == null || initiatorPawn.DeadOrDowned || targetPawn == null || targetPawn.DeadOrDowned)
          return;
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Im Hit interaction.");
        Dictionary<string, string> constants = [];
        constants.Add("hit_target", (targetPawn == originalTargetPawn).ToString());
        PlayLogEntry_InteractionBattle.ImitateInteractionWithNoPawn(
          targetPawn,
          new RangedImpact_InteractionDef(
            DefDatabase<InteractionDef>.GetNamed("ImHit"),
            initiatorPawn,
            initiatorPawn,
            H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiatorPawn)),
            damagedParts,
            damagedPartsDestroyed,
            deflected,
            weaponDef,
            projectileDef,
            constants));
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public static async void CreateDamageTakenInteraction(
      LogEntry entry,
      BattleLogEntry_DamageTaken damageTakenEntry)
    {
      try
      {
        await Task.Yield();
        if (!Rand.Chance(Settings.DamageTakenQuipChance.Value))
          return;
        Pawn initiator = (Pawn)Reflection.Verse_BattleLogEntry_DamageTaken_InitiatorPawn.GetValue(damageTakenEntry);
        if (!IsValid(initiator) || TooSoon(initiator))
          return;
        Pawn recipient = (Pawn)Reflection.Verse_BattleLogEntry_DamageTaken_RecipientPawn.GetValue(damageTakenEntry);
        if (recipient == null)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {damageTakenEntry.LogID} - Recipient is null.");
          return;
        }
        var damagedParts = (List<BodyPartRecord>)Reflection.Verse_LogEntry_DamageResult_DamagedParts.GetValue(damageTakenEntry);
        var damagedPartsDestroyed = (List<bool>)Reflection.Verse_LogEntry_DamageResult_DamagedPartsDestroyed.GetValue(damageTakenEntry);
        var deflected = (bool)Reflection.Verse_LogEntry_DamageResult_Deflected.GetValue(damageTakenEntry);

        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Damage taken interaction.");
        PlayLogEntry_InteractionBattle.ImitateInteractionWithNoPawn(
          initiator,
          new DamageTaken_InteractionDef(
            DefDatabase<InteractionDef>.GetNamed("DamageTakenQuip"),
            recipient,
            H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiator)),
            damagedParts,
            damagedPartsDestroyed,
            deflected));
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public static async void CreateRangedImpactInteraction(
      LogEntry entry,
      BattleLogEntry_RangedImpact rangedImpactEntry)
    {
      try
      {
        await Task.Yield();

        if (!Rand.Chance(Settings.RangedImpactQuipChance.Value))
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {rangedImpactEntry.LogID} - Ranged Impact interaction - no chance.");
          return;
        }
        Pawn initiatorPawn = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_InitiatorPawn.GetValue(rangedImpactEntry);
        Pawn originalTargetPawn = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_OriginalTargetPawn.GetValue(rangedImpactEntry);
        //Thing initiatorThing = (Thing)Reflection.Verse_BattleLogEntry_RangedImpact_InitiatorThing.GetValue(rangedImpactEntry);
        Pawn targetPawn = (Pawn)Reflection.Verse_BattleLogEntry_RangedImpact_RecipientPawn.GetValue(rangedImpactEntry);
        //Thing targetThing = (Thing)Reflection.Verse_BattleLogEntry_RangedImpact_RecipientThing.GetValue(rangedImpactEntry);
        //Thing originalTargetThing = (Thing)Reflection.Verse_BattleLogEntry_RangedImpact_OriginalTargetThing.GetValue(rangedImpactEntry);
        ThingDef weaponDef = (ThingDef)Reflection.Verse_BattleLogEntry_RangedImpact_WeaponDef.GetValue(rangedImpactEntry);
        ThingDef projectileDef = (ThingDef)Reflection.Verse_BattleLogEntry_RangedImpact_ProjectileDef.GetValue(rangedImpactEntry);
        var damagedParts = (List<BodyPartRecord>)Reflection.Verse_LogEntry_DamageResult_DamagedParts.GetValue(rangedImpactEntry);
        var damagedPartsDestroyed = (List<bool>)Reflection.Verse_LogEntry_DamageResult_DamagedPartsDestroyed.GetValue(rangedImpactEntry);
        var deflected = (bool)Reflection.Verse_LogEntry_DamageResult_Deflected.GetValue(rangedImpactEntry);
        if (initiatorPawn == null || originalTargetPawn == null || targetPawn == null)
          return;
        if (targetPawn != originalTargetPawn)
          return;

        Dictionary<string, string> constants = [];
        constants.Add("hit_target", (targetPawn == originalTargetPawn).ToString());
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Ranged Impact interaction.");
        if (initiatorPawn != null)
        {
          if (!IsValid(initiatorPawn) || TooSoon(initiatorPawn))
            return;
          PlayLogEntry_InteractionBattle.ImitateInteractionWithNoPawn(
            initiatorPawn,
            new RangedImpact_InteractionDef(
              DefDatabase<InteractionDef>.GetNamed("RangedImpactQuip"),
              targetPawn,
              originalTargetPawn,
              H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiatorPawn)),
              damagedParts,
              damagedPartsDestroyed,
              deflected,
              weaponDef,
              projectileDef,
              constants));
        }
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public static async void CreateMeleeCombatInteraction(
      LogEntry entry,
      BattleLogEntry_MeleeCombat meleeEntry)
    {
      try
      {
        await Task.Yield();
        if (!Rand.Chance(Settings.MeleeCombatQuipChance.Value))
          return;
        Pawn initiator = (Pawn)Reflection.Verse_BattleLogEntry_MeleeCombat_Initiator.GetValue(meleeEntry);
        if (TooSoon(initiator))
          return;
        Pawn recipient = (Pawn)Reflection.Verse_BattleLogEntry_MeleeCombat_RecipientPawn.GetValue(meleeEntry);
        if (recipient == null)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {meleeEntry.LogID} - Recipient is null.");
          return;
        }
        var damagedParts = (List<BodyPartRecord>)Reflection.Verse_LogEntry_DamageResult_DamagedParts.GetValue(meleeEntry);
        var damagedPartsDestroyed = (List<bool>)Reflection.Verse_LogEntry_DamageResult_DamagedPartsDestroyed.GetValue(meleeEntry);
        var deflected = (bool)Reflection.Verse_LogEntry_DamageResult_Deflected.GetValue(meleeEntry);
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Melee Combat interaction.");
        PlayLogEntry_InteractionBattle.ImitateInteractionWithNoPawn(
          initiator,
          new MeleeCombat_InteractionDef(
            DefDatabase<InteractionDef>.GetNamed("MeleeCombatQuip"),
            recipient,
            H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiator)),
            damagedParts,
            damagedPartsDestroyed,
            deflected));
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public static async void CreateRangedFireInteraction(
      LogEntry entry,
      BattleLogEntry_RangedFire rangedEntry)
    {
      try
      {
        await Task.Yield();
        if (!Rand.Chance(Settings.RangedFireQuipChance.Value))
          return;
        Pawn initiator = (Pawn)Reflection.Verse_BattleLogEntry_RangedFire_InitiatorPawn.GetValue(rangedEntry);
        if (TooSoon(initiator))
          return;
        Pawn recipient = (Pawn)Reflection.Verse_BattleLogEntry_RangedFire_RecipientPawn.GetValue(rangedEntry);
        if (recipient == null)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {rangedEntry.LogID} - Recipient is null.");
          return;
        }
        ThingDef weaponDef = (ThingDef)Reflection.Verse_BattleLogEntry_RangedFire_WeaponDef.GetValue(rangedEntry);
        ThingDef projectileDef = (ThingDef)Reflection.Verse_BattleLogEntry_RangedFire_ProjectileDef.GetValue(rangedEntry);
        // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - Ranged Fire interaction.");
        PlayLogEntry_InteractionBattle.ImitateInteractionWithNoPawn(
          initiator,
          new RangedFire_InteractionDef(
            DefDatabase<InteractionDef>.GetNamed("RangedFireQuip"),
            recipient,
            H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(initiator)),
            weaponDef,
            projectileDef));
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }
  }
}
