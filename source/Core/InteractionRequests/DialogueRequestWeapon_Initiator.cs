#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeapon_Initiator : DialogueRequestWeapon
  {
    public static DialogueRequestWeapon_Initiator BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestWeapon_Initiator(entry, interactionTemplate);
    }

    public DialogueRequestWeapon_Initiator(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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
