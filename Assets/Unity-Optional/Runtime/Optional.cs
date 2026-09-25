#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Object = UnityEngine.Object;

namespace Unity.Utility.Functional
{
  [StructLayout(LayoutKind.Sequential)]
  public readonly struct Optional<T> : IEquatable<Optional<T>> where T : notnull
  {
    private readonly bool m_HasValue;
    private readonly T? m_Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Optional(bool hasValue, T? value)
    {
      m_HasValue = hasValue;
      m_Value = value;
    }

    public static Optional<T> Empty
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new(false, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> From(T value)
    {
      return new(true, value);
    }

    public bool HasValue
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => m_HasValue;
    }

    public T Value
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => m_HasValue ? m_Value! : throw new NullReferenceException("Value is null.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue([NotNullWhen(true)] out T value)
    {
      value = m_Value!;
      return m_HasValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<TOther> Cast<TOther>() where TOther : notnull
    {
      return TryGetValue(out T value) && value is TOther other
        ? Optional<TOther>.From(other)
        : Optional<TOther>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<T> Or(Optional<T> other)
    {
      return m_HasValue ? this : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OrElse(T other)
    {
      return TryGetValue(out T value) ? value : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<T> Filter(Func<T, bool> predicate)
    {
      if (!TryGetValue(out T value))
      {
        return Empty;
      }
      return predicate.Invoke(value) ? this : Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<T> FlatFilter(Func<T, Optional<bool>> predicate)
    {
      if (!TryGetValue(out T value) || !predicate.Invoke(value).TryGetValue(out bool result))
      {
        return Empty;
      }
      return result ? this : Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull
    {
      return TryGetValue(out T value)
        ? Optional<TResult>.From(mapper.Invoke(value))
        : Optional<TResult>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<TResult> FlatMap<TResult>(Func<T, Optional<TResult>> mapper) where TResult : notnull
    {
      return TryGetValue(out T value)
        ? mapper.Invoke(value)
        : Optional<TResult>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<TResult> Merge<TOther, TResult>(Optional<TOther> other, Func<T, TOther, TResult> merger)
      where TOther : notnull
      where TResult : notnull
    {
      if (!TryGetValue(out T value) || !other.TryGetValue(out TOther otherValue))
      {
        return Optional<TResult>.Empty;
      }
      return Optional<TResult>.From(merger.Invoke(value, otherValue));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<TResult> FlatMerge<TOther, TResult>(
      Optional<TOther> other,
      Func<T, TOther, Optional<TResult>> merger)
      where TOther : notnull
      where TResult : notnull
    {
      if (!TryGetValue(out T value) || !other.TryGetValue(out TOther otherValue))
      {
        return Optional<TResult>.Empty;
      }
      return merger.Invoke(value, otherValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
      return m_HasValue ? $"Optional({m_Value})" : "Optional.Empty";
    }

    public bool Equals(Optional<T> other)
    {
      bool isEmpty = !m_HasValue;
      if (isEmpty)
      {
        return m_HasValue == other.m_HasValue;
      }
      return m_HasValue == other.m_HasValue && EqualityComparer<T>.Default.Equals(m_Value!, other.m_Value!);
    }

    public override bool Equals(object? obj)
    {
      return obj is Optional<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
      return m_HasValue ? HashCode.Combine(m_Value) : 0;
    }

    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(Optional<T> left, Optional<T> right)
    {
      return !left.Equals(right);
    }
  }

  public static class Optional
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> ToOptional<T>(this T value, ForStruct unused = default) where T : struct
    {
      return Optional<T>.From(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> ToOptional<T>(this T? value, ForNullableStruct unused = default) where T : struct
    {
      return value.HasValue ? Optional<T>.From(value.Value) : Optional<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> ToOptional<T>(this T? value, ForManaged unused = default) where T : class
    {
      return value != null ? Optional<T>.From(value) : Optional<T>.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> ToUnityOptional<T>(this T? value, ForUnityObject unused = default) where T : Object
    {
      return value ? Optional<T>.From(value) : Optional<T>.Empty;
    }

    /// <summary>
    ///   Marker struct.
    /// </summary>
    public readonly struct ForStruct { }

    /// <summary>
    ///   Marker struct.
    /// </summary>
    public readonly struct ForNullableStruct { }

    /// <summary>
    ///   Marker struct.
    /// </summary>
    public readonly struct ForManaged { }

    /// <summary>
    ///   Marker struct.
    /// </summary>
    public readonly struct ForUnityObject { }
  }
}
