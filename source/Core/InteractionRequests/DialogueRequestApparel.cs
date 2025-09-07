#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestApparel : DialogueRequestTwoPawn<DialogueDataApparel>
  {
    const string Placeholder = "apparel";


    public DialogueRequestApparel(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
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

    public override async Task BuildData(DialogueDataApparel data)
    {
      data.ApparelLabel = Apparel?.def.label ?? string.Empty;
      data.ApparelDescription = H.RemoveWhiteSpace(Apparel?.def.description) ?? string.Empty;
      data.WornByCorpse = Apparel?.WornByCorpse ?? false;
      this.Apparel.TryGetQuality(out var quality);
      data.ApparelQuality = quality.GetLabel();
      await base.BuildData(data);
    }

    public override string? Action => "ApparelChitchat";

    public override Rule[] Rules => [new Rule_String(Placeholder, Apparel.LabelNoParenthesis)];
  }
}
