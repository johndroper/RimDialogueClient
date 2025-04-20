using RimDialogue.Access;
using RimDialogue.Core.InteractionRequests;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonist<DataT> : DialogueRequestTarget<DataT> where DataT : DialogueTargetData, new()
  {
    const string colonistPlaceholder = "**colonist**";

    public static DialogueRequestColonist<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestColonist<DataT>(entry, interactionTemplate);
    }

    private Pawn _target;
    public override Pawn Target
    {
      get
      {
        return _target;
      }
    }

    public DialogueRequestColonist(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      var colonists = Reflection.RimWorld_ColonistBar_TmpColonistsInOrder.GetValue(Find.ColonistBar) as List<Pawn>;
      if (colonists != null && colonists.Any())
        _target = colonists
          .Where(colonist => colonist != this.Initiator && colonist != this.Recipient)
          .RandomElement();
      else
        _target = this.Initiator;
    }

    public override string Action => "ColonistChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(colonistPlaceholder, this.Target.Name.ToStringShort);
    }
  }
}
