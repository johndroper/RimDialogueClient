#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRoom : DialogueRequestTwoPawn<DialogueDataRoom>
  {
    public static new DialogueRequestRoom BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestRoom(entry);
    }


    public DialogueRequestRoom(PlayLogEntry_Interaction entry) : base(entry)
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

    public override Rule[] Rules => [new Rule_String("room", Room.GetRoomRoleLabel())];
  }
}
