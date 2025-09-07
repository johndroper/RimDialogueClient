#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public abstract class DialogueRequestTarget<DataT> : DialogueRequestTwoPawn<DataT> where DataT : DialogueTargetData, new()
  {

    public DialogueRequestTarget(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
    }

    public abstract Pawn? Target { get; }

    public override async Task BuildData(DataT data)
    {
      if (Target != null)
      {
        data.InitiatorOpinionOfTarget = Initiator.relations.OpinionOf(Target);
        data.RecipientOpinionOfTarget = Recipient.relations.OpinionOf(Target);
      }
      await base.BuildData(data);
    }

    public override async Task Send(List<KeyValuePair<string, object?>> datae, string? action = null)
    {
      if (Target != null)
        datae.Add(new KeyValuePair<string, object?>("targetJson", Target.MakeData(H.GetTracker().GetInstructions(Target), Entry.LogID)));

      await base.Send(datae, action);
    }

  }
}
