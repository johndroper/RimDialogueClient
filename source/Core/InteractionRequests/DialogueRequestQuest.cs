#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Linq;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestQuest : DialogueRequestTwoPawn<DialogueDataQuest>
  {
    public DialogueRequestQuest(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    private Quest? _quest;
    public virtual Quest Quest
    {
      get
      {
        _quest ??= Find.QuestManager
          .QuestsListForReading
          .Where(quest => !quest.hidden && quest.State == QuestState.Ongoing)
          .RandomElement();
        return _quest;
      }
    }

    public override async Task BuildData(DialogueDataQuest data)
    {
      data.QuestName = Quest.name ?? string.Empty;
      data.QuestDescription = Quest.description;
      data.QuestState = Quest.State.ToString();
      data.AccepterPawn = Quest.AccepterPawn?.Name?.ToStringShort ?? Quest.AccepterPawn?.Label ?? string.Empty;
      data.QuestTags = Quest.tags?.ToArray() ?? [];
      data.Charity = Quest.charity;
      data.PeriodSinceAccepted = Quest.TicksSinceAccepted.ToStringTicksToPeriod();
#if !RW_1_5
      data.PeriodUntilExpiry = Quest.TicksUntilExpiry.ToStringTicksToPeriod();
#endif

      await base.BuildData(data);
    }

    public override string? Action => "QuestChitchat";

    public override Rule[] Rules => [
      new Rule_String("quest_name", Quest.name),
      new Rule_String("period_since_accepted", Quest.TicksSinceAccepted.ToStringTicksToPeriod()),
#if !RW_1_5
      new Rule_String("period_until_expiry", Quest.TicksUntilExpiry.ToStringTicksToPeriod()),
#endif
    ];

  }
}

