#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestMessage : DialogueRequest<DialogueDataMessage>
  {
    const string MessagePlaceholder = "**message**";

    static Dictionary<int, DialogueRequestMessage> recent = [];

    public PawnData? TargetData { get; set; } = null;

    public static DialogueRequestMessage BuildFrom(LogEntry entry, string interactionTemplate)
    {
      if (recent.ContainsKey(entry.LogID))
        return recent[entry.LogID];
      var item = new DialogueRequestMessage(entry, interactionTemplate);
      recent.Add(entry.LogID, item);
      return item;
    }

    public Message Message { get; set; }

    public DialogueRequestMessage(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Mod.LogV($"Creating dialogue request for message {entry.LogID} with template {interactionTemplate}.");
      var messages = Verse_Messages_Message.RecentMessages;
      Message = messages.RandomElement();
      var tracker = H.GetTracker();
      var target = Message.lookTargets?.PrimaryTarget.Pawn;
      if (target is not null)
        TargetData = H.MakePawnData(target, tracker.GetInstructions(target));
      else
        TargetData = null;
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(MessagePlaceholder, Message.text.TrimEnd('.'));
    }

    public override void Build(DialogueDataMessage data)
    {
      data.QuestName = Message.quest?.name ?? string.Empty;
      data.QuestDescription = H.RemoveWhiteSpaceAndColor(Message.quest?.description);
      base.Build(data);
    }

    public override void Execute()
    {
      Mod.LogV($"Executing dialogue request for message {Entry.LogID}.");
      InteractionWorker_DialogueMessage.lastUsedTicks = Find.TickManager.TicksAbs;
      var dialogueData = new DialogueDataMessage();
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData),
          new("targetJson", TargetData)
        ]);
    }
  }
}
