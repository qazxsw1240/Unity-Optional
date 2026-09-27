#nullable enable

using System;
using System.Runtime.CompilerServices;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Unity.Utility.Functional.Extensions
{
  public static class ObjectOptional
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<Object> FindFirstObjectByType(Type type, FindObjectsInactive findObjectsInactive)
    {
      return Object.FindFirstObjectByType(type, findObjectsInactive).ToUnityOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<Object> FindAnyObjectByType(Type type, FindObjectsInactive findObjectsInactive)
    {
      return Object.FindAnyObjectByType(type, findObjectsInactive).ToUnityOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> FindFirstObjectByType<T>() where T : Object
    {
      return Object.FindFirstObjectByType<T>().ToUnityOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> FindAnyObjectByType<T>() where T : Object
    {
      return Object.FindAnyObjectByType<T>().ToUnityOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> FindFirstObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
    {
      return Object.FindFirstObjectByType<T>(findObjectsInactive).ToUnityOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> FindAnyObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
    {
      return Object.FindAnyObjectByType<T>(findObjectsInactive).ToUnityOptional();
    }
  }
}
