using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RimDialogue.Core
{
  internal static class OrtNativeBootstrap
  {
    static bool _done;

    public static void Init()
    {
      if (_done) return;
      _done = true;

      var asmDir = Path.Combine(
        Path.GetDirectoryName(Mod.Instance.Content.RootDir),
        "RimDialogueClient",
        "1.6");

      string nativeDir, libName;

      if (IsWindows())
      {
        nativeDir = Path.Combine(asmDir, "runtimes", "win-x64", "native");
        libName = "onnxruntime.dll";
      }
      else if (IsLinux())
      {
        nativeDir = Path.Combine(asmDir, "runtimes", "linux-x64", "native");
        libName = "libonnxruntime.so";
      }
      else // macOS
      {
        nativeDir = Path.Combine(asmDir, "runtimes", "osx-x64", "native");
        libName = "libonnxruntime.dylib";
      }

      string fullPath = Path.Combine(nativeDir, libName);
      if (!File.Exists(fullPath))
        throw new FileNotFoundException("Missing ONNX Runtime native library", fullPath);

      // Force load native lib so OnnxRuntime P/Invoke resolves here
      IntPtr handle = LoadLibraryCrossPlatform(fullPath);
      if (handle == IntPtr.Zero)
        throw new InvalidOperationException($"Failed to load {fullPath}");
    }

    static bool IsWindows() => Environment.OSVersion.Platform == PlatformID.Win32NT;
    static bool IsLinux() => Environment.OSVersion.Platform == PlatformID.Unix && Directory.Exists("/proc");

    static IntPtr LoadLibraryCrossPlatform(string path)
    {
      if (IsWindows())
        return WindowsLoadLibrary(path);
      else
        return UnixDlopen(path, RTLD_NOW);
    }

    // Windows
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern IntPtr LoadLibrary(string lpFileName);

    static IntPtr WindowsLoadLibrary(string path)
    {
      return LoadLibrary(path);
    }

    // Linux / macOS
    const int RTLD_NOW = 2;

    [DllImport("libdl", SetLastError = true)]
    static extern IntPtr dlopen(string fileName, int flags);

    static IntPtr UnixDlopen(string path, int flags)
    {
      return dlopen(path, flags);
    }
  }
}
