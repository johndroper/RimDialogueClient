using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_WildAnimal : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var animals = Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(p => p.RaceProps.Animal && p.Faction == null);
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !animals.Any())
        {
          return 0f;
        }
        // if (Settings.VerboseLogging.Value) Mod.Log($"Wild Animal ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AnimalChitChatWeight.Value}");
        return Settings.AnimalChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_WildAnimal: {ex}");
        return 0f;
      }
    }
  }
}
