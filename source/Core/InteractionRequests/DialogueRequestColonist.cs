using RimDialogue.Access;
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestColonist : DialogueRequestTarget<DialogueTargetData>
  {
    const string colonistPlaceholder = "colonist";

    private Pawn _target;
    public override Pawn Target
    {
      get
      {
        return _target;
      }
    }

    public DialogueRequestColonist(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
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

    public override Rule[] Rules => [new Rule_String(colonistPlaceholder, this.Target.Name.ToStringShort)];

  }
}
