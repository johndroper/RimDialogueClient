#nullable enable
namespace RimDialogue.Core
{
  using RimWorld;
  using System.Collections.Generic;
  using Verse;

  public class GameComponent_PawnDeathTracker : GameComponent
  {
    public List<PawnDeathRecord> DeadColonists = new List<PawnDeathRecord>();
    public List<PawnDeathRecord> DeadColonistAnimals = new List<PawnDeathRecord>();
    public List<PawnDeathRecord> DeadHostiles = new List<PawnDeathRecord>();
    public List<PawnDeathRecord> DeadNeutrals = new List<PawnDeathRecord>();
    public List<PawnDeathRecord> DeadAllies = new List<PawnDeathRecord>();

    private const int MaxTrackedOthers = 25;

    public GameComponent_PawnDeathTracker(Game game) : base() { }

    public override void ExposeData()
    {
      base.ExposeData();

      Scribe_Collections.Look(ref DeadColonists, "DeadColonists", LookMode.Deep);
      Scribe_Collections.Look(ref DeadColonistAnimals, "DeadColonistAnimals", LookMode.Deep);
      Scribe_Collections.Look(ref DeadHostiles, "DeadHostiles", LookMode.Deep);
      Scribe_Collections.Look(ref DeadNeutrals, "DeadNeutrals", LookMode.Deep);
      Scribe_Collections.Look(ref DeadAllies, "DeadAllies", LookMode.Deep);
    }

    public void RecordDeath(Pawn pawn, DamageInfo? dinfo, Hediff exactCulprit)
    {
      if (pawn == null || !pawn.Dead)
        return;

      string cause = GenerateCauseString(dinfo, exactCulprit);
      int timestamp = GenTicks.TicksGame;
      var record = new PawnDeathRecord(pawn, cause, timestamp);

      if (pawn.Faction == Faction.OfPlayer)
      {
        if (pawn.RaceProps.Animal)
        {
          DeadColonistAnimals.Add(record);
        }
        else if (pawn.IsColonist)
        {
          DeadColonists.Add(record);
        }
      }
      else
      {
        var relation = pawn.Faction?.RelationKindWith(Faction.OfPlayer) ?? FactionRelationKind.Neutral;
        switch (relation)
        {
          case FactionRelationKind.Hostile:
            AddLimited(DeadHostiles, record);
            break;
          case FactionRelationKind.Ally:
            AddLimited(DeadAllies, record);
            break;
          case FactionRelationKind.Neutral:
          default:
            AddLimited(DeadNeutrals, record);
            break;
        }
      }
    }

    private string GenerateCauseString(DamageInfo? dinfo, Hediff exactCulprit)
    {
      if (dinfo.HasValue)
      {
        var instigator = dinfo.Value.Instigator?.LabelCap ?? "Unknown";
        var def = dinfo.Value.Def?.label ?? "Unknown damage";
        var weapon = dinfo.Value.Weapon?.label;
        return weapon != null
            ? $"Killed by {instigator} with {weapon} ({def})"
            : $"Killed by {instigator} ({def})";
      }

      if (exactCulprit != null)
      {
        return $"Killed by hediff: {exactCulprit.LabelCap}";
      }

      return "Cause unknown";
    }

    private void AddLimited(List<PawnDeathRecord> list, PawnDeathRecord record)
    {
      list.Add(record);
      if (list.Count > MaxTrackedOthers)
      {
        list.RemoveAt(0);
      }
    }

    public void ClearAll()
    {
      DeadColonists.Clear();
      DeadColonistAnimals.Clear();
      DeadHostiles.Clear();
      DeadNeutrals.Clear();
      DeadAllies.Clear();
    }

    public static GameComponent_PawnDeathTracker Instance =>
        Current.Game.GetComponent<GameComponent_PawnDeathTracker>();
  }


  public class PawnDeathRecord : IExposable
  {
    public Pawn Pawn;
    public string Cause;
    public int TimeStamp;

    public PawnDeathRecord() { }

    public PawnDeathRecord(Pawn pawn, string cause, int ticksAtDeath)
    {
      Pawn = pawn;
      Cause = cause;
      TimeStamp = ticksAtDeath;
    }

    public void ExposeData()
    {
      Scribe_References.Look(ref Pawn, "Pawn");
      Scribe_Values.Look(ref Cause, "Cause");
      Scribe_Values.Look(ref TimeStamp, "TicksAtDeath");
    }
  }
}
