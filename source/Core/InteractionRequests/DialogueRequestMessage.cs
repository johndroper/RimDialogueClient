#nullable enable

using RimDialogue.Core.InteractionRequests;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestMessage : DialogueRequestTarget<DialogueDataMessage>
  {
    const string MessagePlaceholder = "message";

    static Dictionary<int, DialogueRequestMessage> recent = [];

    public static new DialogueRequestMessage BuildFrom(PlayLogEntry_Interaction entry)
    {
      if (recent.ContainsKey(entry.LogID))
        return recent[entry.LogID];
      var item = new DialogueRequestMessage(entry);
      recent.Add(entry.LogID, item);
      return item;
    }

    public Message Message { get; set; }
    protected Pawn? _target { get; set; }

    public override Pawn? Target => _target;

    public DialogueRequestMessage(PlayLogEntry_Interaction entry) : base(entry)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for message {entry.LogID} with template {interactionTemplate}.");

      Message = GameComponent_MessageTracker.Instance.TrackedMessages.RandomElement().Message;
      var tracker = H.GetTracker();
      _target = Message.lookTargets?.PrimaryTarget.Pawn;

     //Message.lookTargets.PrimaryTarget.Thing 
    }

    public override Rule[] Rules => [
      new Rule_String(MessagePlaceholder, Message.text.TrimEnd('.')),
      new Rule_String("questName", Message.quest?.name ?? string.Empty),
      new Rule_String("questDescription", H.RemoveWhiteSpaceAndColor(Message.quest?.description))
    ];

    public override void BuildData(DialogueDataMessage data)
    {
      data.QuestName = Message.quest?.name ?? string.Empty;
      data.QuestDescription = H.RemoveWhiteSpaceAndColor(Message.quest?.description);
      base.BuildData(data);
    }

    public override string? Action => null;

  }
}
