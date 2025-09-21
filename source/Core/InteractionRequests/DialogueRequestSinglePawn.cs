#nullable enable
using RimDialogue.Access;
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionRequests
{
  public class DialogueRequestSinglePawn<DataT> : DialogueRequest<DataT> where DataT : InteractionData.DialogueData, new()
  {
    //public static DialogueRequestSinglePawn<DataT> BuildFrom(PlayLogEntry_InteractionSinglePawn entry)
    //{
    //  return new DialogueRequestSinglePawn<DataT>(entry);
    //}

    protected InteractionDef _interactionDef;
    protected string _instructions;
    protected Pawn _initiator;
    protected PawnData _initiatorData;

    public override bool KnownType { get; }

    public DialogueRequestSinglePawn(
      LogEntry entry,
      InteractionDef interactionDef,
      Pawn initiator,
      bool knownType = true) : base(entry)
    {
      _interactionDef = interactionDef;
      _initiator = initiator;
      if (_initiator == null)
        throw new Exception($"Entry {entry.LogID} - Initiator is null.");
      else
        Mod.Log($"Entry {entry.LogID} - Initiator is {_initiator}.");
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - pawn data built.");
      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist)
        _instructions += "\r\n" + _tracker.GetInstructions(InstructionsSet.COLONISTS);
      KnownType = knownType;
    }

    public DialogueRequestSinglePawn(
      PlayLogEntry_InteractionSinglePawn entry,
      InteractionDef interactionDef) :
      this(
        entry,
        interactionDef,
        (Pawn)Reflection.Verse_PlayLogEntry_InteractionSinglePawn_Initiator.GetValue(entry))
    {
      if (_initiator == null)
        throw new Exception($"Entry {entry.LogID} - Initiator is null.");
      else
        Mod.Log($"Entry {entry.LogID} - Initiator is {_initiator}.");
      _initiatorData = Initiator.MakeData(_tracker.GetInstructions(Initiator), entry.LogID);
      // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entry.LogID} - pawn data built.");
      _instructions = _tracker.GetInstructions(InstructionsSet.ALL_PAWNS) + "\r\n" + Settings.SpecialInstructions.Value;
      if (Initiator.IsColonist)
        _instructions += "\r\n" + _tracker.GetInstructions(InstructionsSet.COLONISTS);
    }
    public override Pawn Initiator => _initiator;

    public override PawnData InitiatorData => _initiatorData;

    public override InteractionDef InteractionDef => _interactionDef;

    public override string Instructions => _instructions;
    public override void AddConversation(string interaction, string text)
    {
      _tracker.AddConversation(Initiator, null, interaction, text);
    }

    public override async Task BuildData(DataT data)
    {
      await base.BuildData(data);
      if (Initiator.IsColonist)
      {
        var now = Find.TickManager.TicksAbs;
#if !RW_1_5
        if (GameComponent_ContextTracker.Instance != null)
        {
          var context = await GameComponent_ContextTracker.Instance
            .BlendedSearch(Initiator, Interaction, 5);
          data.Context = context
            .Select(context => context != null ? $"{(now - context.Tick).ToStringTicksToPeriod()} ago - {context.Text}" : string.Empty)
            .ToArray();
        }
#endif
      }
    }

    public override string Action => "DialogueSinglePawn";
    public override Rule[] Rules => [];

    public override IDictionary<string, string> Constants =>
      new Dictionary<string, string>();


  }
}
