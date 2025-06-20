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

    public static new DialogueRequestMessage BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
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

    public DialogueRequestMessage(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for message {entry.LogID} with template {interactionTemplate}.");

      Message = GameComponent_MessageTracker.Instance.TrackedMessages.RandomElement().Message;
      var tracker = H.GetTracker();
      _target = Message.lookTargets?.PrimaryTarget.Pawn;

     //Message.lookTargets.PrimaryTarget.Thing 
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
        .Replace(MessagePlaceholder, Message.text.TrimEnd('.'));
    }

    public override void BuildData(DialogueDataMessage data)
    {
      data.QuestName = Message.quest?.name ?? string.Empty;
      data.QuestDescription = H.RemoveWhiteSpaceAndColor(Message.quest?.description);
      base.BuildData(data);
    }

    public override string? Action => null;

  }
}
