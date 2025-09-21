using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Linq;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestNeed : DialogueRequestTwoPawn<DialogueDataNeed>
  {
    const string Placeholder = "need";

    public Need Need { get; set; }

    //public static new DialogueRequestNeed<DialogueDataNeed> BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestNeed<DialogueDataNeed>(entry);
    //}

    public DialogueRequestNeed(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      var unsatisfiedNeeds = this.Initiator.needs
        .AllNeeds
          .Where(need => need != null && need.CurLevelPercentage < .333f);
      if (unsatisfiedNeeds.Any())
        Need = unsatisfiedNeeds.RandomElementByWeight(need => 1f - need.CurLevelPercentage);
      else
        Need = this.Initiator.needs.AllNeeds.MinBy(need => need.CurLevelPercentage);
    }

    public override string Action => "NeedsChitchat";

    public override async Task BuildData(DialogueDataNeed data)
    {
      await base.BuildData(data);
      data.NeedLabel = this.Need?.def.label ?? string.Empty;
      data.NeedDescription = this.Need?.def.description ?? string.Empty;
      data.NeedLevel = this.Need?.CurLevelPercentage ?? 0f;
    }

    public override Rule[] Rules => [new Rule_String(Placeholder, this.Need?.def.label ?? "RimDialogue.Sleep".Translate())];
  }
}

