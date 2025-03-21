using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueDataSkill : DialogueData
  {
    public string SkillName = String.Empty;
    public string SkillDescription = String.Empty;
    public string InitiatorSkillLevel = String.Empty;
    public string InitiatorPassion = String.Empty;
    public string RecipientSkillLevel = String.Empty;
    public string RecipientPassion = String.Empty;
  }
}
