using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{


  public class InteractionWorker_Dialogue : InteractionWorker
  {
    public const float TicksPerMinute = 41.6667f;

    public static int LastUsedTicksAll { get; set; } = 0;
    public static Dictionary<string, int> LastTicksByType = [];

    public static int GetLastUsedTicks(string workerType)
    {
      if (LastTicksByType.TryGetValue(workerType, out var lastUsed))
        return lastUsed;
      else
        return 0;
    }

    public void SetLastUsedTicks()
    {
      int ticksAbs = Find.TickManager.TicksAbs;
      LastUsedTicksAll = ticksAbs;
      LastTicksByType[this.GetType().Name] = ticksAbs;
    }

    public int LastUsedTicks
    {
      get
      {
        return GetLastUsedTicks(this.GetType().Name);
      }
    }

    public bool IsEnabled
    {
      get
      {
        //if (Find.TickManager.CurTimeSpeed > (TimeSpeed)Settings.MaxSpeed.Value)
        //{
        //  // if (Settings.VerboseLogging.Value) Mod.Log($"Game speed is too high.");
        //  return false;
        //}
        int ticksAbs = Find.TickManager.TicksAbs;
        if (ticksAbs - LastUsedTicksAll < Settings.MinDelayMinutesAll.Value * TicksPerMinute)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Too soon since last dialogue. Current ticks: '{ticksAbs}' Last used ticks: {LastUsedTicksAll}");
          return false;
        }
        if (ticksAbs - LastUsedTicks < Settings.MinDelayMinutes.Value * TicksPerMinute)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Too soon since last dialogue of type {this.GetType().Name}. Current ticks: '{ticksAbs}' Last used ticks: {LastUsedTicks}");
          return false;
        }
        return true;
      }
    }
  }
}
