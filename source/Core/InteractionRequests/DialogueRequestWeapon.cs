#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestWeapon : DialogueRequestTwoPawn<DialogueDataWeapon>
  {
    const string Placeholder = "**weapon**";

    public DialogueRequestWeapon(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {

    }

    private ThingWithComps? _weapon;
    public virtual ThingWithComps Weapon
    {
      get
      {
        _weapon ??= Initiator.equipment.AllEquipmentListForReading.RandomElement();
        return _weapon;
      }
    }

    public override void BuildData(DialogueDataWeapon data)
    {
      data.WeaponLabel = Weapon.def?.label ?? string.Empty;
      data.WeaponDescription = H.RemoveWhiteSpace(Weapon.def?.description) ?? string.Empty;
      data.WeaponQuality = Weapon.TryGetQuality(out var quality) ? quality.GetLabel() : string.Empty;
    }

    public override string? Action => "WeaponChitchat";

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Weapon.LabelNoParenthesis);
    }
  }
}
