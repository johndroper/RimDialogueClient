using RimWorld;
using System;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_SameIdeology : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          initiator.Ideo.name != recipient.Ideo.name)
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Same Ideology ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.SameIdeologyChitChatWeight.Value}");
        return Settings.SameIdeologyChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_Ideology: {ex}");
        return 0f;
      }
    }
  }
}
