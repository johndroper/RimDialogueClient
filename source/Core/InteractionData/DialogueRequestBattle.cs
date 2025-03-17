using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestBattle<DataT> : DialogueRequest<DataT> where DataT : DialogueDataBattle, new()
  {
    const string BattlePlaceholder = "**recent_battle**";
    public string Interaction { get; set; }
    public string Duration { get; set; }

    public Battle Battle { get; set; }

    public static DialogueRequestBattle<DataT> BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestBattle<DataT>(entry, interactionTemplate);
    }

    public DialogueRequestBattle(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Mod.LogV($"Creating dialogue request for battle {entry.LogID} with template {interactionTemplate}.");
      Battle = H.GetRecentBattles(Settings.RecentBattleHours.Value).RandomElement();
      Duration = (Find.TickManager.TicksGame - Battle.CreationTimestamp).ToStringTicksToPeriod();
      Interaction = $"a battle named '{Battle.GetName()}' occurred {Duration} ago.";
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(BattlePlaceholder, Interaction);
    }

    public override void Build(DataT data)
    {
      data.Name = Battle.GetName();
      data.Entries = Battle.Entries.Select(entry => entry.ToGameStringFromPOV(this.Initiator)).ToArray();
      data.Duration = (Find.TickManager.TicksGame - Battle.CreationTimestamp).ToStringTicksToPeriod();
      data.Importance = Battle.Importance.ToString();
      data.Participants = Battle.Entries.SelectMany(entry => entry.GetConcerns().Select(thing => thing.Label)).ToArray();
      base.Build(data);
    }
    public override void Execute()
    {
      Mod.LogV($"Executing dialogue request for battle {Entry.LogID}.");
      InteractionWorker_DialogueBattle.lastUsedTicks = Find.TickManager.TicksAbs;
      var dialogueData = new DataT();
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ]);
    }
  }
}
