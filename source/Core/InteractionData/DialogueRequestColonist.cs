using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonist<DataT> : DialogueRequest<DataT> where DataT : DialogueTargetData, new()
  {
    const string colonistPlaceholder = "**colonist**";

    public static DialogueRequestColonist<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestColonist<DataT>(entry, interactionTemplate);
    }

    Pawn Target { get; set; }

    public DialogueRequestColonist(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      var colonists = Reflection.RimWorld_ColonistBar_TmpColonistsInOrder.GetValue(Find.ColonistBar) as List<Pawn>;
      if (colonists != null && colonists.Any())
        Target = colonists
          .Where(colonist => colonist != this.Initiator && colonist != this.Recipient)
          .RandomElement();
      else
        Target = this.Initiator;
    }

    public override void Execute()
    {
      var dialogueData = new DataT();
      var tracker = H.GetTracker();
      Build(dialogueData);
      Send(
        new List<KeyValuePair<string, object?>>
        {
          new("chitChatJson", dialogueData),
          new("targetJson", H.MakePawnData(Target, tracker.GetInstructions(Target)))
        },
        "ColonistChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(colonistPlaceholder, this.Target.Name.ToStringShort);
    }
  }
}
