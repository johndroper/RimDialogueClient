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
    public string scenario = string.Empty;
    public string initiatorFullName = String.Empty;
    public string initiatorFactionName = String.Empty;
    public string initiatorDescription = String.Empty;
    public string initiatorRace = String.Empty;
    public int initiatorAge = -1;
    public string initiatorIdeology = String.Empty;
    public bool initiatorIsColonist = false;
    public bool initiatorIsPrisoner = false;
    public string[] initiatorSkills = [];
    public string[] initiatorTraits = [];
    public string initiatorChildhood = String.Empty;
    public string initiatorAdulthood = String.Empty;
    public string[] initiatorRelations = [];
    public string[] initiatorApparel = [];
    public string[] initiatorWeapons = [];
    public string[] initiatorHediffs = [];
    public string[] initiatorOpinionOfRecipient = [];
    public string[] initiatorMoodThoughts = [];
    public string initiatorMoodString = string.Empty;
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
    public string recipientIdeology = String.Empty;
    public bool recipientIsColonist = false;
    public bool recipientIsPrisoner = false;
    public string[] recipientSkills = [];
    public string[] recipientTraits = [];
    public string recipientChildhood = String.Empty;
    public string recipientAdulthood = String.Empty;
    public string[] recipientRelations = [];
    public string[] recipientApparel = [];
    public string[] recipientWeapons = [];
    public string[] recipientHediffs = [];
    public string[] recipientOpinionOfInitiator = [];
    public string[] recipientMoodThoughts = [];
    public string recipientMoodString = string.Empty;
    public float recipientMoodPercentage;
    public float recipientComfortPercentage;
    public float recipientFoodPercentage;
    public float recipientRestPercentage;
    public float recipientJoyPercentage;
    public float recipientBeautyPercentage;
    public float recipientDrugsDesirePercentage;
    public float recipientEnergyPercentage;
  }


}
