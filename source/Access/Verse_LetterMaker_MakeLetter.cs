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
      if (Settings.VerboseLogging.Value)
        Log.Message($"Creating LetterRecord {def.defName}");

      var record = new GameComponent_LetterTracker.RecentLetter(label, text, def, lookTargets, relatedFaction, quest, hyperlinkThingDefs);

      GameComponent_LetterTracker.Instance.RecordLetter(record);
    }
  }
}
