#nullable enable

using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestIncident : DialogueRequestTarget<DialogueDataIncident>
  {
    public override Pawn? Target
    {
      get
      {
        return _target;
      }
    }

    private Pawn? _target { get; set; }
    public string IncidentType { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public int Ticks { get; set; }
    public RecentLetter Incident { get; set; }

    public DialogueRequestIncident(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for incident {entry.LogID} with template {interactionTemplate}.");
      var incidents = GameComponent_LetterTracker.Instance.RecentLetters;
      if (!incidents.Any())
        throw new System.Exception("No recent incidents found for DialogueRequestIncident.");
      var now = Find.TickManager.TicksAbs;
      Incident = incidents.RandomElementByWeight(incident => (float)incident.Ticks / now);
      Explanation = Incident.Text ?? string.Empty;
      Ticks = Incident.Ticks;
      switch (Incident.Type)
      {
        case "ThreatBig":
          IncidentType = "RimDialogue.ThreatBig".Translate();
          break;
        case "ThreatSmall":
          IncidentType = "RimDialogue.ThreatSmall".Translate();
          break;
        case "PositiveEvent":
          IncidentType = "RimDialogue.PositiveEvent".Translate();
          break;
        case "NeutralEvent":
          IncidentType = "RimDialogue.NeutralEvent".Translate();
          break;
        default:
          IncidentType = "RimDialogue.DefaultEvent".Translate();
          break;
      }
    }

    public string Period
      {
      get
      {
        var now = Find.TickManager.TicksAbs;
        return (now - Ticks).ToStringTicksToPeriod();
      }
    }

    public override async Task BuildData(DialogueDataIncident data)
    {
      data.Explanation = Explanation;
      await base.BuildData(data);
    }

    public override Rule[] Rules => [
      new Rule_String("incident", Incident.Label),
      new Rule_String("incident_type", IncidentType),
      new Rule_String("period", Period)
    ];
  }
}

