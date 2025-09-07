#nullable enable
using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

    public DialogueRequestTwoPawn(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry)
    {
      _interactionDef = interactionDef;
      _initiator = initiator;
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);
      Recipient = recipient;
      recipientData = Recipient.MakeData(_tracker.GetInstructions(Recipient), entry.LogID);

      initiatorOpinionOfRecipient = Initiator.relations.OpinionOf(Recipient);
      recipientOpinionOfInitiator = Recipient.relations.OpinionOf(Initiator);

      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist || Recipient.IsColonist)
        _instructions += "\r\n" + _tracker.GetInstructions(InstructionsSet.COLONISTS);
    }

    //public DialogueRequestTwoPawn(PlayLogEntry_Interaction entry) : this(
    //  entry,
    //  (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(entry),
    //  (Pawn)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(entry),
    //  (Pawn)Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(entry))
    //{

    //}

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
        var contexts = await GameComponent_ContextTracker.Instance
          .BlendedSearch(Initiator, Recipient, Interaction, 5);
        data.Context = contexts
            .Select(context => context != null ? $"{(now - context.Tick).ToStringTicksToPeriod()} ago - {context.Text}" : string.Empty)
            .ToArray();
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
  }
}
