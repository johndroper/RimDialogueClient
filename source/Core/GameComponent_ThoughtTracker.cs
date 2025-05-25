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
      if (Settings.VerboseLogging.Value) Mod.Log($"Created '{thought.GetType().Name}' interaction. Defname: '{thought.def.defName}' Label: '{thought.LabelCap}'");
      return interaction;
    }

    public static void ImitateInteraction(
      Pawn initiator,
      Thought_InteractionDef thoughtInteraction)
    {
      try
      {
        if (Settings.VerboseLogging.Value) Mod.Log($"Imitating Interaction - {thoughtInteraction.label} ");
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
    public List<Pawn> Pawns
    {
      get
      {
        _pawns ??= (List<Pawn>)Reflection.Verse_MapPawns_HumanlikeSpawnedPawnsResult.GetValue(Find.CurrentMap.mapPawns);
        return _pawns;
      }
    }

    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();

      if (Pawns == null || !Pawns.Any())
        return;

      if (Find.TickManager.TicksGame - LastThoughtTick < MinDelayTicks / Pawns.Count)
        return;

      if (Settings.VerboseLogging.Value) Mod.Log($"Doing thought update.");

      LastThoughtTick = Find.TickManager.TicksGame;

      var pawn = Pawns.RandomElement();
      var situational_thoughts = (List<Thought_Situational>)Reflection.RimWorld_SituationalThoughtHandler_CachedThoughts.GetValue(pawn.needs.mood.thoughts.situational);
      var thought_memories = pawn.needs.mood.thoughts.memories.Memories;
      if (situational_thoughts != null && situational_thoughts.Any() && Rand.Chance(Settings.ThoughtChance.Value))
      {
        var thought = situational_thoughts.RandomElement();
        if (thought == null || thought.CurStageIndex < 0 || thought.CurStageIndex >= thought.def.stages.Count)
          return;
        if (Settings.VerboseLogging.Value) Mod.Log($"Situational thought selected: {thought}.");
        var thoughtInteraction = CreateThoughtInteraction(thought);
        ImitateInteraction(thought.pawn, thoughtInteraction);
      }
      else if (thought_memories != null && thought_memories.Any() && Rand.Chance(Settings.ThoughtChance.Value))
      {
        var thought = thought_memories.RandomElement();
        if (thought == null || thought.CurStageIndex < 0 || thought.CurStageIndex >= thought.def.stages.Count)
          return;
        if (Settings.VerboseLogging.Value) Mod.Log($"Memory thought selected: {thought}.");
        var thoughtInteraction = CreateThoughtInteraction(thought, thought.otherPawn);
        ImitateInteraction(thought.pawn, thoughtInteraction);
      }
    }
  }
}
