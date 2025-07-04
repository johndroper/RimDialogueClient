#nullable enable

using RimDialogue.Core.InteractionRequests;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestIncident<DataT> : DialogueRequestTarget<DataT> where DataT : DialogueDataIncident, new()
  {
    const string IncidentPlaceholder = "recent_incident";

    public static new DialogueRequestIncident<DataT> BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestIncident<DataT>(entry);
    }

    public override Pawn? Target
    {
      get
      {
        return _target;
      }
    }

    private Pawn? _target { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;

    public DialogueRequestIncident(PlayLogEntry_Interaction entry) : base(entry)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for incident {entry.LogID} with template {interactionTemplate}.");
      var incidents = GameComponent_LetterTracker.Instance.RecentLetters;
      if (!incidents.Any())
      {
        Mod.Warning("No recent incidents found.");
        return;
      }
      var incident = incidents.RandomElement();
      Explanation = incident.Text;
      switch (incident.Def.defName)
      {
        case "ThreatBig":
          Subject = $"a major threat has that just occurred, '{incident.Label.ToLower()}'";
          break;
        case "ThreatSmall":
          Subject = $"a minor threat has that just occurred, '{incident.Label.ToLower()}'";
          break;
        case "PositiveEvent":
          Subject = $"something positive that has just happened, '{incident.Label.ToLower()}'";
          break;
        case "NeutralEvent":
          Subject = $"something that just occurred, '{incident.Label.ToLower()}'";
          break;
        default:
          Subject = $"{incident.Label} that has just occurred";
          break;
      }
    }

    public override void BuildData(DataT data)
    {
      data.Explanation = Explanation;
      base.BuildData(data);
    }

    public override Rule[] Rules => [
      new Rule_String(IncidentPlaceholder, Subject)
    ];
  }
}

