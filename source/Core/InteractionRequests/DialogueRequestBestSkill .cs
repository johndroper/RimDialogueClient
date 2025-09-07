#nullable enable
using RimWorld;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestBestSkill : DialogueRequestSkill
  {
    //public static new DialogueRequestBestSkill BuildFrom(PlayLogEntry_Interaction entry)
    //{
    //  return new DialogueRequestBestSkill(entry);
    //}

    public DialogueRequestBestSkill(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
    {

    }

    private SkillDef? _skillDef;
    public override SkillDef SkillDef
    {
      get
      {
        //TODO FIXME
        //what happens if this is about recipient's best skill?
        if (_skillDef == null)
        {
          var skills = Initiator.skills?.skills;
          var maxLevel = skills.Max(s => s.Level);
          _skillDef = skills.Where(s => s.Level == maxLevel).RandomElement().def;
        }
        return _skillDef;
      }
    }
  }
}
