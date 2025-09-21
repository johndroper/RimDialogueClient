#nullable enable
using Google.Protobuf.WellKnownTypes;
using RimDialogue.Access;
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestRecord : DialogueRequestTwoPawn<DialogueData>
  {

    public static RecordDef[] TimeRecordDefs = DefDatabase<RecordDef>
      .AllDefsListForReading
      .Where(def => def.type == RecordType.Time)
      .ToArray();

    public static RecordDef[] FloatRecordDefs = DefDatabase<RecordDef>
      .AllDefsListForReading
      .Where(def => def.type == RecordType.Float)
      .ToArray();

    public static RecordDef[] IntRecordDefs = DefDatabase<RecordDef>
      .AllDefsListForReading
      .Where(def => def.type == RecordType.Int)
      .ToArray();

    private Rule[] _rules;
    private Dictionary<string, string> _constants = new();

    //public static Regex TimeRegex = new();

    public DialogueRequestRecord(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {
      var selectedRecord = DefDatabase<RecordDef>
        .AllDefsListForReading
        .Select(recordDef => (RecordDef: recordDef, Value: initiator.records.GetValue(recordDef)))
        .Where(t => t.Value > 0)
        .RandomElement();
      Rule_String valueRule;
      string label = selectedRecord.RecordDef.label;
      switch (selectedRecord.RecordDef.type)
      {
        case RecordType.Int:
          valueRule = new Rule_String("record_value", selectedRecord.Value.ToString("n0"));
          break;
        case RecordType.Time:
          valueRule = new Rule_String("record_value", ((int)selectedRecord.Value).ToStringTicksToPeriod());
          label = label.Replace("RimDialogue.Time".Translate(), string.Empty).Trim();
          break;
        default:
          valueRule = new Rule_String("record_value", initiator.records.GetValue(selectedRecord.RecordDef).ToString("n1"));
          break;
      }
      _rules = [
        new Rule_String(
          "record_name",
          label),
        valueRule,
      ];
    }
    public override Rule[] Rules => _rules;

    public override IDictionary<string, string> Constants => _constants;
  }
}
