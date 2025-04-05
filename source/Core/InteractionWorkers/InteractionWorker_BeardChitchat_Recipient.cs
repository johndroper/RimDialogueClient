using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_BeardChitchat_Recipient : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          recipient.style == null ||
          recipient.style.beardDef == null ||
          recipient.style.beardDef.defName == "NoBeard" ||
          recipient.style.beardDef.label == "none")
        {
          if (Settings.VerboseLogging.Value) Mod.Log($"Recipient Beard ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Recipient Beard ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_BeardChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}
