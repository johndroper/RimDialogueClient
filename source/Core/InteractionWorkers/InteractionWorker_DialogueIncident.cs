using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueIncident : InteractionWorker_Dialogue
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
          !GameComponent_LetterTracker.Instance.RecentLetters.Any())
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Incident ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.RecentIncidentChitChatWeight.Value}");
        return Settings.RecentIncidentChitChatWeight.Value * Settings.TimelyEventWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
        return 0f;
      }
    }
  }
}
