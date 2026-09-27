#nullable enable

using System.Runtime.CompilerServices;

using UnityEngine;

namespace Unity.Utility.Functional.Extensions
{
  public static class GameObjectOptional
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<GameObject> Find(string name)
    {
      return GameObject.Find(name).ToUnityOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<GameObject> FindWithTag(string tag)
    {
      return GameObject.FindWithTag(tag).ToUnityOptional();
    }
  }
}
