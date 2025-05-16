#nullable enable
using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestTwoPawn<DataT> : DialogueRequest<DataT> where DataT : InteractionData.DialogueData, new()
  {
    public static DialogueRequestTwoPawn<DataT> BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestTwoPawn<DataT>(entry, interactionTemplate);
    }

    protected InteractionDef _interactionDef;
    protected string _instructions;
    protected Pawn _initiator;
    protected PawnData _initiatorData;

    public Pawn Recipient { get; set; }
    public PawnData recipientData;

    public int initiatorOpinionOfRecipient;
    public int recipientOpinionOfInitiator;

    public DialogueRequestTwoPawn(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      _interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(entry);
      Recipient = (Pawn)Reflection.Verse_PlayLogEntry_Interaction_Recipient.GetValue(entry);
      _initiator = (Pawn)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(entry);
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);
      recipientData = Recipient.MakeData(_tracker.GetInstructions(Recipient), entry.LogID);

      initiatorOpinionOfRecipient = Initiator.relations.OpinionOf(Recipient);
      recipientOpinionOfInitiator = Recipient.relations.OpinionOf(Initiator);

      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist || Recipient.IsColonist)
        _instructions += "\r\n" + _tracker.GetInstructions(InstructionsSet.COLONISTS);
    }

    public override Pawn Initiator => _initiator;
    public override PawnData InitiatorData => _initiatorData;
    public override InteractionDef InteractionDef => _interactionDef;
    public override string Instructions => _instructions;

    public override void BuildData(DataT data)
    {
      base.BuildData(data);
      data.InitiatorOpinionOfRecipient = initiatorOpinionOfRecipient;
      data.RecipientOpinionOfInitiator = recipientOpinionOfInitiator;
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
  }
}
