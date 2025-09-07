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
using Verse.Grammar;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestMany<DataT> : DialogueRequest<DataT> where DataT : InteractionData.DialogueData, new()
  {
    //public static DialogueRequestMany<DataT> BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestMany<DataT>(entry);
    //}

    protected InteractionDef _interactionDef;
    protected string _instructions;
    protected Pawn _initiator;
    protected PawnData _initiatorData;

    public DialogueRequestMany(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef
      ) : base(entry)
    {
      _interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_Interaction_InteractionDef.GetValue(entry);
      _initiator = (Pawn)Reflection.Verse_PlayLogEntry_Interaction_Initiator.GetValue(entry);
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);

      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist)
        _instructions += "\r\n" + _tracker.GetInstructions(InstructionsSet.COLONISTS);
    }

    public override Pawn Initiator => _initiator;
    public override PawnData InitiatorData => _initiatorData;
    public override InteractionDef InteractionDef => _interactionDef;
    public override string Instructions => _instructions;
    public override Rule[] Rules => [];

    public override async Task BuildData(DataT data)
    {
      await base.BuildData(data);
    }

    public override void AddConversation(string interaction, string text)
    {
      _tracker.AddConversation(Initiator, null, interaction, text);
    }

    public override string Action => "DialogueSinglePawn";
  }
}
