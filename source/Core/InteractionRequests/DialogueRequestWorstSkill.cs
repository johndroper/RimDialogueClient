#nullable enable
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWorstSkill : DialogueRequestSkill
  {
    public static new DialogueRequestWorstSkill BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestWorstSkill(entry, interactionTemplate);
    }

    public DialogueRequestWorstSkill(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private SkillDef? _skillDef;
    public override SkillDef SkillDef
    {
      get
      {
        if (_skillDef == null)
        {
          var skills = Initiator.skills?.skills;
          var level = skills.Min(s => s.Level);
          _skillDef = skills.Where(s => s.Level == level).RandomElement().def;
        }
        return _skillDef;
      }
    }
  }
}
