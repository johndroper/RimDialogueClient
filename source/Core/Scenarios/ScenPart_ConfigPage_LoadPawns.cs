#nullable enable
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimDialogue.Core.Scenarios
{
  public class ScenPart_ConfigPage_LoadPawns : ScenPart_ConfigPage
  {
    // UI state
    public string selectedFileName = string.Empty;
    private Vector2 filesScroll;

    // Cached absolute directory path
    private string? startingPawnsDir = Path.Combine(
      Mod.Instance.Content.RootDir,
      "Common",
      "StartingPawns");

    // Deserialized pawns from XML
    private List<PawnBlueprint> loadedPawns = new();

    private bool loaded = false;

    public override string Summary(Scenario scen)
    {
      return $"Load starting pawns from XML: {selectedFileName ?? "(none)"}";
    }

    public override void PostIdeoChosen()
    {
      try
      {
        if (!loaded)
        {
          LoadSelectedXml(showMessages: false);
          loaded = true;
        }

        if (loadedPawns.Count == 0) return;

        Find.GameInitData.startingPawnCount = StartingPawnCount;

        StartingPawnUtility.ClearAllStartingPawns();
        Dictionary<string, Pawn> pawns = [];

        for (int i = 0; i < loadedPawns.Count; i++)
        {
          var bp = loadedPawns[i];

          var req = StartingPawnUtility.GetGenerationRequest(i) with { KindDef = PawnKindDefOf.Colonist };
          req.RelationWithExtraPawnChanceFactor = 0f;
          StartingPawnUtility.SetGenerationRequest(i, req);
          StartingPawnUtility.AddNewPawn(i);
          Pawn pawn = Find.GameInitData.startingAndOptionalPawns[i];
          ApplyBlueprintToPawn(pawn, bp);
          pawns.Add(bp.Name.First, pawn);
          pawn.relations.ClearAllRelations();
          if (!string.IsNullOrWhiteSpace(bp.PromptInstructions))
            GameComponent_ConversationTracker.Instance?.AddAdditionalInstructions(pawn, bp.PromptInstructions.Trim());
        }

        //Add Relations
        for (int i = 0; i < loadedPawns.Count; i++)
        {
          var bp = loadedPawns[i];
          if (!pawns.ContainsKey(bp.Name.First))
          {
            Mod.Warning($"Could not find pawn for relations: {bp.Name.Full}");
            continue;
          }
          var pawn = pawns[bp.Name.First];
          //Relations
          foreach (var relation in bp.Relations)
          {
            var relationDef = DefDatabase<PawnRelationDef>.GetNamedSilentFail(relation.RelationDef);
            if (relationDef == null || !pawns.ContainsKey(relation.OtherPawn))
            {
              Mod.Warning($"Could not find relation or other pawn for relation: {bp.Name.Full} {relation.RelationDef} {relation.OtherPawn}");
              continue;
            }
            var otherPawn = pawns[relation.OtherPawn];
            Mod.Log($"Adding relation for {pawn}: {relationDef.defName} {otherPawn}");
            pawn.relations.AddDirectRelation(relationDef, otherPawn);
          }
        }

        while (Find.GameInitData.startingAndOptionalPawns.Count < 10)
          StartingPawnUtility.AddNewPawn();
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in PostIdeoChosen. {ex}");
      }
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look(ref selectedFileName, "selectedFileName", string.Empty);
      // Do not persist loadedPawns; XML is the source of truth.
    }

    private int StartingPawnCount { get; set; }
    private void LoadSelectedXml(bool showMessages)
    {
      loadedPawns.Clear();
      if (startingPawnsDir == null || string.IsNullOrEmpty(selectedFileName))
      {
        Mod.Error($"No staring pawns xml file selected. {startingPawnsDir} {selectedFileName}");
        return;
      }

      var abs = Path.Combine(startingPawnsDir, selectedFileName);
      try
      {
        var serializer = new XmlSerializer(typeof(StartingPawnsFile));
        using var fs = File.OpenRead(abs);
        if (serializer.Deserialize(fs) is StartingPawnsFile f && f.Pawns != null)
        {
          StartingPawnCount = f.StartingPawnCount;
          loadedPawns = f.Pawns.Where(p => p != null).ToList();
          Mod.Log($"Loaded {loadedPawns.Count} pawn(s) from {selectedFileName}.");
        }
        else if (showMessages)
        {
          Mod.Error("File did not contain any pawns.");
        }
      }
      catch (Exception ex)
      {
        Mod.Error($"Failed to load starting pawns XML '{abs}': {ex}");
        if (showMessages) Messages.Message("Error loading XML. See log.", MessageTypeDefOf.RejectInput, false);
      }
    }

    private static void ApplyBlueprintToPawn(Pawn pawn, PawnBlueprint bp)
    {
      switch (bp.Gender.ToLowerInvariant())
      {
        case "male":
          pawn.gender = Gender.Male;
          break;
        case "female":
          pawn.gender = Gender.Female;
          break;
        default:
          pawn.gender = Gender.None;
          break;
      }

      var xenotypeDef = DefDatabase<XenotypeDef>.GetNamedSilentFail(bp.Xenotype);
      if (xenotypeDef != null)
        pawn.genes.SetXenotype(xenotypeDef);

      // Name
      var first = string.IsNullOrWhiteSpace(bp.Name.First) ? pawn.Name?.ToStringShort ?? "Colonist" : bp.Name.First;
      var nick = bp.Name.Nick ?? string.Empty;
      var last = bp.Name.Last ?? string.Empty;
      pawn.Name = new NameTriple(first, nick, last);

      // Age
      if (bp.Age.BiologicalYears > 0)
        pawn.ageTracker.AgeBiologicalTicks = bp.Age.BiologicalYears * GenDate.TicksPerYear;
      if (bp.Age.ChronologicalYears > 0)
        pawn.ageTracker.AgeChronologicalTicks = bp.Age.ChronologicalYears * GenDate.TicksPerYear;

      // Appearance
      if (pawn.story != null)
      {
        if (!string.IsNullOrWhiteSpace(bp.Appearance.BodyTypeDef))
        {
          var bd = DefDatabase<BodyTypeDef>.GetNamedSilentFail(bp.Appearance.BodyTypeDef);
          if (bd != null) pawn.story.bodyType = bd;
        }
        if (!string.IsNullOrWhiteSpace(bp.Appearance.HeadTypeDef))
        {
          var hd = DefDatabase<HeadTypeDef>.GetNamedSilentFail(bp.Appearance.HeadTypeDef);
          if (hd != null) pawn.story.headType = hd;
        }
        if (!string.IsNullOrWhiteSpace(bp.Appearance.HairDef))
        {
          var hair = DefDatabase<HairDef>.GetNamedSilentFail(bp.Appearance.HairDef);
          if (hair != null) pawn.story.hairDef = hair;
        }
        if (bp.Appearance.HairColor.HasValue())
          pawn.story.HairColor = bp.Appearance.HairColor.ToColor(pawn.story.HairColor);
        if (bp.Appearance.Melanin > 0f)
          pawn.story.skinColorOverride = PawnSkinColors.GetSkinColor(bp.Appearance.Melanin);
      }

      if (pawn.style != null)
      {
        if (!string.IsNullOrWhiteSpace(bp.Appearance.BeardDef))
        {
          var beard = DefDatabase<BeardDef>.GetNamedSilentFail(bp.Appearance.BeardDef);
          if (beard != null) pawn.style.beardDef = beard;
        }

        if (!string.IsNullOrWhiteSpace(bp.Appearance.BodyTattooDef))
        {
          var tat = DefDatabase<TattooDef>.GetNamedSilentFail(bp.Appearance.BodyTattooDef);
          if (tat != null) pawn.style.BodyTattoo = tat;
        }

        if (!string.IsNullOrWhiteSpace(bp.Appearance.FaceTattooDef))
        {
          var tat = DefDatabase<TattooDef>.GetNamedSilentFail(bp.Appearance.FaceTattooDef);
          if (tat != null) pawn.style.FaceTattoo = tat;
        }
      }


      // Backstory
      if (pawn.story != null)
      {
        if (!string.IsNullOrWhiteSpace(bp.Backstory.Childhood))
        {
          var bs = DefDatabase<BackstoryDef>.GetNamedSilentFail(bp.Backstory.Childhood);
          if (bs != null) pawn.story.Childhood = bs;
        }
        if (!string.IsNullOrWhiteSpace(bp.Backstory.Adulthood))
        {
          var bs = DefDatabase<BackstoryDef>.GetNamedSilentFail(bp.Backstory.Adulthood);
          if (bs != null) pawn.story.Adulthood = bs;
        }
      }

      // Traits
      if (pawn.story?.traits != null && bp.Traits != null)
      {
        while (pawn.story.traits.allTraits.Any())
        {
          var trait = pawn.story.traits.allTraits[0];
          pawn.story.traits.RemoveTrait(trait);
        }
        foreach (var t in bp.Traits)
        {
          if (string.IsNullOrWhiteSpace(t.DefName)) continue;
          var def = DefDatabase<TraitDef>.GetNamedSilentFail(t.DefName);
          if (def == null) continue;
          var tr = new Trait(def, t.Degree, forced: true);
          if (!pawn.story.traits.HasTrait(def))
            pawn.story.traits.GainTrait(tr);
        }
      }

      // Skills
      if (pawn.skills != null && bp.Skills != null)
      {
        foreach (var s in bp.Skills)
        {
          if (string.IsNullOrWhiteSpace(s.DefName)) continue;
          var def = DefDatabase<SkillDef>.GetNamedSilentFail(s.DefName);
          if (def == null) continue;
          var rec = pawn.skills.GetSkill(def);
          if (rec != null)
          {
            rec.Level = Mathf.Clamp(s.Level, 0, 20);
            rec.passion = s.Passion switch
            {
              "None" => Passion.None,
              "Minor" => Passion.Minor,
              "Major" => Passion.Major,
              _ => rec.passion
            };
          }
        }
      }

      //hediffs
      if (bp.Hediffs != null)
      {
        pawn.health.RemoveAllHediffs();
        foreach (var h in bp.Hediffs)
        {
          if (string.IsNullOrWhiteSpace(h.DefName)) continue;
          var def = DefDatabase<HediffDef>.GetNamedSilentFail(h.DefName);
          if (def == null) continue;

          if (!string.IsNullOrWhiteSpace(h.BodyPartDef))
          {
            var part = pawn.def.race.body.AllParts.FirstOrDefault(p => p.def.defName == h.BodyPartDef);
            if (part == null)
              continue;
            pawn.health.AddHediff(def, part);
          }
        }
      }
    }
  }
}

