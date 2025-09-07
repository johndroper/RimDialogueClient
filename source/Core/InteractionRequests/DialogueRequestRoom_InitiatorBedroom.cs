#nullable enable
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom_InitiatorBedroom : DialogueRequestRoom
  {
    //public static new DialogueRequestRoom_InitiatorBedroom BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestRoom_InitiatorBedroom(entry);
    //}

    public DialogueRequestRoom_InitiatorBedroom(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
    }

    private Room? _bedroom;
    public override Room Room
    {
      get
      {
        _bedroom ??= this.Initiator.ownership?.OwnedRoom;
        return _bedroom ?? throw new Exception("Initiator does not have a bedroom");
      }
    }

  }
}
