using RimWorld;
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

    public override void Build(DialogueDataRoom data)
    {
      data.RoomLabel = Room.GetRoomRoleLabel();
      data.RoomCleanliness = RoomStatDefOf.Cleanliness.GetScoreStage(Room.GetStat(RoomStatDefOf.Cleanliness)).label;
      data.RoomImpressiveness = RoomStatDefOf.Impressiveness.GetScoreStage(Room.GetStat(RoomStatDefOf.Impressiveness)).label;
      data.RoomWealth = RoomStatDefOf.Wealth.GetScoreStage(Room.GetStat(RoomStatDefOf.Wealth)).label;
      data.RoomSpace = RoomStatDefOf.Space.GetScoreStage(Room.GetStat(RoomStatDefOf.Space)).label;
      data.RoomBeauty = RoomStatDefOf.Beauty.GetScoreStage(Room.GetStat(RoomStatDefOf.Beauty)).label;
      base.Build(data);
    }

    public override string Action => "RoomChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate
          .Replace("**room**", Room.GetRoomRoleLabel());
    }
  }
}
