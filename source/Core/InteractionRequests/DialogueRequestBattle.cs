#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestBattle : DialogueRequestTwoPawn<DialogueDataBattle>
  {
    const string BattlePlaceholder = "**battle**";

    public static float AgeDays(Battle battle)
    {
      return (Find.TickManager.TicksGame - battle.CreationTimestamp).TicksToDays();
    }

    public DialogueRequestBattle(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
    }

    public abstract Battle Battle
    {
      get;
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(BattlePlaceholder, Battle.GetName() ?? "an unnamed battle");
    }

    public override void BuildData(DialogueDataBattle data)
    {
      data.Name = Battle.GetName();
      data.Entries = Battle.Entries
        .OrderBy(entry => entry.Timestamp)
        .Select(entry => H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(this.Initiator))).ToArray();
      data.TimeSinceBattle = (Find.TickManager.TicksGame - Battle.CreationTimestamp).ToStringTicksToPeriod();
      data.Importance = Battle.Importance.ToString();
      data.Participants = Battle.Entries
        .SelectMany(entry => entry.GetConcerns()
          .Select(thing => H.RemoveWhiteSpaceAndColor(thing != null ? thing.ToString() : "RimDialogue.Unknown".Translate()) + thing?.Faction != null ? $" ({thing?.Faction?.Name ?? "RimDialogue.None".Translate()})" : string.Empty))
        .Distinct()
        .ToArray();
      data.Factions = Battle.Entries
        .SelectMany(entry => entry.GetConcerns().Select(thing => thing.Faction))
        .Distinct()
        .Where(faction => faction != null)
        .Select(faction => (faction.Name ?? "RimDialogue.Unknown".Translate()) + $" ({faction.def.label}) - " + faction.def.description)
        .ToArray();
      base.BuildData(data);
    }

    public override string Action => "Battle";
  }
}
