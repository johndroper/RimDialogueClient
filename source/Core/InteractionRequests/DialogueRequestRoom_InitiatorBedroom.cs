#nullable enable
using System;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom_InitiatorBedroom : DialogueRequestRoom
  {
    public static new DialogueRequestRoom_InitiatorBedroom BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestRoom_InitiatorBedroom(entry, interactionTemplate);
    }

    public DialogueRequestRoom_InitiatorBedroom(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
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
