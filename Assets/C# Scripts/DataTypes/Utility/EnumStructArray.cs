using System;
using UnityEngine;


[Serializable]
public struct EnumStructArray<TEnum, TValue> where TEnum : Enum
{
    [SerializeField] private TValue[] values;
    public readonly TValue[] AsArray => values;


    public readonly TValue this[TEnum key]
    {
        get => values[Convert.ToInt32(key)];
        set => values[Convert.ToInt32(key)] = value;
    }
    public readonly int Length => values.Length;


    public readonly TValue GetValue(TEnum key)
    {
        return values[(int)(object)key];
    }
    public void SetFromArray(TValue[] array)
    {
        values = array;
    }
}