using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Bubbles.Core
{
  [Serializable]
  public class DialogueData
  {
    public string interaction = String.Empty;
    public string initiatorFullName = String.Empty;
    public string initiatorFactionName = String.Empty;
    public string initiatorDescription = String.Empty;
    public string initiatorRace = String.Empty;
    public int initiatorAge = -1;
    public bool initiatorIsColonist = false;
    public bool initiatorIsPrisoner = false;
    public SkillData[] initiatorSkills = [];
    public string[] initiatorTraits = [];
    public string initiatorChildhood = String.Empty;
    public string initiatorAdulthood = String.Empty;
    public RelationshipData[] initiatorRelations = [];
    public string[] initiatorApparel = [];
    public string[] initiatorWeapons = [];
    public string[] initiatorHediffs = [];
    public string[] initiatorMemories = [];
    public float[] initiatorOpinionOfRecipient = [];
    public float initiatorMoodPercentage = -1f;
    public float initiatorComfortPercentage = -1f;
    public float initiatorFoodPercentage = -1f;
    public float initiatorRestPercentage = -1f;
    public float initiatorJoyPercentage = -1f;
    public float initiatorBeautyPercentage = -1f;
    public float initiatorDrugsDesirePercentage = -1f;
    public float initiatorEnergyPercentage = -1f;
    public string recipientFullName = String.Empty;
    public string recipientFactionName = String.Empty;
    public string recipientDescription = String.Empty;
    public string recipientRace = String.Empty;
    public int recipientAge = -1;
    public bool recipientIsColonist = false;
    public bool recipientIsPrisoner = false;
    public SkillData[] recipientSkills = [];
    public string[] recipientTraits = [];
    public string recipientChildhood = String.Empty;
    public string recipientAdulthood = String.Empty;
    public RelationshipData[] recipientRelations = [];
    public string[] recipientApparel = [];
    public string[] recipientWeapons = [];
    public string[] recipientHediffs = [];
    public string[] recipientMemories = [];
    public float[] recipientOpinionOfInitiator = [];
    public float recipientMoodPercentage;
    public float recipientComfortPercentage;
    public float recipientFoodPercentage;
    public float recipientRestPercentage;
    public float recipientJoyPercentage;
    public float recipientBeautyPercentage;
    public float recipientDrugsDesirePercentage;
    public float recipientEnergyPercentage;
  }

  [Serializable]
  public class SkillData
  {
    public SkillData()
    {
    }

    public SkillData(string name, int level)
    {
      Name = name;
      Level = level;
    }

    public string Name;
    public int Level;
  }

  [Serializable]
  public class RelationshipData
  {
    public RelationshipData()
    {

    }

    public RelationshipData(string kind, string otherPawn)
    {
      Kind = kind;
      OtherPawn = otherPawn;
    }
    public string Kind;
    public string OtherPawn;
  }
}
