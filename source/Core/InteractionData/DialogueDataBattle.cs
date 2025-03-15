using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  [Serializable]
  public class DialogueDataBattle : DialogueData
  {
    public string Name;
    public string Duration;
    public string Importance;
    public string[] Entries = [];
    public string[] Participants = [];
  }
}
