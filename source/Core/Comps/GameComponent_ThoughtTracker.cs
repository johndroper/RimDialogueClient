#nullable enable
using Bubbles.Access;
using RimDialogue.Core.InteractionDefs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static RimDialogue.Core.GameComponent_LetterTracker;
using Reflection = RimDialogue.Access.Reflection;

namespace RimDialogue.Core
{
  public class GameComponent_ThoughtTracker : GameComponent
  {
    public const int MinDelayTicks = 6000;
    public int LastThoughtTick = 0;

    public static GameComponent_ThoughtTracker Instance =>
      Current.Game.GetComponent<GameComponent_ThoughtTracker>();

    public static Thought_InteractionDef CreateThoughtInteraction(Thought thought, Pawn? otherPawn = null)
    {
      var interaction = new Thought_InteractionDef(
        DefDatabase<InteractionDef>.GetNamed("Thought"),
        thought,
        otherPawn);
      // if (Settings.VerboseLogging.Value) Mod.Log($"Created '{thought.GetType().Name}' interaction. Defname: '{thought.def.defName}' Label: '{thought.LabelCap}'");
      return interaction;
    }

    public static void ImitateInteraction(
      Pawn initiator,
      Thought_InteractionDef thoughtInteraction)
    {
      try
      {
        // if (Settings.VerboseLogging.Value) Mod.Log($"Imitating Interaction - {thoughtInteraction.label} ");
        PlayLogEntry_InteractionThought.ImitateInteractionWithNoPawn(
          initiator,
          thoughtInteraction);
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
      }
    }

    public GameComponent_ThoughtTracker(Game game) : base()
    {

    }

    private List<Pawn>? _pawns;
    public List<Pawn>? Pawns
    {
      get
      {
        if (Find.CurrentMap?.mapPawns == null)
          return null;

        _pawns ??= (List<Pawn>)Reflection.Verse_MapPawns_HumanlikeSpawnedPawnsResult.GetValue(Find.CurrentMap.mapPawns);
        return _pawns;
      }
    }

    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();

      if (
        Pawns == null ||
        !Pawns.Any() ||
        Find.TickManager == null ||
        Find.TickManager.TicksAbs - LastThoughtTick < MinDelayTicks / Pawns.Count)
      return;

      // if (Settings.VerboseLogging.Value) Mod.Log($"Doing thought update.");

      LastThoughtTick = Find.TickManager.TicksAbs;

      var pawn = Pawns.RandomElement();
      var situationalThoughtHandler = pawn?.needs?.mood?.thoughts?.situational;
      List<Thought_Situational>? situational_thoughts = null;
      if (situationalThoughtHandler != null)
        situational_thoughts = (List<Thought_Situational>)Reflection.RimWorld_SituationalThoughtHandler_CachedThoughts.GetValue(situationalThoughtHandler);
      var thought_memories = pawn?.needs?.mood?.thoughts?.memories?.Memories;
      if (situational_thoughts != null && situational_thoughts.Any() && Rand.Chance(Settings.ThoughtChance.Value))
      {
        var thought = situational_thoughts.RandomElementByWeight(thought => thought.CurStageIndex >= 0 ? Math.Abs(thought.MoodOffset()): 0);
        if (thought == null || thought.CurStageIndex < 0 || thought.CurStageIndex >= thought.def.stages.Count || thought.MoodOffset() == 0)
          return;
        // if (Settings.VerboseLogging.Value) Mod.Log($"Situational thought selected: {thought}.");
        var thoughtInteraction = CreateThoughtInteraction(thought);
        ImitateInteraction(thought.pawn, thoughtInteraction);
      }
      else if (thought_memories != null && thought_memories.Any() && Rand.Chance(Settings.ThoughtChance.Value))
      {
        var thought = thought_memories.RandomElementByWeight(thought => Math.Max(thought.CurStageIndex >= 0 ? Math.Abs(thought.MoodOffset()) : 1f, 1f));
        if (thought == null || thought.CurStageIndex < 0 || thought.CurStageIndex >= thought.def.stages.Count || thought.MoodOffset() == 0)
          return;
        // if (Settings.VerboseLogging.Value) Mod.Log($"Memory thought selected: {thought}.");
        var thoughtInteraction = CreateThoughtInteraction(thought, thought.otherPawn);
        ImitateInteraction(thought.pawn, thoughtInteraction);
      }
    }
  }
}
