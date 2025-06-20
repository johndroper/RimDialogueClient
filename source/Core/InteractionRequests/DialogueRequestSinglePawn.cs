using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestSinglePawn<DataT> : DialogueRequest<DataT> where DataT : InteractionData.DialogueData, new()
  {
    public static DialogueRequestSinglePawn<DataT> BuildFrom(PlayLogEntry_InteractionSinglePawn entry, string interactionTemplate)
    {
      return new DialogueRequestSinglePawn<DataT>(entry, interactionTemplate);
    }

    protected InteractionDef _interactionDef;
    protected string _instructions;
    protected Pawn _initiator;
    protected PawnData _initiatorData;

    public DialogueRequestSinglePawn(PlayLogEntry_InteractionSinglePawn entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      _interactionDef = (InteractionDef)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_InteractionDef.GetValue(entry);
      _initiator = (Pawn)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(entry);
      if (_initiator == null)
        throw new Exception($"Entry {entry.LogID} - Initiator is null.");
      else
        Mod.Log($"Entry {entry.LogID} - Initiator is {_initiator}.");
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - pawn data built.");
      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist)
        _instructions += "\r\n" + _tracker.GetInstructions(InstructionsSet.COLONISTS);
    }
    public override Pawn Initiator => _initiator;

    public override PawnData InitiatorData => _initiatorData;

    public override InteractionDef InteractionDef => _interactionDef;

    public override string Instructions => _instructions;
    public override void AddConversation(string interaction, string text)
    {
      _tracker.AddConversation(Initiator, null, interaction, text);
    }

    public override string Action => "DialogueSinglePawn";

  }
}
