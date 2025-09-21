// Windows-only CF_DIB writer. Bottom-up, 32bpp BI_RGB, opaque alpha.
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Win32Clipboard
{
  const uint CF_DIB = 8;
  const uint GMEM_MOVEABLE = 0x0002;

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  struct BITMAPINFOHEADER
  {
    public uint biSize;        // 40
    public int biWidth;
    public int biHeight;      // positive = bottom-up
    public ushort biPlanes;     // 1
    public ushort biBitCount;   // 32
    public uint biCompression; // BI_RGB = 0
    public uint biSizeImage;   // stride * height
    public int biXPelsPerMeter;
    public int biYPelsPerMeter;
    public uint biClrUsed;
    public uint biClrImportant;
  }

  [DllImport("user32.dll", SetLastError = true)] static extern bool OpenClipboard(IntPtr hWndNewOwner);
  [DllImport("user32.dll", SetLastError = true)] static extern bool CloseClipboard();
  [DllImport("user32.dll", SetLastError = true)] static extern bool EmptyClipboard();
  [DllImport("user32.dll", SetLastError = true)] static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
  [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);
  [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr GlobalLock(IntPtr hMem);
  [DllImport("kernel32.dll", SetLastError = true)] static extern bool GlobalUnlock(IntPtr hMem);
  [DllImport("kernel32.dll")] static extern uint GetLastError();

  public static bool CopyImage(Texture2D tex)
  {
    if (tex == null) return false;
    var src = tex.GetPixels32();         // RGBA
    int w = tex.width, h = tex.height;
    int stride = w * 4;                  // DWORD aligned
    int imageBytes = stride * h;

    var bih = new BITMAPINFOHEADER
    {
      biSize = 40,
      biWidth = w,
      biHeight = h,
      biPlanes = 1,
      biBitCount = 32,
      biCompression = 0,               // BI_RGB
      biSizeImage = (uint)imageBytes
    };

    int headerSize = Marshal.SizeOf<BITMAPINFOHEADER>();
    int total = headerSize + imageBytes;

    IntPtr hMem = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)(uint)total);
    if (hMem == IntPtr.Zero) { Verse.Log.Error("Clipboard: GlobalAlloc failed."); return false; }

    IntPtr pMem = GlobalLock(hMem);
    if (pMem == IntPtr.Zero) { Verse.Log.Error("Clipboard: GlobalLock failed."); return false; }

    try
    {
      // Header
      Marshal.StructureToPtr(bih, pMem, false);

      // Pixels: BGRA, top-down, force A=255
      unsafe
      {
        byte* basePtr = (byte*)pMem + headerSize;
        for (int y = 0; y < h; y++)
        {
          byte* row = basePtr + (long)y * stride; // no flip
          int rowStart = y * w;
          for (int x = 0; x < w; x++)
          {
            var c = src[rowStart + x]; // RGBA
            *row++ = c.b;              // B
            *row++ = c.g;              // G
            *row++ = c.r;              // R
            *row++ = 255;              // A = opaque
          }
        }
      }
    }
    finally
    {
      GlobalUnlock(hMem);
    }

    if (!OpenClipboard(IntPtr.Zero)) { Verse.Log.Error($"Clipboard: OpenClipboard failed. err={GetLastError()}"); return false; }
    try
    {
      if (!EmptyClipboard()) { Verse.Log.Error($"Clipboard: EmptyClipboard failed. err={GetLastError()}"); return false; }
      if (SetClipboardData(CF_DIB, hMem) == IntPtr.Zero)
      {
        Verse.Log.Error($"Clipboard: SetClipboardData failed. err={GetLastError()}");
        return false; // app still owns hMem; OS frees on exit
      }
      return true;
    }
    finally { CloseClipboard(); }
  }
}
