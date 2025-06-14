using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeather : DialogueRequestTwoPawn<DialogueDataWeather>
  {
    public static new DialogueRequestWeather BuildFrom(PlayLogEntry_Interaction entry, string interactionTemplate)
    {
      return new DialogueRequestWeather(entry, interactionTemplate);
    }

    public WeatherDef Weather { get; set; }
    public BiomeDef Biome { get; set; }
    public Season Season { get; set; }
    public float OutdoorTemp { get; set; }

    public DialogueRequestWeather(PlayLogEntry_Interaction entry, string interactionTemplate) : base(entry, interactionTemplate)
    {
      Weather = Find.CurrentMap.weatherManager.CurWeatherPerceived;
      Biome = Find.CurrentMap.Biome;
      Season = GenLocalDate.Season(Find.CurrentMap);
      OutdoorTemp = Find.CurrentMap.mapTemperature.OutdoorTemp;
    }

    public override void BuildData(DialogueDataWeather data)
    {
      data.WeatherLabel = this.Weather.label;
      data.WeatherDescription = this.Weather.description;
      data.BiomeLabel = this.Biome.label;
      data.BiomeDescription = this.Biome.description;
      data.Season = this.Season.ToString();
      data.OutdoorTemp = this.OutdoorTemp;
      base.BuildData(data);
    }

    public override string Action => "WeatherChitchat";

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
