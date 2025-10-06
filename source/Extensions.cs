using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue
{
  public static class Extensions
  {
    public static string Clean(this string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return string.Empty;
      var sb = new StringBuilder();
      foreach (var c in input)
      {
        if (char.IsLetter(c) || char.IsDigit(c))
          sb.Append(c);
        else
          sb.Append('_');
      }
      return sb.ToString();
    }
  }
}
