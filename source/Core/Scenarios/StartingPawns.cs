using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace RimDialogue.Core.Scenarios
{
  // ---------- XML model (no IExposable, all attributes non-nullable) ----------

  [XmlRoot("StartingPawns")]
  public class StartingPawnsFile
  {
    [XmlAttribute]
    public int StartingPawnCount { get; set; } = 3;

    [XmlElement("Pawn")]
    public List<PawnBlueprint> Pawns { get; set; } = new();
  }

  public class PawnBlueprint
  {
    [XmlElement] public PawnName Name { get; set; } = new();
    [XmlElement] public PawnAge Age { get; set; } = new();
    [XmlElement] public PawnAppearance Appearance { get; set; } = new();
    [XmlElement] public PawnBackstory Backstory { get; set; } = new();

    [XmlArray] public List<PawnSkill> Skills { get; set; } = new();
    [XmlArray] public List<PawnTrait> Traits { get; set; } = new();
    [XmlArray] public List<PawnRelation> Relations { get; set; } = new();
    [XmlArray] public List<PawnHediff> Hediffs { get; set; } = new();
    [XmlElement] public string PromptInstructions { get; set; } = string.Empty;
    [XmlAttribute] public string Gender { get; set; } = string.Empty;
    [XmlAttribute] public string Xenotype { get; set; } = string.Empty;
  }

  public class PawnName
  {
    [XmlAttribute] public string First { get; set; } = string.Empty;
    [XmlAttribute] public string Nick { get; set; } = string.Empty;
    [XmlAttribute] public string Last { get; set; } = string.Empty;

    [XmlIgnore]
    public string Full
    {
      get
      {
        if (!string.IsNullOrEmpty(Nick)) return $"{First} \"{Nick}\" {Last}".Trim();
        return $"{First} {Last}".Trim();
      }
    }
  }

  public class PawnAge
  {
    [XmlAttribute] public int BiologicalYears { get; set; } = 0;
    [XmlAttribute] public int ChronologicalYears { get; set; } = 0;
  }

  public class PawnAppearance
  {
    [XmlAttribute] public string BodyTypeDef { get; set; } = string.Empty;
    [XmlAttribute] public string HeadTypeDef { get; set; } = string.Empty;
    [XmlAttribute] public string HairDef { get; set; } = string.Empty;
    [XmlAttribute] public string BeardDef { get; set; } = string.Empty;
    [XmlAttribute] public string BodyTattooDef { get; set; } = string.Empty;
    [XmlAttribute] public string FaceTattooDef { get; set; } = string.Empty;

    [XmlElement] public PawnColor HairColor { get; set; } = new();

    [XmlAttribute] public float Melanin { get; set; } = 0f;
  }

  public class PawnColor
  {
    [XmlAttribute] public float R { get; set; } = -1f;
    [XmlAttribute] public float G { get; set; } = -1f;
    [XmlAttribute] public float B { get; set; } = -1f;
    [XmlAttribute] public float A { get; set; } = -1f;

    public bool HasValue() => R >= 0f && G >= 0f && B >= 0f && A >= 0f;

    public Color ToColor(Color fallback) =>
      HasValue() ? new Color(R, G, B, A <= 0f ? 1f : A) : fallback;
  }

  public class PawnBackstory
  {
    [XmlAttribute] public string Childhood { get; set; } = string.Empty;
    [XmlAttribute] public string Adulthood { get; set; } = string.Empty;
  }

  public class PawnSkill
  {
    [XmlAttribute] public string DefName { get; set; } = string.Empty;
    [XmlAttribute] public int Level { get; set; } = 0;      // 0..20
    [XmlAttribute] public string Passion { get; set; } = "None"; // None|Minor|Major
  }

  public class PawnRelation
  {
    [XmlAttribute] public string RelationDef { get; set; } = string.Empty;
    [XmlAttribute] public string OtherPawn { get; set; } = string.Empty;
  }

  public class PawnTrait
  {
    [XmlAttribute] public string DefName { get; set; } = string.Empty;
    [XmlAttribute] public int Degree { get; set; } = 0;
  }

  public class PawnHediff
  {
    [XmlAttribute] public string DefName { get; set; } = string.Empty;
    [XmlAttribute] public string BodyPartDef { get; set; } = string.Empty;


  }
}
