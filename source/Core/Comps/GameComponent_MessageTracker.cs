#nullable enable
using RimDialogue.Context;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core
{
  public class GameComponent_MessageTracker : GameComponent
  {
    public List<MessageRecord> TrackedMessages = new List<MessageRecord>();
    private const int MaxTrackedMessages = 250;
    private const int MaxMessageAgeTicks = 2500 * 4;

    public GameComponent_MessageTracker(Game game) : base() { }

    public override void FinalizeInit()
    {
      base.FinalizeInit();
      lastCleanupTick = Find.TickManager.TicksGame + GenDate.TicksPerHour;
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Collections.Look(
        ref TrackedMessages,
        "TrackedMessages2",
        LookMode.Deep);
      if (Scribe.mode == LoadSaveMode.LoadingVars && TrackedMessages == null)
        TrackedMessages = new List<MessageRecord>();
    }

    public int lastCleanupTick = GenDate.TicksPerHour;
    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();
      try
      {
        if (Find.TickManager.TicksGame > lastCleanupTick + GenDate.TicksPerHour)
        {
          lastCleanupTick = Find.TickManager.TicksGame;
          TrackedMessages.RemoveAll(record => Find.TickManager.TicksAbs - record.Ticks > MaxMessageAgeTicks);
        }
          
      }
      catch (System.Exception ex)
      {
        Log.ErrorOnce($"Error updating message tracker: {ex}", 90834234);
      }
    }

    public void AddMessage(Message message)
    {
      try
      {
        if (message == null || Find.TickManager.gameStartAbsTick == 0)
          return;
        TrackedMessages.Add(new MessageRecord(message, Find.TickManager.TicksAbs));
        if (TrackedMessages.Count > MaxTrackedMessages)
          TrackedMessages.RemoveAt(0);
#if !RW_1_5
        if (GameComponent_ContextTracker.Instance != null)
        {
          var context = TemporalContextCatalog.Create(message);
          if (context == null)
            return;
          GameComponent_ContextTracker.Instance.Add(context);
        }
#endif
      }
      catch (System.Exception ex)
      {
        Mod.Error("Error adding message to tracker: " + ex.ToString());
      }
    }

    public MessageRecord GetRandomMessage()
    {
      var now = Find.TickManager.TicksAbs;
      return TrackedMessages.RandomElementByWeight(messageRecord => (float)messageRecord.Ticks / now);
    }

    public void ClearAll()
    {
      TrackedMessages.Clear();
    }

    public static GameComponent_MessageTracker? Instance =>
        Current.Game != null ? Current.Game.GetComponent<GameComponent_MessageTracker>() : null;
  }

  public class MessageRecord : IExposable
  {
    public string? MessageText;
    public int Ticks;
    public int? TargetId;

#pragma warning disable CS8618
    public MessageRecord() { }
#pragma warning restore CS8618
    public MessageRecord(Message message, int ticks)
    {
      MessageText = message.text;
      Ticks = ticks;
      TargetId = message.lookTargets?.PrimaryTarget.Pawn?.thingIDNumber;
    }

    private Pawn? _target = null;
    public Pawn? Target
    {
      get
      {
        if (TargetId == null)
          return null;
        _target ??= PawnsFinder.AllMaps.FirstOrDefault(Pawn => Pawn.thingIDNumber == TargetId.Value);
        return _target;
      }
    }

    public void ExposeData()
    {
      Scribe_Values.Look(ref MessageText, "MessageText");
      Scribe_Values.Look(ref Ticks, "Ticks");
      Scribe_Values.Look(ref TargetId, "TargetId");
    }
  }
}
