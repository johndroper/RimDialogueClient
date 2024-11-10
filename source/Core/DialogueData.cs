using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Bubbles.Core
{
  [Serializable]
  public class DialogueData
  {
    public string specialInstructions = String.Empty;
    public int maxWords = -1;
    public string interaction = String.Empty;
    public string scenario = string.Empty;
    public int daysPassedSinceSettle = -1;
    public string biome = String.Empty;
    public bool isOutside = false;
    public string room = String.Empty;
    public string currentWeather = String.Empty;
    public float outdoorTemp = -1f;
    public float wealthTotal = -1f;
    public string[] recentIncidents = [];
    public string initiatorFullName = String.Empty;
    public string initiatorNickName = String.Empty;
    public string initiatorRoyaltyTitle = string.Empty;
    public string initiatorGender = String.Empty; 
    public string initiatorFactionName = String.Empty;
    public string initiatorDescription = String.Empty;
    public string initiatorRace = String.Empty;
    public int initiatorAge = -1;
    public string initiatorBeard = string.Empty;
    public string initiatorHair = string.Empty;
    public string initiatorFaceTattoo = string.Empty;
    public string initiatorBodyTattoo = string.Empty;
    public string initiatorIdeology = String.Empty;
    public bool initiatorIsColonist = false;
    public bool initiatorIsPrisoner = false;
    public bool initiatorIsCreepJoiner = false;
    public bool initiatorIsGhoul = false;
    public bool initiatorIsBloodFeeder = false;
    public bool initiatorIsSlave = false;
    public bool initiatorIsAnimal = false;
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
    public string recipientNickName = String.Empty;
    public string recipientRoyaltyTitle = string.Empty;
    public string recipientGender = string.Empty;
    public string recipientFactionName = String.Empty;
    public string recipientDescription = String.Empty;
    public string recipientRace = String.Empty;
    public int recipientAge = -1;
    public string recipientHair = string.Empty;
    public string recipientFaceTattoo = string.Empty;
    public string recipientBodyTattoo = string.Empty;
    public string recipientBeard = string.Empty;
    public string recipientIdeology = String.Empty;
    public bool recipientIsColonist = false;
    public bool recipientIsPrisoner = false;
    public bool recipientIsCreepJoiner = false;
    public bool recipientIsGhoul = false;
    public bool recipientIsBloodfeeder = false;
    public bool recipientIsSlave = false;
    public bool recipientIsAnimal = false;
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
