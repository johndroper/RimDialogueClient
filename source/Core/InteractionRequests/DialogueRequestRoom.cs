using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom : DialogueRequest<DialogueDataRoom>
  {
    public static DialogueRequestRoom BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestRoom(entry, interactionTemplate);
    }


    public DialogueRequestRoom(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Room _room;
    public virtual Room Room
    {
      get
      {
        _room ??= this.Initiator.GetRoom();
        return _room;
      }
    }

    public override void Execute()
    {
      var dialogueData = new DialogueDataRoom
      {
        RoomLabel = Room.GetRoomRoleLabel(),
        RoomCleanliness = RoomStatDefOf.Cleanliness.GetScoreStage(Room.GetStat(RoomStatDefOf.Cleanliness)).label,
        RoomImpressiveness = RoomStatDefOf.Impressiveness.GetScoreStage(Room.GetStat(RoomStatDefOf.Impressiveness)).label,
        RoomWealth = RoomStatDefOf.Wealth.GetScoreStage(Room.GetStat(RoomStatDefOf.Wealth)).label,
        RoomSpace = RoomStatDefOf.Space.GetScoreStage(Room.GetStat(RoomStatDefOf.Space)).label,
        RoomBeauty = RoomStatDefOf.Beauty.GetScoreStage(Room.GetStat(RoomStatDefOf.Beauty)).label,
      };
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "RoomChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
          .Replace("**room**", Room.GetRoomRoleLabel());
    }
  }
}
