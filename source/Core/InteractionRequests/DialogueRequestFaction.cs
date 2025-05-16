#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestFaction : DialogueRequestTwoPawn<DialogueDataFaction>
  {
    const string FactionPlaceholder = "**faction**";


    public DialogueRequestFaction(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    public abstract Faction Faction
    {
      get;
    }

    public override string? Action => "FactionChitchat";

    public override void BuildData(DialogueDataFaction data)
    {
      base.BuildData(data);
      data.FactionName = Faction.Name;
      data.FactionLeader = Faction.leader?.Name.ToStringFull ?? string.Empty;
      data.LeaderTitle = Faction.LeaderTitle;
      data.GoodWill = Faction.GoodwillWith(Faction.OfPlayer);
      data.PlayerRelationKind = Faction.PlayerRelationKind.ToString();
      data.Label = Faction.def?.label ?? string.Empty;
      data.Description = Faction.def?.description ?? string.Empty;
      data.TechLevel = Faction.def?.techLevel.ToString() ?? string.Empty;
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(FactionPlaceholder, Faction?.Name ?? "a nearby faction");
    }
  }
}
