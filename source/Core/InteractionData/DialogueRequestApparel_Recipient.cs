using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestApparel_Recipient : DialogueRequestApparel<DialogueDataApparel>
  {
    public static DialogueRequestApparel_Recipient BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestApparel_Recipient(entry, interactionTemplate);
    }

    public DialogueRequestApparel_Recipient(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Apparel = this.Recipient.apparel?.WornApparel.RandomElement();
    }
  }
}

