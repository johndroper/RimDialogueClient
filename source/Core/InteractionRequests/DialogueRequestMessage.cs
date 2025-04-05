#nullable enable

using RimDialogue.Access;
using RimDialogue.Core.InteractionWorkers;
using System.Collections.Generic;
using Verse;

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
    public Pawn? Target { get; set; }

    public DialogueRequestMessage(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for message {entry.LogID} with template {interactionTemplate}.");
      var messages = Verse_Messages_Message.RecentMessages;
      Message = messages.RandomElement();
      var tracker = H.GetTracker();
      Target = Message.lookTargets?.PrimaryTarget.Pawn;
      if (Target is not null)
        TargetData = H.MakePawnData(Target, tracker.GetInstructions(Target));
      else
        TargetData = null;
    }

    public override string GetInteraction()
    {
      if (Target == null)
        return this.InteractionTemplate
          .Replace(MessagePlaceholder, Message.text.TrimEnd('.'));
      else
        return this.InteractionTemplate
          .Replace(MessagePlaceholder, Target.Name.ToStringShort + " is experiencing a " + Message.text.TrimEnd('.'));
    }

    public override void Build(DialogueDataMessage data)
    {
      data.QuestName = Message.quest?.name ?? string.Empty;
      data.QuestDescription = H.RemoveWhiteSpaceAndColor(Message.quest?.description);
      base.Build(data);
    }

    public override void Execute()
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Executing dialogue request for message {Entry.LogID}.");
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
