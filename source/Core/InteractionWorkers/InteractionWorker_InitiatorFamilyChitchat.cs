using RimDialogue.Access;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_InitiatorFamilyChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      try
      {
        var relations = initiator.relations.DirectRelations;
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !relations.Any())
        {
          return 0f;
        }
        if (Settings.VerboseLogging.Value) Mod.Log($"Family ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.FamilyChitChatWeight.Value}");
        return Settings.FamilyChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_FamilyChitchat: {ex}");
        return 0f;
      }
    }
  }
}

