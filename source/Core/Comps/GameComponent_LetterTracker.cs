#nullable enable
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core
{
  public class GameComponent_LetterTracker : GameComponent
  {
    public List<RecentLetter> RecentLetters = new List<RecentLetter>();
    private const int MaxTrackedLetters = 150;
    private const int LetterExpiryTicks = 2500 * 4;

    public GameComponent_LetterTracker(Game game) : base() { }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Collections.Look(ref RecentLetters, "RecentLetters", LookMode.Deep);

      if (Scribe.mode == LoadSaveMode.PostLoadInit)
        RecentLetters.RemoveAll(letter => letter == null || string.IsNullOrEmpty(letter.Label) || letter.Label == "error");
    }

    //public override void GameComponentUpdate()
    //{
    //  base.GameComponentUpdate();
    //  //if (Find.TickManager.TicksAbs % 2500 == 0 ) 
    //  //  RecentLetters.RemoveAll(letter => Find.TickManager.TicksAbs - letter.Ticks > LetterExpiryTicks);
    //}

    public void RecordLetter(RecentLetter letter)
    {
      try
      {
        if (letter == null)
          return;
        RecentLetters.Add(letter);
        if (RecentLetters.Count > MaxTrackedLetters)
          RecentLetters.RemoveAt(0);

#if !RW_1_5
        if (GameComponent_ContextTracker.Instance != null)
          GameComponent_ContextTracker.Instance.Add(letter);
#endif

        if (Settings.VerboseLogging.Value)
          Mod.Log($"Letter stored: '{letter.Label}'");
      }
      catch (System.Exception ex)
      {
        Mod.Error("Error recording letter: " + ex.ToString());
      }
    }

    public void ClearAll()
    {
      RecentLetters.Clear();
    }

    public static GameComponent_LetterTracker Instance =>
        Current.Game.GetComponent<GameComponent_LetterTracker>();


  }

  public class RecentLetter : IExposable
  {
    public string? Label;
    public string? Text;
    public int Ticks;
    public int? TargetId;
    public string? Type;

#pragma warning disable CS8618
    public RecentLetter() { }
#pragma warning restore CS8618
    public RecentLetter(
      string label,
      string text,
      string type,
      Pawn? target)
    {
      Label = label;
      Text = text;
      Ticks = Find.TickManager.TicksAbs;
      TargetId = target?.thingIDNumber;
      Type = type;
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
      Scribe_Values.Look(ref Label, "Label");
      Scribe_Values.Look(ref Text, "Text");
      Scribe_Values.Look(ref Ticks, "Ticks");
      Scribe_Values.Look(ref TargetId, "TargetId");
      Scribe_Values.Look(ref Type, "Type");
    }
  }
}
