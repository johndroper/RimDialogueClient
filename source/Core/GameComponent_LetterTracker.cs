#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core
{
  public class GameComponent_LetterTracker : GameComponent
  {
    public List<RecentLetter> RecentLetters = new List<RecentLetter>();
    private const int MaxTrackedLetters = 50;
    private const int LetterExpiryTicks = 2500 * 4;

    public GameComponent_LetterTracker(Game game) : base() { }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Collections.Look(ref RecentLetters, "RecentLetters", LookMode.Deep);
    }

    public override void GameComponentUpdate()
    {
      base.GameComponentUpdate();
      RecentLetters.RemoveAll(letter => Find.TickManager.TicksAbs - letter.Ticks > LetterExpiryTicks);
    }

    public void RecordLetter(RecentLetter letter)
    {
      if (letter == null)
        return;
      RecentLetters.Add(letter);
      if (RecentLetters.Count > MaxTrackedLetters)
        RecentLetters.RemoveAt(0);
    }

    public void ClearAll()
    {
      RecentLetters.Clear();
    }

    public static GameComponent_LetterTracker Instance =>
        Current.Game.GetComponent<GameComponent_LetterTracker>();

    public class RecentLetter : IExposable
    {
      public TaggedString Label;
      public TaggedString Text;
      public LetterDef Def;
      //public LookTargets LookTargets;
      public Faction RelatedFaction;
      public Quest? Quest;
      //public List<ThingDef>? HyperlinkThingDefs;
      public int Ticks;
#pragma warning disable CS8618
      public RecentLetter() { }
#pragma warning restore CS8618
      public RecentLetter(
          TaggedString label,
          TaggedString text,
          LetterDef def,
          LookTargets lookTargets,
          Faction relatedFaction,
          Quest? quest,
          List<ThingDef> hyperlinkThingDefs)
      {
        Label = label;
        Text = text;
        Def = def;
        //LookTargets = lookTargets;
        RelatedFaction = relatedFaction;
        Quest = quest;
        //HyperlinkThingDefs = hyperlinkThingDefs;
        Ticks = Find.TickManager.TicksAbs;
      }

      public void ExposeData()
      {
        Scribe_Values.Look(ref Label, "Label");
        Scribe_Values.Look(ref Text, "Text");
        Scribe_Defs.Look(ref Def, "Def");
        //Scribe_TargetInfo.Look(ref LookTargets, "LookTargets");
        Scribe_References.Look(ref RelatedFaction, "RelatedFaction");
        Scribe_References.Look(ref Quest, "Quest");
        //Scribe_Collections.Look(ref HyperlinkThingDefs, "HyperlinkThingDefs", LookMode.Def);
        Scribe_Values.Look(ref Ticks, "Ticks");
      }
    }
  }
}
