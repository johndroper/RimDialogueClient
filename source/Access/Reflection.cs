using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace RimDialogue.Access
{
  public static class Reflection
  {
    public static readonly FieldInfo Verse_PlayLogEntry_Interaction_Initiator = AccessTools.Field(typeof(PlayLogEntry_Interaction), "initiator");
    public static readonly FieldInfo Verse_PlayLogEntry_Interaction_Recipient = AccessTools.Field(typeof(PlayLogEntry_Interaction), "recipient");
    public static readonly FieldInfo Verse_PlayLogEntry_InteractionSinglePawn_Initiator = AccessTools.Field(typeof(PlayLogEntry_InteractionSinglePawn), "initiator");
    public static readonly FieldInfo Verse_PlayLogEntry_Interaction_InteractionDef = AccessTools.Field(typeof(PlayLogEntry_Interaction), "intDef");
    public static readonly FieldInfo Verse_PlayLogEntry_InteractionSinglePawn_InteractionDef = AccessTools.Field(typeof(PlayLogEntry_InteractionSinglePawn), "intDef");

    public static readonly MethodInfo Bubbles_Bubbler_ShouldShow = AccessTools.Method(typeof(Bubbles.Core.Bubbler), "ShouldShow");
    public static readonly FieldInfo Bubbles_Bubbler_Dictionary = AccessTools.Field(typeof(Bubbles.Core.Bubbler), "Dictionary");

    public static readonly FieldInfo Bubbles_Settings_DoNonPlayer = AccessTools.Field(typeof(Bubbles.Settings), "DoNonPlayer");
    public static readonly FieldInfo Bubbles_Settings_DoAnimals = AccessTools.Field(typeof(Bubbles.Settings), "DoAnimals");
    public static readonly FieldInfo Bubbles_Settings_DoDrafted = AccessTools.Field(typeof(Bubbles.Settings), "DoDrafted");

    public static readonly FieldInfo Verse_Rule_String_Output = AccessTools.Field(typeof(Verse.Grammar.Rule_String), "output");
    public static readonly FieldInfo RimWorld_InteractionDef_Symbol = AccessTools.Field(typeof(RimWorld.InteractionDef), "symbol");

    public static readonly FieldInfo Verse_Messages_LiveMessages = AccessTools.Field(typeof(Verse.Messages), "liveMessages");

    public static readonly FieldInfo RimWorld_ColonistBar_CachedDrawLocs = AccessTools.Field(typeof(RimWorld.ColonistBar), "cachedDrawLocs");
    public static readonly FieldInfo RimWorld_ColonistBar_TmpColonistsInOrder = AccessTools.Field(typeof(RimWorld.ColonistBar), "tmpColonistsInOrder");

    //ALERTS
    public static readonly FieldInfo RimWorld_Alert_AbandonedBaby_tmpAbandonedBabiesList = AccessTools.Field(typeof(RimWorld.Alert_AbandonedBaby), "tmpAbandonedBabiesList");
    public static readonly FieldInfo RimWorld_AlertsReadout_ActiveAlerts = AccessTools.Field(typeof(RimWorld.AlertsReadout), "activeAlerts");
    public static readonly FieldInfo RimWorld_Alert_ColonistsIdle_IdleColonistsResult = AccessTools.Field(typeof(RimWorld.Alert_ColonistsIdle), "idleColonistsResult");
    public static readonly FieldInfo RimWorld_Alert_HunterLacksRangedWeapon_HuntersWithoutRangedWeaponResult = AccessTools.Field(typeof(RimWorld.Alert_HunterLacksRangedWeapon), "huntersWithoutRangedWeaponResult");
    public static readonly FieldInfo RimWorld_Alert_Heatstroke_HeatstrokePawnsResult = AccessTools.Field(typeof(RimWorld.Alert_Heatstroke), "heatstrokePawnsResult");
    public static readonly FieldInfo RimWorld_Alert_AnimalFilth_Targets = AccessTools.Field(typeof(RimWorld.Alert_AnimalFilth), "targets");
    public static readonly FieldInfo RimWorld_Alert_AnimalPenNeeded_Targets = AccessTools.Field(typeof(RimWorld.Alert_AnimalPenNeeded), "targets");
    public static readonly FieldInfo RimWorld_Alert_AnimalRoaming_Targets = AccessTools.Field(typeof(RimWorld.Alert_AnimalRoaming), "targets");
    public static readonly FieldInfo RimWorld_Alert_Boredom_BoredPawnsResult = AccessTools.Field(typeof(RimWorld.Alert_Boredom), "boredPawnsResult");
    public static readonly FieldInfo RimWorld_Alert_BrawlerHasRangedWeapon_BrawlersWithRangedWeaponResult = AccessTools.Field(typeof(RimWorld.Alert_BrawlerHasRangedWeapon), "brawlersWithRangedWeaponResult");
    public static readonly FieldInfo RimWorld_Alert_ColonistLeftUnburied_UnburiedColonistCorpsesResult = AccessTools.Field(typeof(RimWorld.Alert_ColonistLeftUnburied), "unburiedColonistCorpsesResult");
    public static readonly FieldInfo RimWorld_Alert_BreakRiskAlertUtility_PawnsAtRiskMajorResult = AccessTools.Field(typeof(RimWorld.BreakRiskAlertUtility), "pawnsAtRiskMajorResult");
    public static readonly FieldInfo RimWorld_Alert_BreakRiskAlertUtility_PawnsAtRiskMinorResult = AccessTools.Field(typeof(RimWorld.BreakRiskAlertUtility), "pawnsAtRiskMinorResult");
    public static readonly FieldInfo RimWorld_Alert_ColonistNeedsRescuing_ColonistsNeedingRescueResult = AccessTools.Field(typeof(RimWorld.Alert_ColonistNeedsRescuing), "colonistsNeedingRescueResult");
    public static readonly FieldInfo RimWorld_Alert_ColonistNeedsTend_NeedingColonistsResult = AccessTools.Field(typeof(RimWorld.Alert_ColonistNeedsTend), "needingColonistsResult");
    public static readonly FieldInfo RimWorld_Alert_DateRitualComing_RitualEntries = AccessTools.Field(typeof(RimWorld.Alert_DateRitualComing), "ritualEntries");
    public static readonly FieldInfo RimWorld_Alert_Hypothermia_HypothermiaPawnsResult = AccessTools.Field(typeof(RimWorld.Alert_Hypothermia), "hypothermiaPawnsResult");
    public static readonly FieldInfo RimWorld_Alert_HypothermicAnimals_HypothermicAnimalsResult = AccessTools.Field(typeof(RimWorld.Alert_HypothermicAnimals), "hypothermicAnimalsResult");
    public static readonly FieldInfo RimWorld_Alert_IdeoBuildingDisrespected_Demand = AccessTools.Field(typeof(RimWorld.Alert_IdeoBuildingDisrespected), "demand");
    public static readonly FieldInfo RimWorld_Alert_IdeoBuildingMissing_Demand = AccessTools.Field(typeof(RimWorld.Alert_IdeoBuildingMissing), "demand");
    public static readonly FieldInfo RimWorld_Alert_QuestExpiresSoon_QuestExpiring = AccessTools.Field(typeof(RimWorld.Alert_QuestExpiresSoon), "questExpiring");
    public static readonly FieldInfo RimWorld_Alert_StarvationAnimals_StarvingAnimalsResult = AccessTools.Field(typeof(RimWorld.Alert_StarvationAnimals), "starvingAnimalsResult");
    public static readonly FieldInfo RimWorld_Alert_StarvationColonists_StarvingColonistsResult = AccessTools.Field(typeof(RimWorld.Alert_StarvationColonists), "starvingColonistsResult");
    public static readonly FieldInfo RimWorld_Alert_UnhappyNudity_AffectedPawnsResult = AccessTools.Field(typeof(RimWorld.Alert_UnhappyNudity), "affectedPawnsResult");


    private static readonly Dictionary<string, bool> _isAssemblyLoaded = [];
    public static bool IsAssemblyLoaded(string assemblyName)
    {
      if (_isAssemblyLoaded.ContainsKey(assemblyName))
        return _isAssemblyLoaded[assemblyName];
      _isAssemblyLoaded[assemblyName] = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == assemblyName);
      return _isAssemblyLoaded[assemblyName];
    }

    private static MethodInfo tryGetEnneagramCompMethod;
    private static MethodInfo getDescriptionMethodInfo;
    private static PropertyInfo enneagramProperty;
    private static MethodInfo toStringMethod;

    static Reflection()
    {
      var spm1Assembly = AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(a => a.GetName().Name == "SP_Module1");
      if (spm1Assembly == null)
        return;
      var spm1ExtensionsType = spm1Assembly.GetType("SPM1.Extensions");
      if (spm1ExtensionsType == null)
        return;
      tryGetEnneagramCompMethod = spm1ExtensionsType.GetMethod("TryGetEnneagramComp", [typeof(ThingWithComps)]);
      if (tryGetEnneagramCompMethod == null)
        return;
      var enneagramCompType = spm1Assembly.GetType("SPM1.Comps.CompEnneagram");
      if (enneagramCompType == null)
        return;
      getDescriptionMethodInfo = enneagramCompType.GetMethod("GetDescription");
      if (getDescriptionMethodInfo == null)
        return;
      enneagramProperty = enneagramCompType.GetProperty("Enneagram");
      if (enneagramProperty == null)
        return;
      var enneagramObjectType = spm1Assembly.GetType("SPM1.Enneagram");
      toStringMethod = enneagramObjectType.GetMethod("ToString");
      if (toStringMethod == null)
        return;
    }

    public static void GetPersonality(Pawn pawn, out string label, out string description)
    {
      label = null;
      description = null;
      if (pawn == null || tryGetEnneagramCompMethod == null)
        return;
      object enneagramCompObject = tryGetEnneagramCompMethod.Invoke(null, [pawn]);
      if (enneagramCompObject == null)
        return;
      description = (string)getDescriptionMethodInfo.Invoke(enneagramCompObject, null);
      var enneagramObject = enneagramProperty.GetValue(enneagramCompObject);
      label = (string)toStringMethod.Invoke(enneagramObject, null);
    }
  }
}
