using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

    public static Dictionary<int, LetterRecord> incidentData = [];

    private static System.Random random = new();
    public static LetterRecord? GetLetter(int logId)
    {
      if (incidentData.ContainsKey(logId))
        return incidentData[logId];
      if (recentLetters.Any())
      {
        var letter = recentLetters[random.Next(recentLetters.Count)];
        incidentData.Add(logId, letter);
        return letter;
      }
      return null;
    }

    public static List<LetterRecord> recentLetters = new List<LetterRecord>();

    public class LetterRecord
    {
      public TaggedString Label { get; set; }
      public TaggedString Text { get; set; }
      public LetterDef Def { get; set; }
      public LookTargets LookTargets { get; set; }
      public Faction RelatedFaction { get; set; }
      public Quest Quest { get; set; }
      public List<ThingDef> HyperlinkThingDefs { get; set; }
      public int Ticks { get; set; } = Find.TickManager.TicksAbs;
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
      var record = new LetterRecord
      {
        Label = label,
        Text = text,
        Def = def,
        LookTargets = lookTargets,
        RelatedFaction = relatedFaction,
        Quest = quest,
        HyperlinkThingDefs = hyperlinkThingDefs
      };
      recentLetters.Add(record);
      var currentTicks = Find.TickManager.TicksAbs;
      recentLetters.RemoveAll(letter => currentTicks - letter.Ticks > Settings.RecentIncidentHours.Value * 2500);
    }
  }
}
