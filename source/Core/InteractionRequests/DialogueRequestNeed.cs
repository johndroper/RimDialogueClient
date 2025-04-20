using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestNeed<DataT> : DialogueRequest<DataT> where DataT : DialogueDataNeed, new()
  {
    const string Placeholder = "**need**";

    public Need Need { get; set; }

    public static DialogueRequestNeed<DialogueDataNeed> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestNeed<DialogueDataNeed>(entry, interactionTemplate);
    }

    public DialogueRequestNeed(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      var unsatisfiedNeeds = this.Initiator.needs
        .AllNeeds
          .Where(need => need.CurLevelPercentage < .333f);
      Need = unsatisfiedNeeds.RandomElement();
    }

    public override string Action => "NeedChitchat";

    public override void Build(DataT data)
    {
      base.Build(data);
      data.NeedLabel = this.Need?.def.label ?? string.Empty;
      data.NeedDescription = this.Need?.def.description ?? string.Empty;
      data.NeedLevel = this.Need?.CurLevelPercentage ?? 0f;
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, this.Need?.def.label);
    }
  }
}

