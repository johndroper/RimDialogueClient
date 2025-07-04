using RimDialogue.Core.InteractionRequests;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace RimDialogue.Core.InteractionData
{
  public class DialogueRequestWeather : DialogueRequestTwoPawn<DialogueDataWeather>
  {
    public static new DialogueRequestWeather BuildFrom(PlayLogEntry_Interaction entry)
    {
      return new DialogueRequestWeather(entry);
    }

    public WeatherDef Weather { get; set; }
    public BiomeDef Biome { get; set; }
    public Season Season { get; set; }
    public float OutdoorTemp { get; set; }

    public DialogueRequestWeather(PlayLogEntry_Interaction entry) : base(entry)
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

    public override Rule[] Rules => [
      new Rule_String("weather", this.Weather.label),
      new Rule_String("biome", this.Biome.label),
      new Rule_String("season", this.Season.ToString()),
      new Rule_String("outdoor_temp", OutdoorTemp.ToString("F0"))
    ];

  }
}
