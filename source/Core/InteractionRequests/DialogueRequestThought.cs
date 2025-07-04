#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestThought : DialogueRequestSinglePawn<ThoughtData>
  {
    public DialogueRequestThought(
      PlayLogEntry_InteractionSinglePawn entry,
      string label,
      string description,
      float moodOffset,
      string? preceptLabel,
      string? preceptDescription,
      Pawn? target) :
        base(entry)
    {
      Label = label;
      Description = description;
      Target = target;
      PreceptLabel = preceptLabel;
      PreceptDescription = preceptDescription;
      MoodOffset = moodOffset;
    }

    public string Label { get; set; }
    public string Description { get; set; }

    public string? PreceptLabel { get; set; }
    public string? PreceptDescription { get; set; }

    public float MoodOffset { get; set; }

    public Pawn? Target { get; set; }

    public override string? Action => "Thought";

    public override void BuildData(ThoughtData data)
    {
      data.Label = Label;
      data.Description = Description;
      data.PreceptLabel = PreceptLabel ?? string.Empty;
      data.PreceptDescription = PreceptDescription ?? string.Empty;
      data.MoodOffset = MoodOffset;
      base.BuildData(data);
    }
    public override void BuildForm(WWWForm form)
    {
      if (Target != null)
        form.AddField("targetJson", JsonUtility.ToJson(H.MakeData(Target, _tracker.GetInstructions(Target), -1)));
      base.BuildForm(form);
    }
  }
}
