using System;

namespace RimDialogue.Context
{
  public static class VectorMath
  {
    public static float Cosine(float[] a, float[] b)
    {
      if (a.Length != b.Length) throw new ArgumentException("Vector length mismatch");
      double dot = 0, na = 0, nb = 0;
      for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
      if (na == 0 || nb == 0) return 0f;
      return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
    }
    public static void L2NormalizeInPlace(float[] v)
    {
      double n = 0; for (int i = 0; i < v.Length; i++) n += v[i] * v[i];
      if (n <= 0) return;
      float inv = (float)(1.0 / Math.Sqrt(n));
      for (int i = 0; i < v.Length; i++) v[i] *= inv;
    }
  }
}
