using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_WeaponChitchat_Recipient : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          recipient.equipment == null ||
          recipient.equipment.Primary == null)
        {
          // if (Settings.VerboseLogging.Value) Mod.Log($"Recipient Weapon ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Recipient Weapon ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.WeaponChitChatWeight.Value}");
        return Settings.WeaponChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_WeaponChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}
