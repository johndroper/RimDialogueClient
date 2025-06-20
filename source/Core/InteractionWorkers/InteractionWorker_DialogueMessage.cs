using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueMessage : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !GameComponent_MessageTracker.Instance.TrackedMessages.Any())
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Message ChitChat Weight: {initiator.Name} -> {recipient.Name} = 1");
        return Settings.MessageChitChatWeight.Value * Settings.TimelyEventWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
        return 0f;
      }
    }
  }
}
