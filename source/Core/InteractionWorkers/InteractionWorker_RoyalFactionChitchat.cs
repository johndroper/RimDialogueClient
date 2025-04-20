using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_RoyalFactionChitchat : InteractionWorker_Dialogue
  {
    public static bool HasRoyalFactions = Find.FactionManager.GetFactions()
          .Any(faction => faction.def.HasRoyalTitles);

    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !HasRoyalFactions)
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Royal Faction ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.FactionChitChatWeight.Value}");
        return Settings.FactionChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_RoyalFactionChitchat: {ex}");
        return 0f;
      }
    }
  }
}
