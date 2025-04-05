using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_HairChitchat_Initiator : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.story == null ||
          initiator.story.hairDef == null ||
          initiator.story.hairDef.defName == HairDefOf.Bald.defName)
        {
          if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Hair ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Initiator Hair ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_HairChitchat_Initiator: {ex}");
        return 0f;
      }
    }
  }
}
