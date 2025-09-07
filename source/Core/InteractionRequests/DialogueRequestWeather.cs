using RimDialogue.Core.InteractionRequests;
using RimWorld;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeather : DialogueRequestTwoPawn<DialogueDataWeather>
  {
    public DialogueRequestWeather(
      PlayLogEntry_Interaction entry,
      InteractionDef interactionDef,
      Pawn initiator,
      Pawn recipient
      ) : base(entry, interactionDef, initiator, recipient)
    {
    }

    public override Rule[] Rules => [
      new Rule_String("weather", this.Weather.label),
      new Rule_String("biome", this.Biome.label),
      new Rule_String("season", this.Season.ToString()),
      new Rule_String("outdoor_temp", OutdoorTemp.ToString("F0"))
    ];
  }
}
