#nullable enable

using RimDialogue.Core.InteractionRequests;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestMessage : DialogueRequestTarget<DialogueDataMessage>
  {
    const string MessagePlaceholder = "**message**";

    static Dictionary<int, DialogueRequestMessage> recent = [];

    public static DialogueRequestMessage BuildFrom(LogEntry entry, string interactionTemplate)
    {
      if (recent.ContainsKey(entry.LogID))
        return recent[entry.LogID];
      var item = new DialogueRequestMessage(entry, interactionTemplate);
      recent.Add(entry.LogID, item);
      return item;
    }

    public Message Message { get; set; }
    protected Pawn? _target { get; set; }

    public override Pawn? Target => _target;

    public DialogueRequestMessage(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for message {entry.LogID} with template {interactionTemplate}.");

      Message = GameComponent_MessageTracker.Instance.TrackedMessages.RandomElement().Message;
      var tracker = H.GetTracker();
      _target = Message.lookTargets?.PrimaryTarget.Pawn;
    }

    public override string GetInteraction()
    {
      if (_target == null)
        return this.InteractionTemplate
          .Replace(MessagePlaceholder, Message.text.TrimEnd('.'));
      else
        return this.InteractionTemplate
          .Replace(MessagePlaceholder, _target.Name.ToStringShort + " is experiencing a " + Message.text.TrimEnd('.'));
    }

    public override void Build(DialogueDataMessage data)
    {
      data.QuestName = Message.quest?.name ?? string.Empty;
      data.QuestDescription = H.RemoveWhiteSpaceAndColor(Message.quest?.description);
      base.Build(data);
    }

    public override string? Action => null;

  }
}
