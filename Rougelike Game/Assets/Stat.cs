using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a stat that can be altered by modifiers
/// </summary>
[System.Serializable]
public class Stat
{
    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    [SerializeField]
    private float baseValue;

    public int GetIntValue()
    {
        return Mathf.RoundToInt(GetValue());
    }

    /// <summary>
    /// Calculate the final value from the base value with modifiers
    /// </summary>
    /// <returns></returns>
    public float GetValue()
    {
        float finalValue = baseValue;
        foreach (var mod in modifiers)
        {
            finalValue += mod.value;
        }
        return finalValue;
    }

    public void AddModifier(float modifier)
    {
        modifiers.Add(new StatModifier(modifier, null));
    }

    public void AddModifier(float modifier, object source)
    {
        modifiers.Add(new StatModifier(modifier, source));
    }

    /// <summary>
    /// Remove modifiers that came from a source - a source can be anything
    /// </summary>
    /// <param name="source"></param>
    public void RemoveSource(object source)
    {
        List<StatModifier> toRemove = new List<StatModifier>();
        foreach (var mod in modifiers)
        {
            if (mod.source == source)
            {
                toRemove.Add(mod);
            }
        }

        toRemove.ForEach((mod) => modifiers.Remove(mod));
    }

}

/// <summary>
/// Holds a modifier's value and what object it came from
/// </summary>
[System.Serializable]
struct StatModifier
{
    public float value;
    public object source;

    public StatModifier(float value, object source)
    {
        this.value = value;
        this.source = source;
    }

}