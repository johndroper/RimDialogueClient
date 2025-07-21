#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core
{
  public class Conversation : IExposable, IEquatable<Conversation>
  {
    private Pawn initiator;
    private Pawn? recipient;
    public string? text;
    public string? interaction;
    public int? timestamp;
#pragma warning disable CS8618
    public Conversation() { }
#pragma warning restore CS8618

    public Conversation(Pawn initiator, Pawn? recipient, string? interaction, string text)
    {
      this.initiator = initiator;
      this.recipient = recipient;
      this.text = text;
      this.interaction = interaction;
      timestamp = Find.TickManager.TicksGame;
    }
    public Pawn Initiator => initiator;
    public Pawn? Recipient => recipient;
    public string Participants => $"{Initiator.Name?.ToStringShort ?? "Unknown"}" + Recipient != null ? $" ↔ {Recipient?.Name?.ToStringShort ?? "Unknown"}" : "";
    public bool InvolvesPawn(Pawn pawn)
    {
      return pawn.thingIDNumber == initiator.thingIDNumber || pawn.thingIDNumber == recipient?.thingIDNumber;
    }
    public bool InvolvesColonist()
    {
      return initiator != null && initiator.IsColonist || recipient != null && recipient.IsColonist;
    }

    public override string ToString()
    {
      return $"{initiator?.Name?.ToStringShort ?? "Unknown"}" + (recipient != null ? $" ↔ {recipient.Name?.ToStringShort ?? "Unknown"}" : "") + $" ({interaction ?? "No Interaction"}): {text}";
    }

    override public int GetHashCode()
    {
      return this.text?.GetHashCode() ?? base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this.text?.Equals((obj as Conversation)?.text) ?? base.Equals(obj);
    }

    public static string[] BreakWord(string text, int maxChars)
    {
      if (string.IsNullOrEmpty(text) || maxChars <= 0)
        return new string[] { text };
      List<string> words = new List<string>();
      for (int i = 0; i < text.Length; i += maxChars)
      {
        int length = Math.Min(maxChars, text.Length - i);
        words.Add(text.Substring(i, length));
      }
      return words.ToArray();
    }

    public static IEnumerable<string> BreakWords(IEnumerable<string> words, int maxCharsPerWord)
    {
      foreach (var line in words)
      {
        if (line.Length > maxCharsPerWord)
        {
          foreach (var wrappedLine in BreakWord(line, maxCharsPerWord))
          {
            yield return wrappedLine;
          }
        }
        else
        {
          yield return line;
        }
      }
    }

    private static string[] SplitLine(string line, int maxWordsPerLine)
    {
      var words = line.Split([' ', '\n', '\u200B'], StringSplitOptions.RemoveEmptyEntries);
      words = BreakWords(words, 10).ToArray();

      int wordsPerLine = (int)Math.Floor(Math.Max(1, (maxWordsPerLine * 4) / words.Average(word => word.Length)));
      if (wordsPerLine > maxWordsPerLine)
        wordsPerLine = maxWordsPerLine;
      int lineCount = (int)Math.Ceiling((float)words.Length / (float)wordsPerLine);
      List<string> fragments = [];
      for (int i = 0; i < lineCount; i++)
      {
        var fragment = string.Join(" ", words.Skip(i * wordsPerLine).Take(wordsPerLine));
        if (i > 0)
          fragment = "..." + fragment;
        if (i < lineCount - 1)
          fragment += "...";
        fragments.Add(fragment);
      }
      return fragments.ToArray();
    }

    static Regex regex = new Regex(@"^(?<name>\w+):\s*[""“*](?<line>.+)[""”*]\W*$", RegexOptions.Multiline);
    public static Line[] ParseLines(string text)
    {
      if (string.IsNullOrEmpty(text))
        return [];
      var lines = new List<Line>();
      var matches = regex.Matches(text);
      if (matches.Count == 0)
      {
        var soloLines = text
          .Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var soloLine in soloLines)
        {
          if (soloLine.Length > 300)
          {
            var splitLines = SplitLine(soloLine, 40);

            for (int i = 0; i < splitLines.Length; i++)
            {
              var splitLine = splitLines[i];
              lines.Add(new Line("unknown", splitLine));
            }
          }
          else
            lines.Add(new Line("unknown", soloLine));
        }
        return lines.ToArray();
      }

      foreach (Match match in matches)
      {
        if (match.Success)
        {
          var name = match.Groups["name"].Value;
          var line = match.Groups["line"].Value.Trim();
          if (line.Length > 150)
          {
            var splitLines = SplitLine(line, 20);
            foreach (var splitLine in splitLines)
            {
              lines.Add(new Line(name, splitLine));
            }
          }
          else
            lines.Add(new Line(name, line));
        }
      }
      return lines.ToArray();
    }

    Line[]? lines;
    public Line[] Lines
    {
      get
      {
        if (text == null)
          return [];
        lines ??= ParseLines(text);
        return lines;
      }
    }

    public void ExposeData()
    {
      Scribe_References.Look(ref initiator, "initiator");
      Scribe_References.Look(ref recipient, "recipient");
      Scribe_Values.Look(ref text, "text");
      Scribe_Values.Look(ref interaction, "interaction");
      Scribe_Values.Look(ref timestamp, "timestamp");
    }

    public bool Equals(Conversation other)
    {
      if (other == null) return false;
      return this.text?.Equals(other.text) ?? false;
    }
  }

}
