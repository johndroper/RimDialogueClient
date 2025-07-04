#nullable enable
using RimDialogue.Core.InteractionData;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public abstract class DialogueRequestTarget<DataT> : DialogueRequestTwoPawn<DataT> where DataT : DialogueTargetData, new()
  {

    public DialogueRequestTarget(PlayLogEntry_Interaction entry) : base(entry)
    {
    }

    public abstract Pawn? Target { get; }

    public override void BuildData(DataT data)
    {
      if (Target != null)
      {
        data.InitiatorOpinionOfTarget = Initiator.relations.OpinionOf(Target);
        data.RecipientOpinionOfTarget = Recipient.relations.OpinionOf(Target);
      }
      base.BuildData(data);
    }

    public override void Send(List<KeyValuePair<string, object?>> datae, string? action = null)
    {
      if (Target != null)
        datae.Add(new KeyValuePair<string, object?>("targetJson", Target.MakeData(H.GetTracker().GetInstructions(Target), Entry.LogID)));

      base.Send(datae, action);
    }

  }
}
