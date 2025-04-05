#nullable enable

using System;

namespace RimDialogue.Core
{
  [Serializable]
  public class PawnData
  {
    public string? Instructions = String.Empty;
    public string ThingID = String.Empty;
    public string FullName = String.Empty;
    public string NickName = String.Empty;
    public string RoyaltyTitle = string.Empty;
    public string Gender = String.Empty;
    public string FactionName = String.Empty;
    public string FactionLabel = String.Empty;
    public string FactionDescription = String.Empty;
    public string Description = String.Empty;
    public string Race = String.Empty;
    public int Age = -1;
    public string IdeologyName = string.Empty;
    public string IdeologyDescription = String.Empty;
    public string[] IdeologyPrecepts = [];
    public bool IsColonist = false;
    public bool IsPrisoner = false;
    public bool IsHostile = false;
    public bool IsCreepJoiner = false;
    public bool IsGhoul = false;
    public bool IsBloodFeeder = false;
    public bool IsSlave = false;
    public bool IsAnimal = false;
    public string Childhood = String.Empty;
    public string Adulthood = String.Empty;
    //public int OpinionOfRecipient = 0;
    public string MoodString = string.Empty;
    public string Personality = string.Empty;
    public string PersonalityDescription = string.Empty;
    public string JobReport = string.Empty;
    public string Carrying = string.Empty;
    public string[] Skills = [];
    public string[] Traits = [];
  }
}
