#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeapon_Initiator : DialogueRequestWeapon
  {
    public static new DialogueRequestWeapon_Initiator BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestWeapon_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestWeapon_Initiator(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private ThingWithComps? _weapon = null;
    public override ThingWithComps Weapon
    {
      get
      {
        if (_weapon == null)
          _weapon = Initiator.equipment.AllEquipmentListForReading.RandomElement();
        return _weapon;
      }
    }
  }
}
