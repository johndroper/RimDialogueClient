#nullable enable
using HarmonyLib;
using RimDialogue.Configuration;
using RimDialogue.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse;

namespace RimDialogue
{
  public sealed class Mod : Verse.Mod
  {
    public const string Id = "ProceduralProducts.RimDialogue";
    public const string Name = "RimDialogue";
    public const string Version = "0.91.0";

    public static Mod Instance = null!;

    public static string FontPath
    {
      get
      {
        return Path.Combine(
          Instance.Content.RootDir,
          "Common",
          "BitmapFonts");
      }
    }


    public Mod(ModContentPack content) : base(content)
    {
      Instance = this;

      try
      {
        OrtNativeBootstrap.Init();
        Log("ONNX Runtime initialized successfully.");
      }
      catch (Exception ex)
      {
        Error($"Failed to load ONNX.{ex}");
      }

      GetSettings<Settings>();

      new Harmony(Id).PatchAll();

      Login();

      Log("Initialized");
    }

    public static LoginData? LoginData { get; private set; } = null;

    public static async void Login()
    {
      try
      {
        var serverUri = new Uri(Settings.ServerUrl.Value);
        var serverUrl = serverUri.GetLeftPart(UriPartial.Authority) + $"/Home/Login?clientId={UnityWebRequest.EscapeURL(Settings.ClientId.Value)}";
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
          var asyncOperation = request.SendWebRequest();
          while (!asyncOperation.isDone)
          {
            await Task.Yield();
          }
          if (request.isHttpError || request.isNetworkError)
            throw new Exception($"Network error logging in: {request.error} Url: {serverUrl}");
          else
          {
            while (!request.downloadHandler.isDone) { await Task.Yield(); }
            var body = request.downloadHandler.text;
            //if (Settings.VerboseLogging.Value) Log($"Login Body: '{body}'.");
            LoginData = JsonUtility.FromJson<LoginData>(body);
            Mod.Log($"You are logged in.");
            Mod.Log($"Tier: {LoginData.Tier}");
            Mod.Log($"Max Requests Per Minute: {LoginData.RateLimit * 60}");
            Mod.Log($"Max Prompt Length: {LoginData.MaxPromptLength:N0} characters");
            Mod.Log($"Max Output Words: {LoginData.maxOutputWords:N0} words");
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
    public static void ErrorOnce(string message, int key) => Verse.Log.ErrorOnce(PrefixMessage(message), key);
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
