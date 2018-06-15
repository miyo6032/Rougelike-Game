using System.Collections.Generic;
using UnityEngine;

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