using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestNeed<DataT> : DialogueRequestTwoPawn<DataT> where DataT : DialogueDataNeed, new()
  {
    const string Placeholder = "need";

    public Need Need { get; set; }

    public static new DialogueRequestNeed<DialogueDataNeed> BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestNeed<DialogueDataNeed>(entry);
    }

    public DialogueRequestNeed(PlayLogEntry_Interaction entry) : base(entry)
    {
      var unsatisfiedNeeds = this.Initiator.needs
        .AllNeeds
          .Where(need => need.CurLevelPercentage < .333f);
      Need = unsatisfiedNeeds.RandomElement();
    }

    public override string Action => "NeedsChitchat";

    public override void BuildData(DataT data)
    {
      base.BuildData(data);
      data.NeedLabel = this.Need?.def.label ?? string.Empty;
      data.NeedDescription = this.Need?.def.description ?? string.Empty;
      data.NeedLevel = this.Need?.CurLevelPercentage ?? 0f;
    }

    public override Rule[] Rules => [new Rule_String(Placeholder, this.Need?.def.label ?? string.Empty)];
  }
}

