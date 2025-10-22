#nullable enable
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Verse;

namespace RimDialogue.Core.Scenarios
{
  public class ScenPart_ConfigPage_LoadCanon : ScenPart_ConfigPage
  {
    private string? canonDir = Path.Combine(
      Mod.Instance.Content.RootDir,
      "Common",
      "Canon");

    public string selectedFileName = string.Empty;

    public override string Summary(Scenario scen)
    {
      return $"Load canon from: {selectedFileName ?? "(none)"}";
    }

    public override void PostIdeoChosen()
    {
      if (canonDir == null || string.IsNullOrEmpty(selectedFileName))
      {
        Mod.Error($"No canon file selected. {canonDir} {selectedFileName}");
        return;
      }

      var abs = Path.Combine(canonDir, selectedFileName);
      try
      {
        if (GameComponent_ContextTracker.Instance == null)
        {
          Mod.Error("ContextTracker instance is null when loading canon.");
          return;
        }

        using (var stream = new FileStream(abs, FileMode.Open))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
          var i = 0;
          while(!reader.EndOfStream)
          {
            GameComponent_ContextTracker.Instance.Canon.Add(reader.ReadLine());
            i++;
          }
          Mod.Log($"Loaded {i} canon entries from {abs}");
        }
      }
      catch (Exception ex)
      {
        Mod.Error($"Error loading canon file {abs}: {ex}");
      }
    }
  }
}
