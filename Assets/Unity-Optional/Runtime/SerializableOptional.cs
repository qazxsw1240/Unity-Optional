#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Unity.Utility.Functional
{
  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public sealed class SerializableOptional<T> where T : notnull
  {
    [SerializeField]
    private bool m_HasValue;

    [SerializeField]
    private T? m_Value;

    internal SerializableOptional() : this(false, default) { }

    internal SerializableOptional(bool hasValue, T? value)
    {
      m_HasValue = hasValue;
      m_Value = value;
    }

    public static SerializableOptional<T> Empty
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SerializableOptional<T> From(T value)
    {
      return new(true, value);
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the optional has a value.
    /// </summary>
    public bool HasValue
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => m_HasValue;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set
      {
        if (!(m_HasValue = value))
        {
          m_Value = default;
        }
      }
    }

    public T? Value
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => m_Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Optional<T> Build()
    {
      bool hasValue = m_HasValue && m_Value != null;
      return new(hasValue, hasValue ? m_Value : default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SerializableOptional<T>(Optional<T> optional)
    {
      return new(optional.HasValue, optional.Value);
    }
  }
}
