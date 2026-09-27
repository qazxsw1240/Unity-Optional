#nullable enable

using System.Runtime.CompilerServices;

using UnityEngine;

namespace Unity.Utility.Functional.Extensions
{
  public static class UnityExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentOptional<T>(this GameObject gameObject) where T : class
    {
      return gameObject.TryGetComponent(out T targetComponent)
        ? Optional<T>.From(targetComponent)
        : Optional<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentOptional<T>(this Transform transform) where T : class
    {
      return transform.TryGetComponent(out T targetComponent)
        ? Optional<T>.From(targetComponent)
        : Optional<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentOptional<T>(this Component component) where T : class
    {
      return component.TryGetComponent(out T targetComponent)
        ? Optional<T>.From(targetComponent)
        : Optional<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentInChildrenOptional<T>(this GameObject gameObject) where T : class
    {
      return gameObject.GetComponentInChildren<T>().ToOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentInChildrenOptional<T>(this Transform transform) where T : class
    {
      return transform.GetComponentInChildren<T>().ToOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentInChildrenOptional<T>(this Component component) where T : class
    {
      return component.GetComponentInChildren<T>().ToOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentInParentOptional<T>(this GameObject gameObject) where T : class
    {
      return gameObject.GetComponentInParent<T>().ToOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentInParentOptional<T>(this Transform transform) where T : class
    {
      return transform.GetComponentInParent<T>().ToOptional();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> GetComponentInParentOptional<T>(this Component component) where T : class
    {
      return component.GetComponentInParent<T>().ToOptional();
    }
  }
}
