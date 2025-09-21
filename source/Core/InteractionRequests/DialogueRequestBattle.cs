#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Linq;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestBattle : DialogueRequestTwoPawn<DialogueDataBattle>
  {
    public static float AgeDays(Battle battle)
    {
      return (Find.TickManager.TicksAbs  - battle.CreationTimestamp).TicksToDays();
    }

    public DialogueRequestBattle(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(
        entry,
        interactionDef,
        initiator,
        recipient)
    {
    }

    public abstract Battle Battle
    {
      get;
    }

    public override Rule[] Rules => [new Rule_String("battle", Battle?.GetName() ?? "an unnamed battle")];

    public override async Task BuildData(DialogueDataBattle data)
    {
      if (Battle == null)
      {
        Log.Warning($"DialogueRequestBattle: Battle is null for entry.");
        return;
      }

      data.Name = Battle.GetName();
      data.Entries = Battle.Entries
        ?.OrderBy(entry => entry.Timestamp)
        .Take(25)
        .Select(entry => H.RemoveWhiteSpaceAndColor(entry.ToGameStringFromPOV(this.Initiator))).ToArray() ?? [];
      data.TimeSinceBattle = (Find.TickManager.TicksAbs - Battle.CreationTimestamp).ToStringTicksToPeriod();
      data.Importance = Battle.Importance.ToString();
      data.Participants = Battle.Entries
        ?.SelectMany(entry => entry.GetConcerns()
          .Select(thing => H.RemoveWhiteSpaceAndColor(thing != null ? thing.ToString() : "RimDialogue.Unknown".Translate()) + thing?.Faction != null ? $" ({thing?.Faction?.Name ?? "RimDialogue.None".Translate()})" : string.Empty))
        .Distinct()
        .ToArray() ?? [];
      data.Factions = Battle.Entries
        ?.SelectMany(entry => entry.GetConcerns().Select(thing => thing.Faction))
        .Distinct()
        .Where(faction => faction != null)
        .Select(faction => (faction.Name ?? "RimDialogue.Unknown".Translate()) + $" ({faction.def.label}) - " + faction.def.description)
        .ToArray() ?? [];
      await base.BuildData(data);
    }

    public override string Action => "Battle";
  }
}
