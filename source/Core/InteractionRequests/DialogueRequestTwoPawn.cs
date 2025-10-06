#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestTwoPawn<DataT> : DialogueRequest<DataT> where DataT : InteractionData.DialogueData, new()
  {
    //public static DialogueRequestTwoPawn<DataT> BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestTwoPawn<DataT>(entry);
    //}

    protected InteractionDef _interactionDef;
    protected string _instructions;
    protected Pawn _initiator;
    protected PawnData _initiatorData;

    public Pawn Recipient { get; set; }
    public PawnData recipientData;

    public int initiatorOpinionOfRecipient;
    public int recipientOpinionOfInitiator;

    public override bool KnownType { get; }

    public DialogueRequestTwoPawn(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient,
      bool knownType = true) : base(entry)
    {
      KnownType = knownType;
      _interactionDef = interactionDef;
      _initiator = initiator;
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);
      Recipient = recipient;
      recipientData = Recipient.MakeData(_tracker.GetInstructions(Recipient), entry.LogID);

      initiatorOpinionOfRecipient = Initiator.relations.OpinionOf(Recipient);
      recipientOpinionOfInitiator = Recipient.relations.OpinionOf(Initiator);

      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + Environment.NewLine + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist || Recipient.IsColonist)
        _instructions += Environment.NewLine + _tracker.GetInstructions(InstructionsSet.COLONISTS);
      KnownType = knownType;
    }

    public override Pawn Initiator => _initiator;
    public override PawnData InitiatorData => _initiatorData;
    public override InteractionDef InteractionDef => _interactionDef;
    public override string Instructions => _instructions;

    public async override Task BuildData(DataT data)
    {
      await base.BuildData(data);
      data.InitiatorOpinionOfRecipient = initiatorOpinionOfRecipient;
      data.RecipientOpinionOfInitiator = recipientOpinionOfInitiator;

      if (this.Initiator.IsColonist)
      {
        var now = Find.TickManager.TicksAbs;
#if !RW_1_5
        if (GameComponent_ContextTracker.Instance != null)
        {
          var basicContexts = await GameComponent_ContextTracker.Instance
            .GetContext(Interaction, 10);
          data.Context = basicContexts
            .Select(context => context.Text)
            .ToArray();

          var temporalContexts = await GameComponent_ContextTracker.Instance
            .GetTemporalContext(Initiator, Recipient, Interaction, 5);
          data.TemporalContext = temporalContexts
              .Where(context => context != null)
              .OrderBy(context => context.Tick)
              .Select(context => $"{(now - context.Tick).ToStringTicksToPeriod()} ago - {context.Text}")
              .ToArray();
        }
#endif
      }
    }

    public override void BuildForm(WWWForm form)
    {
      base.BuildForm(form);
      form.AddField("recipientJson", JsonUtility.ToJson(recipientData));
    }
    public override void AddConversation(string interaction, string text)
    {
      _tracker.AddConversation(Initiator, Recipient, interaction, text);
    }
    public override Rule[] Rules => [];
    public override IDictionary<string, string> Constants =>
      new Dictionary<string, string>();
  }
}
