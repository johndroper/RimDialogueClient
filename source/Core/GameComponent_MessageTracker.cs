#nullable enable
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core
{
  public class GameComponent_MessageTracker : GameComponent
  {
    public List<MessageRecord> TrackedMessages = new List<MessageRecord>();
    private const int MaxTrackedMessages = 25;
    private const int MaxMessageAgeTicks = 2500 * 4;

    public GameComponent_MessageTracker(Game game) : base() { }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Collections.Look(ref TrackedMessages, "TrackedMessages", LookMode.Deep);
    }

    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();
      TrackedMessages.RemoveAll(record => Find.TickManager.TicksGame - record.Ticks > MaxMessageAgeTicks);
    }

    public void AddMessage(Message message)
    {
      if (message == null)
        return;
      TrackedMessages.Add(new MessageRecord(message, Find.TickManager.TicksGame));
      if (TrackedMessages.Count > MaxTrackedMessages)
      {
        TrackedMessages.RemoveAt(0);
      }
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
    public Message Message;
    public int Ticks;
#pragma warning disable CS8618
    public MessageRecord() { }
#pragma warning restore CS8618
    public MessageRecord(Message message, int ticks)
    {
      Message = message;
      Ticks = ticks;
    }

    public void ExposeData()
    {
      Scribe_Deep.Look(ref Message, "Message");
      Scribe_Values.Look(ref Ticks, "Ticks");
    }
  }
}
