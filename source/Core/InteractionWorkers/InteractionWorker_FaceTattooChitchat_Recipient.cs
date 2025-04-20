using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_FaceTattooChitchat_Recipient : InteractionWorker_Dialogue
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
          recipient.style.FaceTattoo == null ||
          recipient.style.FaceTattoo.defName == "NoTattoo_Face" ||
          recipient.style.FaceTattoo.label == "none")
          return 0f;

        if (Settings.VerboseLogging.Value) Mod.Log($"Face Tattoo Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_FaceTattooChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}
