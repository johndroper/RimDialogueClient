#nullable enable
using RimDialogue.Core.InteractionData;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public abstract class DialogueRequestTarget<DataT> : DialogueRequest<DataT> where DataT : DialogueTargetData, new()
  {

    public DialogueRequestTarget(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
    }

    public abstract Pawn? Target { get; }

    public override void Build(DataT data)
    {
      if (Target != null)
      {
        data.InitiatorOpinionOfTarget = Initiator.relations.OpinionOf(Target);
        data.RecipientOpinionOfTarget = Recipient.relations.OpinionOf(Target);
      }
      base.Build(data);
    }

    public override void Send(List<KeyValuePair<string, object?>> datae, string? action = null)
    {
      if (Target != null)
        datae.Add(new KeyValuePair<string, object?>("targetJson", H.MakePawnData(Target, H.GetTracker().GetInstructions(Target))));

      base.Send(datae, action);
    }

  }
}
