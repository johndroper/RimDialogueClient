#nullable enable
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeapon_Initiator : DialogueRequestWeapon
  {
    public static new DialogueRequestWeapon_Initiator BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestWeapon_Initiator(entry);
    }

    public DialogueRequestWeapon_Initiator(PlayLogEntry_Interaction entry) : base(entry)
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
