using UnityEngine;
using Verse;

namespace RimDialogue.Core
{
  [StaticConstructorOnStartup]
  public static class Textures
  {
    public static readonly Texture2D Icon = ContentFinder<Texture2D>.Get("RimDialogue/Icon");
    public static readonly Texture2D Inner = ContentFinder<Texture2D>.Get("RimDialogue/Inner");
    public static readonly Texture2D Outer = ContentFinder<Texture2D>.Get("RimDialogue/Outer");
  }
}
