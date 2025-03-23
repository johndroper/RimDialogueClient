using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_DialogueIncident : InteractionWorker_Dialogue
  {
    public static int lastUsedTicks = 0;
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !Verse_LetterMaker_MakeLetter.recentLetters.Any() ||
          lastUsedTicks > GetMinTime())
        {
          Mod.LogV($"Incident ChitChat Weight: {initiator.Name} -> {recipient.Name} = 0");
          return 0f;
        }
        Mod.LogV($"Incident ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.RecentIncidentChitChatWeight.Value}");
        return Settings.RecentIncidentChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error(ex.ToString());
        return 0f;
      }
    }
  }
}
