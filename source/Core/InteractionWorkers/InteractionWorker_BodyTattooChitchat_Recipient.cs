using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_BodyTattooChitchat_Recipient : InteractionWorker_Dialogue
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
          recipient.style.BodyTattoo == null ||
          recipient.style.BodyTattoo.defName == "NoTattoo_Body" ||
          recipient.style.BodyTattoo.label == "none")
          return 0f;

        if (Settings.VerboseLogging.Value) Mod.Log($"Body Tattoo Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_BodyTattooChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}
