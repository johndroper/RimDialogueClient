using HarmonyLib;
using RimDialogue.Configuration;
using RimDialogue.Core;
using RimDialogue.Core.InteractionData;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;
using static RimWorld.ColonistBar;

namespace RimDialogue
{
  public sealed class Mod : Verse.Mod
  {
    public const string Id = "ProceduralProducts.RimDialogue";
    public const string Name = "RimDialogue";
    public const string Version = "0.79.1";

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

      Login();

      Log("Initialized");
    }

    public static LoginData LoginData { get; private set; } = null;

    public static async void Login()
    {
      var serverUri = new Uri(Settings.ServerUrl.Value);
      var serverUrl = serverUri.GetLeftPart(UriPartial.Authority) + "/Login";
      try
      {
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
          var asyncOperation = request.SendWebRequest();
          while (!asyncOperation.isDone)
          {
            await Task.Yield();
          }
          if (request.isHttpError || request.isNetworkError)
            throw new Exception($"Network error logging in: {request.error}");
          else
          {
            while (!request.downloadHandler.isDone) { await Task.Yield(); }
            var body = request.downloadHandler.text;
            // if (Settings.VerboseLogging.Value) Mod.Log($"Entry {entryId} - Body: '{body}'.");
            LoginData = JsonUtility.FromJson<LoginData>(body);
          }
        }
      }
      catch (Exception ex)
      {
        Error($"Error logging in: {ex.Message}");
      }
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
