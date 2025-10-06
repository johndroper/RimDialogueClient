#nullable enable

using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestMessage : DialogueRequestTarget<DialogueDataMessage>
  {
    const string MessagePlaceholder = "message";

    //static Dictionary<int, DialogueRequestMessage> recent = [];

    //public static new DialogueRequestMessage BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  if (recent.ContainsKey(entry.LogID))
    //    return recent[entry.LogID];
    //  var item = new DialogueRequestMessage(entry);
    //  recent.Add(entry.LogID, item);
    //  return item;
    //}

    public MessageRecord MessageRecord { get; set; }
    public override Pawn? Target => MessageRecord.Target;

    public DialogueRequestMessage(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      // if (Settings.VerboseLogging.Value) Mod.Log($"Creating dialogue request for message {entry.LogID} with template {interactionTemplate}.");

      MessageRecord = GameComponent_MessageTracker.Instance.GetRandomMessage();

      //Message.lookTargets.PrimaryTarget.Thing 
    }

    public override Rule[] Rules => [
      new Rule_String(MessagePlaceholder, MessageRecord.MessageText?.TrimEnd('.'))
    ];

    //public override async Task BuildData(DialogueDataMessage data)
    //{
    //  data.QuestName = MessageRecord.quest?.name ?? string.Empty;
    //  data.QuestDescription = H.RemoveWhiteSpaceAndColor(MessageRecord.quest?.description);
    //  await base.BuildData(data);
    //}

    public override string? Action => null;

  }
}
