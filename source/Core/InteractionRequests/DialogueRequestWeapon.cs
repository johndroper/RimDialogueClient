#nullable enable
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestWeapon : DialogueRequest<DialogueDataWeapon>
  {
    const string Placeholder = "**weapon**";

    public DialogueRequestWeapon(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
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

    public override void Execute()
    {
      var dialogueData = new DialogueDataWeapon();
      dialogueData.WeaponLabel = Weapon.def?.label ?? string.Empty;
      dialogueData.WeaponDescription = H.RemoveWhiteSpace(Weapon.def?.description) ?? string.Empty;
      dialogueData.WeaponQuality = Weapon.TryGetQuality(out var quality) ? quality.GetLabel() : string.Empty;

      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "WeaponChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate.Replace(Placeholder, Weapon.LabelNoParenthesis);
    }
  }
}
