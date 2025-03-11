using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Linq;

namespace BlindServerCore;

public static partial class GlobalExtensionMethods
{
    public static bool TryAddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ref var valueRefOrNullRef = ref CollectionsMarshal.GetValueRefOrNullRef(source, key);
        if (Unsafe.IsNullRef(ref valueRefOrNullRef))
        {
            source.Add(key, value);
            return false;
        }

        valueRefOrNullRef = value;
        return true;
    }

    /// <summary>
    /// 레퍼런스로 반환
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <returns>현재 값 레버펀스로 반환</returns>
    public static ref TValue GetValueRef<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key) where TKey : notnull
    {
        return ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
    }

    public static TValue GetValueRefSet<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue setValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = setValue;
        return before;
    }

    public static long GetValueRefInc<TKey>(this Dictionary<TKey, long> source, TKey key, long incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before + incValue;
        return before;
    }

    public static uint GetValueRefInc<TKey>(this Dictionary<TKey, uint> source, TKey key, uint incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before + incValue;
        return before;
    }

    public static int GetValueRefInc<TKey>(this Dictionary<TKey, int> source, TKey key, int incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before + incValue;
        return before;
    }

    public static ushort GetValueRefInc<TKey>(this Dictionary<TKey, ushort> source, TKey key, ushort incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = (ushort)(before + incValue);
        return before;
    }

    public static short GetValueRefInc<TKey>(this Dictionary<TKey, short> source, TKey key, short incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = (short)(before + incValue);
        return before;
    }

    public static float GetValueRefInc<TKey>(this Dictionary<TKey, float> source, TKey key, float incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before + incValue;
        return before;
    }

    public static double GetValueRefInc<TKey>(this Dictionary<TKey, double> source, TKey key, double incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before + incValue;
        return before;
    }

    public static byte GetValueRefInc<TKey>(this Dictionary<TKey, byte> source, TKey key, byte incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = (byte)(before + incValue);
        return before;
    }

    public static long GetValueRefDnc<TKey>(this Dictionary<TKey, long> source, TKey key, long incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before - incValue;
        return before;
    }

    public static uint GetValueRefDnc<TKey>(this Dictionary<TKey, uint> source, TKey key, uint incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before - incValue;
        return before;
    }

    public static int GetValueRefDnc<TKey>(this Dictionary<TKey, int> source, TKey key, int incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before - incValue;
        return before;
    }

    public static ushort GetValueRefDnc<TKey>(this Dictionary<TKey, ushort> source, TKey key, ushort incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = (ushort)(before - incValue);
        return before;
    }

    public static short GetValueRefDnc<TKey>(this Dictionary<TKey, short> source, TKey key, short incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = (short)(before - incValue);
        return before;
    }

    public static float GetValueRefDnc<TKey>(this Dictionary<TKey, float> source, TKey key, float incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before - incValue;
        return before;
    }

    public static double GetValueRefDnc<TKey>(this Dictionary<TKey, double> source, TKey key, double incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = before - incValue;
        return before;
    }

    public static byte GetValueRefDnc<TKey>(this Dictionary<TKey, byte> source, TKey key, byte incValue) where TKey : notnull
    {
        ref var oldVlaue = ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out bool exists);
        var before = oldVlaue;
        oldVlaue = (byte)(before - incValue);
        return before;
    }

    public static Span<T> AsSpan<T>(this List<T> list)
    {
        return CollectionsMarshal.AsSpan(list);
    }

    public static Dictionary<TKey, TValue> Combine<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> dest)
    {
        if (source != null && dest == null)
        {
            return source.ToDictionary();
        }

        if (dest != null && source == null)
        {
            return dest.ToDictionary();
        }

        var response = source.ToDictionary();

        foreach (var element in source)
        {
            if (dest.TryGetValue(element.Key, out var destValue))
            {
                response[element.Key] = destValue;
            }
        }

        foreach (var element in dest)
        {
            if (source.ContainsKey(element.Key) == false)
            {
                response.Add(element.Key, element.Value);
            }
        }

        return response;
    }
}
