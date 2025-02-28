using RimDialogue.Access;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimDialogue.Core
{
  public class InteractionWorker_RecentIncidentChitChat : InteractionWorker
  {
    public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
    {
      if (initiator.Inhumanized() || !initiator.IsColonist || !recipient.IsColonist || !Verse_LetterMaker_MakeLetter.recentLetters.Any())
      {
        Mod.LogV($"InteractionWorker_RecentIncidentChitChat RandomSelectionWeight: {initiator.Name} -> {recipient.Name} = 0");
        return 0f;
      }
      Mod.LogV($"InteractionWorker_RecentIncidentChitChat RandomSelectionWeight: {initiator.Name} -> {recipient.Name} = 1");
      return 1f;
    }
  }
}
