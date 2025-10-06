using System.Runtime.InteropServices;

public static class PlatformCheck
{
  public static bool IsWindows()
  {
    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  }
}
