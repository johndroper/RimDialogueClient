#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestIncident<DataT> : DialogueRequest<DataT> where DataT : DialogueDataIncident, new()
  {
    const string IncidentPlaceholder = "**recent_incident**";

    public static DialogueRequestIncident<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestIncident<DataT>(entry, interactionTemplate);
    }

    public Pawn? Target { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;

    public PawnData? TargetData { get; set; } = null;

    public DialogueRequestIncident(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Mod.LogV($"Creating dialogue request for incident {entry.LogID} with template {interactionTemplate}.");
      var incidents = Verse_LetterMaker_MakeLetter.recentLetters;
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
      var tracker = H.GetTracker();
      if (Target is not null)
        TargetData = H.MakePawnData(Target, tracker.GetInstructions(Target));
      else
        TargetData = null;
    }

    public override void Build(DataT data)
    {
      data.Explanation = Explanation;
      base.Build(data);
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(IncidentPlaceholder, Subject);
    }

    public override void Execute()
    {
      Mod.LogV($"Executing dialogue request for incident {Entry.LogID}.");
      InteractionWorker_DialogueIncident.lastUsedTicks = Find.TickManager.TicksAbs;
      var dialogueData = new DataT();
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData),
          new("targetJson", TargetData),
        ]);
    }
  }
}

