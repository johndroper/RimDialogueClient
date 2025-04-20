using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestBattle : DialogueRequest<DialogueDataBattle>
  {
    const string BattlePlaceholder = "**battle**";

    public DialogueRequestBattle(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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

    public override void Build(DialogueDataBattle data)
    {
      data.Name = Battle.GetName();
      data.Entries = Battle.Entries
        .OrderBy(entry => entry.Timestamp)
        .Select(entry => H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(this.Initiator))).ToArray();
      data.TimeSinceBattle = (Find.TickManager.TicksGame - Battle.CreationTimestamp).ToStringTicksToPeriod();
      data.Importance = Battle.Importance.ToString();
      data.Participants = Battle.Entries
        .SelectMany(entry => entry.GetConcerns()
          .Select(thing => H.RemoveWhiteSpaceAndColor(thing is Pawn ? ((Pawn)thing).Name.ToStringShort : thing.Label) + $" ({thing.Faction.Name})"))
        .Distinct()
        .ToArray();
      data.Factions = Battle.Entries
        .SelectMany(entry => entry.GetConcerns().Select(thing => thing.Faction))
        .Distinct()
        .Where(faction => faction != null)
        .Select(faction => (faction.Name ?? "Unknown Name") + $" ({faction.def.label}) - " + faction.def.description)
        .ToArray();
      base.Build(data);
    }

    public override string Action => "Battle";
  }
}
