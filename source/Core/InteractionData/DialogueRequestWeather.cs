using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeather : DialogueRequest<DialogueDataWeather>
  {
    public static DialogueRequestWeather BuildFrom(LogEntry entry, string interactionTemplate)
    {
      return new DialogueRequestWeather(entry, interactionTemplate);
    }

    public WeatherDef Weather { get; set; }
    public BiomeDef Biome { get; set; }
    public Season Season { get; set; }
    public float OutdoorTemp { get; set; }

    public DialogueRequestWeather(LogEntry entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Weather = Find.CurrentMap.weatherManager.CurWeatherPerceived;
      Biome = Find.CurrentMap.Biome;
      Season = GenLocalDate.Season(Find.CurrentMap);
      OutdoorTemp = Find.CurrentMap.mapTemperature.OutdoorTemp;
    }

    public override void Execute()
    {
      var dialogueData = new DialogueDataWeather
      {
        WeatherLabel = this.Weather.label,
        WeatherDescription = this.Weather.description,
        BiomeLabel = this.Biome.label,
        BiomeDescription = this.Biome.description,
        Season = this.Season.ToString(),
        OutdoorTemp = this.OutdoorTemp,
      };
      Build(dialogueData);
      Send(
        [
          new("chitChatJson", dialogueData)
        ],
        "WeatherChitchat");
    }

    public override string GetInteraction()
    {
      return this.InteractionTemplate
          .Replace("**weather**", this.Weather.label)
          .Replace("**biome**", this.Biome.label)
          .Replace("**season**", this.Season.ToString())
          .Replace("**outdoor_temp**", OutdoorTemp.ToString("F0"));
    }
  }
}
