using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestApparel<DataT> : DialogueRequest<DataT> where DataT : DialogueDataApparel, new()
  {
    const string Placeholder = "**apparel**";

    public Apparel Apparel { get; set; }

    public DialogueRequestApparel(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Apparel = this.Initiator.apparel?.WornApparel.RandomElement();
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
      dialogueData.ApparelLabel = Apparel.def?.label ?? string.Empty;
      dialogueData.ApparelDescription = H.RemoveWhiteSpace(Apparel.def?.description) ?? string.Empty;
      dialogueData.WornByCorpse = Apparel?.WornByCorpse ?? false;
      this.Apparel.TryGetQuality(out var quality);
      dialogueData.ApparelQuality = quality.GetLabel();

      Build(dialogueData);
      Send(
        new List<KeyValuePair<string, object?>>
        {
          new("chitChatJson", dialogueData)
        },
        "ApparelChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Apparel.LabelNoParenthesis);
    }
  }
}
