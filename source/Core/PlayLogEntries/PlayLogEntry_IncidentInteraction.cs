//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Verse;

//namespace RimDialogue.Core.PlayLogEntries
//{
//  public class PlayLogEntry_IncidentInteraction : PlayLogEntry_DialogueInteraction
//  {
//    public PlayLogEntry_IncidentInteraction()
//    {
//    }

//    public PlayLogEntry_IncidentInteraction(
//      InteractionDef intDef,
//      Pawn initiator,
//      Pawn recipient,
//      List<RulePackDef> extraSentencePacks) : base(
//        intDef,
//        initiator,
//        recipient,
//        extraSentencePacks)
//    {
//      var incidents = GameComponent_LetterTracker.Instance.RecentLetters;
//      if (!incidents.Any())
//        throw new System.Exception("No recent incidents found for DialogueRequestIncident.");
//      var now = Find.TickManager.TicksAbs;
//      Incident = incidents.RandomElementByWeight(incident => (float)incident.Ticks / now);
//      Explanation = Incident.Text ?? string.Empty;
//      Ticks = Incident.Ticks;
//      switch (Incident.Type)
//      {
//        case "ThreatBig":
//          IncidentType = "RimDialogue.ThreatBig".Translate();
//          break;
//        case "ThreatSmall":
//          IncidentType = "RimDialogue.ThreatSmall".Translate();
//          break;
//        case "PositiveEvent":
//          IncidentType = "RimDialogue.PositiveEvent".Translate();
//          break;
//        case "NeutralEvent":
//          IncidentType = "RimDialogue.NeutralEvent".Translate();
//          break;
//        default:
//          IncidentType = "RimDialogue.DefaultEvent".Translate();
//          break;
//      }
//    }
//    public string IncidentType { get; set; } = string.Empty;
//    public string Explanation { get; set; } = string.Empty;
//    public int Ticks { get; set; }
//    public RecentLetter Incident { get; set; }
//  }
//}
