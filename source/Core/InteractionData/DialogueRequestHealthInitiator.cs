using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestHealthInitiator : DialogueRequestHealth<DialogueDataHealth>
  {
    public static DialogueRequestHealthInitiator BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestHealthInitiator(entry, interactionTemplate);
    }

    public DialogueRequestHealthInitiator(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Hediff = this.Initiator.health.hediffSet.hediffs.RandomElement();
    }
  }
}
