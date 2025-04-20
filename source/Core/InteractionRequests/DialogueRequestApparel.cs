#nullable enable
using RimWorld;
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

    public override void Build(DialogueDataApparel data)
    {
      data.ApparelLabel = Apparel.def?.label ?? string.Empty;
      data.ApparelDescription = H.RemoveWhiteSpace(Apparel.def?.description) ?? string.Empty;
      data.WornByCorpse = Apparel?.WornByCorpse ?? false;
      this.Apparel.TryGetQuality(out var quality);
      data.ApparelQuality = quality.GetLabel();
      base.Build(data);
    }

    public override string? Action => "ApparelChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Apparel.LabelNoParenthesis);
    }
  }
}
