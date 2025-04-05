using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom_RecipientBedroom : DialogueRequestRoom
  {
    public static new DialogueRequestRoom_RecipientBedroom BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestRoom_RecipientBedroom(entry, interactionTemplate);
    }

    public DialogueRequestRoom_RecipientBedroom(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
    }

    private Room _bedroom;
    public override Room Room
    {
      get
      {
        _bedroom ??= this.Recipient.ownership?.OwnedRoom;
        return _bedroom;
      }
    }
  }
}

