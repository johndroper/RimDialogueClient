using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ColonyAnimal : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var animals = Find.CurrentMap.mapPawns.SpawnedColonyAnimals;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !animals.Any())
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Animal ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.AnimalChitChatWeight.Value}");
        return Settings.AnimalChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_Animal: {ex}");
        return 0f;
      }
    }
  }
}
