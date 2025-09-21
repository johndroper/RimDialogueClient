#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ActiveQuestChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
#if RW_1_5
        var hasQuests = Find.QuestManager
          .QuestsListForReading
          .Where(quest => !quest.hidden && quest.State == QuestState.Ongoing)
          .Any();
#else
        var hasQuests = Find.QuestManager
          .ActiveQuestsListForReading
          .Where(quest => !quest.hidden && quest.State == QuestState.Ongoing)
          .Any();
#endif

        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !hasQuests)
        {
          return 0f;
        }
        return Settings.QuestWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_ActiveQuestChitchat: {ex}");
        return 0f;
      }
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestQuest(entry, intDef, initiator, recipient);
    }
  }
}
