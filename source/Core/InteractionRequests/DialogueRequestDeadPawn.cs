#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionRequests
{
  public abstract class DialogueRequestDeadPawn : DialogueRequestTarget<DialogueDataDeadPawn>
  {
    private static string agoText = "RimDialogue.Ago".Translate().ToString();

    public DialogueRequestDeadPawn(PlayLogEntry_Interaction entry) : base(entry)
    {
    }


    public abstract PawnDeathRecord Record
    {
      get;
    }

    public override void BuildData(DialogueDataDeadPawn data)
    {
      data.PawnName = Record.Pawn.Name.ToStringShort ?? Record.Pawn.LabelNoParenthesis ?? "Unknown";
      data.CauseOfDeath = Record.Cause;
      data.TimeSinceDeath = (Find.TickManager.TicksGame - Record.TimeStamp).ToStringTicksToPeriod() + agoText;
      base.BuildData(data);
    }

    public override Pawn Target
    {
      get
      {
        return Record.Pawn;
      }
    }

    public override string? Action => "DeadPawn";

    public override Rule[] Rules => [new Rule_String("pawn", Record.Pawn.Name?.ToStringShort ?? Record.Pawn.LabelCap ?? "Unknown")];

  }
}
