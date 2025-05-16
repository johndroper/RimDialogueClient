#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom : DialogueRequestTwoPawn<DialogueDataRoom>
  {
    public static new DialogueRequestRoom BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestRoom(entry, interactionTemplate);
    }


    public DialogueRequestRoom(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Room? _room;
    public virtual Room Room
    {
      get
      {
        _room ??= this.Initiator.GetRoom();
        return _room;
      }
    }

    public override void BuildData(DialogueDataRoom data)
    {
      data.RoomLabel = Room.GetRoomRoleLabel();
      data.RoomCleanliness = RoomStatDefOf.Cleanliness.GetScoreStage(Room.GetStat(RoomStatDefOf.Cleanliness)).label;
      data.RoomImpressiveness = RoomStatDefOf.Impressiveness.GetScoreStage(Room.GetStat(RoomStatDefOf.Impressiveness)).label;
      data.RoomWealth = RoomStatDefOf.Wealth.GetScoreStage(Room.GetStat(RoomStatDefOf.Wealth)).label;
      data.RoomSpace = RoomStatDefOf.Space.GetScoreStage(Room.GetStat(RoomStatDefOf.Space)).label;
      data.RoomBeauty = RoomStatDefOf.Beauty.GetScoreStage(Room.GetStat(RoomStatDefOf.Beauty)).label;
      base.BuildData(data);
    }

    public override string Action => "RoomChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate
          .Replace("**room**", Room.GetRoomRoleLabel());
    }
  }
}
