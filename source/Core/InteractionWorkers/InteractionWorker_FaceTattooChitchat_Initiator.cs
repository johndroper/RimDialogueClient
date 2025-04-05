using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_FaceTattooChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.style == null ||
          initiator.style.FaceTattoo == null ||
          initiator.style.FaceTattoo.defName == "NoTattoo_Face" ||
          initiator.style.FaceTattoo.label == "none")
        {
          if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Face Tattoo ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Face Tattoo ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_FaceTattooChitchat_Initiator: {ex}");
        return 0f;
      }
    }
  }
}
