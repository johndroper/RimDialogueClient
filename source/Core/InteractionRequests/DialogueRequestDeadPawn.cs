#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionRequests
{
  public abstract class DialogueRequestDeadPawn : DialogueRequestTarget<DialogueDataDeadPawn>
  {
    private const string Placeholder = "**pawn**";
    private static string agoText = "RimDialogue.Ago".Translate().ToString();

    public DialogueRequestDeadPawn(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
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

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Record.Pawn.Name?.ToStringShort ?? Record.Pawn.LabelCap ?? "Unknown");
    }
  }
}
