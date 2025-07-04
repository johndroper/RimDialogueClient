#nullable enable
using System;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom_RecipientBedroom : DialogueRequestRoom
  {
    public static new DialogueRequestRoom_RecipientBedroom BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestRoom_RecipientBedroom(entry);
    }

    public DialogueRequestRoom_RecipientBedroom(PlayLogEntry_Interaction entry) : base(entry)
    {
    }

    private Room? _bedroom;
    public override Room Room
    {
      get
      {
        _bedroom ??= this.Recipient.ownership?.OwnedRoom;
        return _bedroom ?? throw new Exception("Recipient does not have a bedroom");
      }
    }
  }
}

