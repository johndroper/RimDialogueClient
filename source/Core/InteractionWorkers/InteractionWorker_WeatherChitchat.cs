using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_WeatherChitchat : InteractionWorker_Dialogue
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
          !initiator.IsOutside() ||
          !recipient.IsOutside())
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Weather ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.WeatherChitChatWeight.Value}");
        return Settings.WeatherChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_WeatherChitchat: {ex}");
        return 0f;
      }
    }
  }
}
