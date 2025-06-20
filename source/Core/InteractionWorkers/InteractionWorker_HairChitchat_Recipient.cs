using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_HairChitchat_Recipient : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          recipient.Inhumanized() ||
          recipient.story == null ||
          recipient.story.hairDef == null ||
          recipient.story.hairDef.defName == HairDefOf.Bald.defName)
          return 0f;

        // if (Settings.VerboseLogging.Value) Mod.Log($"Hair Weight: {initiator.Name} -> {recipient.Name} = {Settings.AppearanceChitChatWeight.Value}");
        return Settings.AppearanceChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_HairChitchat_Recipient: {ex}");
        return 0f;
      }
    }
  }
}
