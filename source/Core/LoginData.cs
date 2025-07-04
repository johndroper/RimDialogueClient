using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core
{
  public class LoginData
  {
    public string Tier;
    public int MaxPromptLength;
    public int MaxResponseLength;
    public int MaxOutputWords;
    public float RateLimit;
    public int RateLimitCacheMinutes;
    public int MinRateLimitRequestCount;
    public string[] Models;
  }
}
