#nullable enable
using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public abstract class DialogueRequestWeapon : DialogueRequestTwoPawn<DialogueDataWeapon>
  {
    public DialogueRequestWeapon(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient) : base(entry, interactionDef, initiator, recipient)
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

    public override async Task BuildData(DialogueDataWeapon data)
    {
      data.WeaponLabel = Weapon.def?.label ?? string.Empty;
      data.WeaponDescription = H.RemoveWhiteSpace(Weapon.def?.description) ?? string.Empty;
      data.WeaponQuality = Weapon.TryGetQuality(out var quality) ? quality.GetLabel() : string.Empty;
      await base.BuildData(data);
    }

    public override string? Action => "WeaponChitchat";

    public override Rule[] Rules => [new Rule_String("weapon", Weapon?.LabelNoParenthesis ?? "RimDialogue.Unknown".Translate())];

  }
}
