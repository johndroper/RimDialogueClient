#nullable enable
using RimDialogue.Core.InteractionData;
using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_WeaponChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.equipment == null ||
          initiator.equipment.Primary == null)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Weapon ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Weapon ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.WeaponChitChatWeight.Value}");
        return Settings.WeaponChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_InitiatorWeaponChitchat: {ex}");
        return 0f;
      }
    }

    public override DialogueRequest CreateRequest(
      PlayLogEntry_Interaction entry,
      InteractionDef intDef,
      Pawn initiator,
      Pawn? recipient)
    {
      if (recipient == null)
        throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null.");
      return new DialogueRequestWeapon_Initiator(entry, intDef, initiator, recipient);
    }
  }
}
