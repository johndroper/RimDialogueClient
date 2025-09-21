using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core.InteractionData
{
  public class BattleLogData : DialogueData
  {
    public string BattleLogEntry = string.Empty;
    public string TargetDescription = String.Empty;
    public string TargetKind = String.Empty;
    public string TargetRace = String.Empty;
    public string TargetFaction = String.Empty;
    public string TargetWeapon = String.Empty;
    public string[] TargetApparel = [];
    public string IdeologyName = string.Empty;
    public string IdeologyDescription = string.Empty;
    public string FactionName = string.Empty;
    public string FactionLeader = string.Empty;
    public string FactionLeaderTitle = string.Empty;
    public string FactionTechLevel = string.Empty;
    public string FactionLabel = string.Empty;
    public string FactionDescription = string.Empty;
  }
}
