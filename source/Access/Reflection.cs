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
