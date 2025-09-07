#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestFaction : DialogueRequestTwoPawn<DialogueDataFaction>
  {
    public DialogueRequestFaction(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    public abstract Faction Faction
    {
      get;
    }

    public override string? Action => "FactionChitchat";

    public override async Task BuildData(DialogueDataFaction data)
    {
      await base.BuildData(data);
      data.FactionName = Faction.Name;
      data.FactionLeader = Faction.leader?.Name.ToStringFull ?? string.Empty;
      data.LeaderTitle = Faction.LeaderTitle;
      data.GoodWill = Faction.GoodwillWith(Faction.OfPlayer);
      data.PlayerRelationKind = Faction.PlayerRelationKind.ToString();
      data.Label = Faction.def?.label ?? string.Empty;
      data.Description = Faction.def?.description ?? string.Empty;
      data.TechLevel = Faction.def?.techLevel.ToString() ?? string.Empty;
    }

    public override Rule[] Rules => [
      new Rule_String("faction", Faction?.Name ?? "a nearby faction"),
      new Rule_String("faction_leader", Faction?.leader?.Name.ToStringFull ?? string.Empty),
      new Rule_String("faction_leader_title", Faction?.LeaderTitle ?? string.Empty),
      new Rule_String("faction_goodwill", Faction?.GoodwillWith(Faction.OfPlayer).ToString() ?? "0"),
      new Rule_String("faction_relation_kind", Faction?.PlayerRelationKind.ToString() ?? "None"),
      new Rule_String("faction_label", Faction?.def?.label ?? string.Empty),
      new Rule_String("faction_description", Faction?.def?.description ?? string.Empty),
      new Rule_String("faction_tech_level", Faction?.def?.techLevel.ToString() ?? string.Empty)
    ];
  }
}
