using HarmonyLib;
using RimDialogue.Configuration;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimDialogue
{
  public sealed class Mod : Verse.Mod
  {
    public const string Id = "ProceduralProducts.RimDialogue";
    public const string Name = "RimDialogue";
    public const string Version = "0.78.0";

    public static Mod Instance = null!;

    public static string FontPath
    {
      get
      {
        return Path.Combine(
          Instance.Content.RootDir,
          "BitmapFonts");
      }
    }


    public Mod(ModContentPack content) : base(content)
    {
      Instance = this;

      GetSettings<Settings>();

      new Harmony(Id).PatchAll();
      Log("Initialized");
    }

    public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));
    public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));
    public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
    private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";

    public override void DoSettingsWindowContents(Rect inRect)
    {
      SettingsEditor.DrawSettings(inRect);
      base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory() => Name;

    public static void ErrorV(string text)
    {
      if (Settings.VerboseLogging.Value)
        Error(text);
    }

  }
}
