using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages effects the player has
/// </summary>
public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public Sprite invisible;

    public List<TimeAndSprite> ui;

    private EffectTooltipShow[] effectInstances;

    private List<ActiveEffect> activeEffects;

    /// <summary>
    /// Adds a new effect to the list, and will become active next iteration in ApplyEffects()
    /// </summary>
    public void AddNewEffect(Effect effect, object source)
    {
        ActiveEffect newEffect = new ActiveEffect(effect, effect.duration, source);
        PlayerStats.instance.ApplyStats(effect.ModifiersAffected, newEffect);
        activeEffects.Add(newEffect);
        UpdateUI();
    }

    /// <summary>
    /// Removes all effects from a source
    /// </summary>
    public void RemoveEffectBySource(object source)
    {
        List<ActiveEffect> toRemove = new List<ActiveEffect>();
        foreach(var effect in activeEffects)
        {
            if (effect.source == source)
            {
                effect.currentDuration = 0;
                PlayerStats.instance.RemoveStats(effect.effect.ModifiersAffected, effect);
                toRemove.Add(effect);
            }
        }
        foreach (var effect in toRemove)
        {
            activeEffects.Remove(effect);
        }
    }

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance  != this)
        {
            Debug.LogError("Duplicate Effect Managers!");
        }

        effectInstances = GetComponentsInChildren<EffectTooltipShow>();
        activeEffects = new List<ActiveEffect>();
        StartCoroutine(ApplyEffects());
    }

    /// <summary>
    /// Apply the list of active effects every second
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyEffects()
    {
        while (PlayerStats.instance)
        {
            List<ActiveEffect> toRemove = new List<ActiveEffect>();
            foreach (ActiveEffect effect in activeEffects)
            {
                effect.currentDuration--;
                if (!effect.effect.applyOnce)
                {
                    PlayerStats.instance.ApplyStats(effect.effect.ModifiersAffected, effect);
                }
                if (effect.currentDuration == 0 && !effect.effect.isPermanent)
                {
                    toRemove.Add(effect);
                    if (effect.effect.removeAfterDone)
                    {
                        PlayerStats.instance.RemoveStats(effect.effect.ModifiersAffected, effect);
                    }
                }
            }

            foreach (var effect in toRemove)
            {
                activeEffects.Remove(effect);
            }

            UpdateUI();

            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// Show the effects in the in game ui
    /// </summary>
    private void UpdateUI()
    {
        for (int i = 0; i < ui.Count; i++)
        {
            if (activeEffects.Count > i)
            {
                ui[i].time.text = activeEffects[i].effect.isPermanent ? "" : activeEffects[i].currentDuration.ToString();
                ui[i].image.sprite = activeEffects[i].effect.sprite;
                effectInstances[i].SetEffect(activeEffects[i].effect);
            }
            else
            {
                effectInstances[i].SetEffect(null);
                ui[i].time.text = "";
                ui[i].image.sprite = invisible;
            }
        }
    }

    private class ActiveEffect
    {
        public readonly Effect effect;
        public int currentDuration;
        public object source;

        public ActiveEffect(Effect effect, int currentDuration, object source)
        {
            this.effect = effect;
            this.currentDuration = currentDuration;
            this.source = source;
        }
    }

    [System.Serializable]
    public class TimeAndSprite
    {
        public Text time;
        public Image image;
    }
}
