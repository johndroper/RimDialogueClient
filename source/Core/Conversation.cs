#nullable enable
using RimDialogue.Core.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core
{
  public class Conversation :  IExposable
  {


    private Pawn? initiator;
    private Pawn? recipient;

    private int? initiatorId;
    private int? recipientId;

    public string? Text;
    public string? Interaction;
    public int? Timestamp;
#pragma warning disable CS8618
    public Conversation() { }
#pragma warning restore CS8618

    public Conversation(
      Pawn initiator,
      Pawn? recipient,
      string? interaction,
      string text)
    {
      //Mod.Log($"New conversation: {initiator}");
      if (initiator == null)
        throw new ArgumentNullException(nameof(initiator), "Initiator cannot be null.");
      this.initiator = initiator;
      initiatorId = initiator.thingIDNumber;
      this.recipient = recipient;
      if (recipient != null)
        recipientId = recipient.thingIDNumber;
      Text = text;
      Interaction = interaction;
      Timestamp = Find.TickManager.TicksAbs;
    }

    public Pawn? Initiator
    {
      get
      {
        initiator ??= TrackerUtils.GetPawnById(initiatorId);
        return initiator;
      }
    }

    public Pawn? Recipient
    {
      get
      {
        if (recipient == null && recipientId != null)
          recipient = TrackerUtils.GetPawnById(recipientId.Value);
        return recipient;
      }
    }

    public string Participants => $"{Initiator?.Name?.ToStringShort ?? "Unknown"}" + (Recipient != null ? $" ↔ {Recipient?.Name?.ToStringShort ?? "Unknown"}" : "");
    public bool Involves(Thing thing)
    {
      return thing.thingIDNumber == initiatorId || thing.thingIDNumber == recipientId;
    }
    public bool InvolvesColonist()
    {
      return (Initiator != null && Initiator.IsColonist) || (Recipient != null && Recipient.IsColonist);
    }

    public override string ToString()
    {
      return $"{Initiator?.Name?.ToStringShort ?? "Unknown"}" + (Recipient != null ? $" ↔ {Recipient.Name?.ToStringShort ?? "Unknown"}" : "") + $" ({Interaction ?? "No Interaction"}): {Text}";
    }

    private string? _formattedText;
    public string? FormattedText
    {
      get
      {
        if (Text == null || Initiator == null)
          return null;
        if (_formattedText == null)
        {
          var initiatorName = Initiator.Name?.ToStringShort ?? Initiator.Label;
          _formattedText = Text.Replace(
            initiatorName,
            initiatorName.Colorize(ColoredText.NameColor));
          if (Recipient != null)
          {
            var recipientName = Recipient.Name?.ToStringShort ?? Recipient.Label;
            _formattedText = _formattedText.Replace(
              recipientName,
              recipientName.Colorize(ColoredText.NameColor));
          }
        }
        return _formattedText;
      }
    }


    //override public int GetHashCode()
    //{
    //  return this.Text?.GetHashCode() ?? base.GetHashCode();
    //}

    //public override bool Equals(object obj)
    //{
    //  return this.Text?.Equals((obj as Conversation)?.Text) ?? base.Equals(obj);
    //}

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
        if (Text == null)
          return [];
        lines ??= ParseLines(Text);
        return lines;
      }
    }



    public void ExposeData()
    {
      Scribe_Values.Look(ref initiatorId, "initiatorId");
      Scribe_Values.Look(ref recipientId, "recipientId");
      Scribe_Values.Look(ref Text, "text");
      Scribe_Values.Look(ref Interaction, "interaction");
      Scribe_Values.Look(ref Timestamp, "timestamp");
    }
  }

}
