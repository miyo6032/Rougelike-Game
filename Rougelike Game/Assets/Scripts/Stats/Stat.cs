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

    public void SetBaseValue(float value)
    {
        baseValue = value;
    }

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
        for (int i = (int)StatModifierType.basePercent; i <= (int)StatModifierType.afterPercent; i++)
        {
            foreach (var mod in modifiers)
            {
                if ((int)mod.statType == i)
                {
                    if (mod.statType == StatModifierType.linear)
                    {
                        finalValue += mod.value;
                    }
                    else if (mod.statType == StatModifierType.afterPercent)
                    {
                        finalValue *= mod.value * 0.01f;
                    }
                    else if (mod.statType == StatModifierType.basePercent)
                    {
                        finalValue += baseValue * mod.value * 0.01f;
                    }
                }
            }
        }
        return finalValue;
    }

    public void AddModifier(float modifier, StatModifierType statType)
    {
        modifiers.Add(new StatModifier(modifier, null, statType));
    }

    public void AddModifier(float modifier, object source, StatModifierType statType)
    {
        modifiers.Add(new StatModifier(modifier, source, statType));
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
internal struct StatModifier
{
    public float value;
    public object source;
    public StatModifierType statType;

    public StatModifier(float value, object source, StatModifierType statType)
    {
        this.value = value;
        this.source = source;
        this.statType = statType;
    }
}

public enum StatModifierType
{
    basePercent,
    linear,
    afterPercent,
}