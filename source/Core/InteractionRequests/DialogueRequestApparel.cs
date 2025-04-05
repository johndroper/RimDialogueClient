#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestApparel : DialogueRequest<DialogueDataApparel>
  {
    const string Placeholder = "**apparel**";


    public DialogueRequestApparel(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private Apparel? _apparel;
    public virtual Apparel Apparel
    {
      get
      {
        _apparel ??= Initiator.apparel.WornApparel.RandomElement();
        return _apparel;
      }
    }


    public override void Execute()
    {
      var dialogueData = new DialogueDataApparel();
      dialogueData.ApparelLabel = Apparel.def?.label ?? string.Empty;
      dialogueData.ApparelDescription = H.RemoveWhiteSpace(Apparel.def?.description) ?? string.Empty;
      dialogueData.WornByCorpse = Apparel?.WornByCorpse ?? false;
      this.Apparel.TryGetQuality(out var quality);
      dialogueData.ApparelQuality = quality.GetLabel();

      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "ApparelChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Apparel.LabelNoParenthesis);
    }
  }
}
