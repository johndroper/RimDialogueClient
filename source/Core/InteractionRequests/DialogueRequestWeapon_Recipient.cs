#nullable enable
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeapon_Recipient : DialogueRequestWeapon
  {
    public static DialogueRequestWeapon_Recipient BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestWeapon_Recipient(entry, interactionTemplate);
    }

    public DialogueRequestWeapon_Recipient(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private ThingWithComps? _weapon = null;
    public override ThingWithComps Weapon
    {
      get
      {
        if (_weapon == null)
          _weapon = Recipient.equipment.AllEquipmentListForReading.RandomElement();
        return _weapon;
      }
    }
  }
}
