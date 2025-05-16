#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestBattleLogEntry : DialogueRequestSinglePawn<BattleLogData>
  {
    public DialogueRequestBattleLogEntry(
      PlayLogEntry_InteractionSinglePawn entry,
      string interactionTemplate,
      string battleLogEntry,
      Pawn? target) :
        base(entry, interactionTemplate)
    {
      Target = target;
      BattleLogEntry = battleLogEntry;
    }

    public Pawn? Target
    {
      get;
      private set;
    }

    public string BattleLogEntry
    {
      get;
      private set;
    }

    public override string? Action => "BattleLog";

    public override void BuildData(BattleLogData data)
    {
      base.BuildData(data);
      data.BattleLogEntry = BattleLogEntry;
      data.IdeologyName = Target?.Ideo?.name ?? string.Empty;
      data.IdeologyDescription = H.RemoveWhiteSpaceAndColor(Target?.Ideo?.description);
      data.TargetDescription = H.RemoveWhiteSpace(Target?.DescriptionDetailed).TrimEnd('.').ToLower();
      data.TargetRace = Target?.def?.defName ?? String.Empty;
      data.TargetKind = Target?.KindLabel ?? string.Empty;
      data.TargetWeapon = Target?.equipment?.Primary?.def?.label ?? string.Empty;
      data.TargetApparel = Target?.apparel?.WornApparel
        .Select(apparel => apparel.def.label)
        .ToArray() ?? Array.Empty<string>();
      data.FactionName = Target?.Faction.Name;
      data.FactionLeader = Target?.Faction.leader?.Name.ToStringFull ?? string.Empty;
      data.FactionLeaderTitle = Target?.Faction.LeaderTitle;
      data.FactionLabel = Target?.Faction.def?.label ?? string.Empty;
      data.FactionDescription = Target?.Faction.def?.description ?? string.Empty;
      data.FactionTechLevel = Target?.Faction.def?.techLevel.ToString() ?? string.Empty;
    }
  }
}
