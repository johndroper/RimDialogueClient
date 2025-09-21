#nullable enable
using HarmonyLib;
using RimDialogue.Core;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RimDialogue.Access
{
  [HarmonyPatch]
  public static class Verse_LetterMaker_MakeLetter
  {
    public static MethodBase TargetMethod()
    {
      return AccessTools.Method(typeof(LetterMaker), "MakeLetter",
        new[] {
            typeof(TaggedString),
            typeof(TaggedString),
            typeof(LetterDef),
            typeof(LookTargets),
            typeof(Faction),
            typeof(Quest),
            typeof(List<ThingDef>)
        });
    }

    public static void Prefix(
      ref TaggedString label,
      ref TaggedString text,
      ref LetterDef def,
      ref LookTargets lookTargets,
      ref Faction relatedFaction,
      ref Quest quest,
      ref List<ThingDef> hyperlinkThingDefs)
    {
      try
      {
        if (Settings.VerboseLogging.Value)
          Mod.Log($"Creating LetterRecord {def.defName} - {label}\n{text}");
        var record = new RecentLetter(
          H.RemoveWhiteSpaceAndColor(label.ToString()),
          H.RemoveWhiteSpaceAndColor(text.ToString()),
          def.defName,
          lookTargets?.PrimaryTarget.Pawn);
        GameComponent_LetterTracker.Instance.RecordLetter(record);
      }
      catch (System.Exception ex)
      {
        Mod.ErrorOnce($"An error occurred in Verse_LetterMaker_MakeLetter.\r\n{ex}", 901823732);
      }
    }
  }
}
