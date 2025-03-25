using RimDialogue.Access;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimDialogue.Core.InteractionWorkers
{
  public class InteractionWorker_ColonistChitchat : InteractionWorker_Dialogue
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      var colonists = Reflection.RimWorld_ColonistBar_TmpColonistsInOrder.GetValue(Find.ColonistBar) as List<Pawn>;
      try
      {
        if (
          !IsEnabled ||
          initiator.Inhumanized() ||
          !initiator.IsColonist ||
          !recipient.IsColonist ||
          !Find.ColonistBar
            .GetColonistsInOrder()
            .Where(colonist => colonist != initiator && colonist != recipient)
            .Any())
        {
          return 0f;
        }
        Mod.LogV($"Colonists ChitChat Weight: {initiator.Name} -> {recipient.Name} = {Settings.ColonistChitChatWeight.Value}");
        return Settings.ColonistChitChatWeight.Value;
      }
      catch (Exception ex)
      {
        Mod.Error($"Error in InteractionWorker_ColonistsChitchat: {ex}");
        return 0f;
      }
    }
  }
}
