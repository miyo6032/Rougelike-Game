﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages effects the player has
/// </summary>
public class EffectManager : MonoBehaviour
{
    public PlayerStats playerStats;

    public Sprite invisible;

    public List<TimeAndSprite> ui;

    private EffectTooltipShow[] effectInstances;

    private List<ActiveEffect> activeEffects;

    /// <summary>
    /// Adds a new effect to the list, and will become active next iteration in ApplyEffects()
    /// </summary>
    /// <param name="effect"></param>
    public void AddNewEffect(Effect effect)
    {
        if (effect.applyOnce)
        {
            playerStats.ApplyStats(effect.ModifiersAffected);
        }
        activeEffects.Add(new ActiveEffect(effect, effect.duration));
    }

    private void Start()
    {
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
        while (playerStats)
        {
            List<ActiveEffect> toRemove = new List<ActiveEffect>();
            foreach (ActiveEffect effect in activeEffects)
            {
                effect.currentDuration--;
                if (!effect.effect.applyOnce)
                {
                    playerStats.ApplyStats(effect.effect.ModifiersAffected);
                }
                if (effect.currentDuration == 0)
                {
                    toRemove.Add(effect);
                    if (effect.effect.removeAfterDone)
                    {
                        playerStats.ReverseStats(effect.effect.ModifiersAffected);
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
                ui[i].time.text = activeEffects[i].currentDuration.ToString();
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

        public ActiveEffect(Effect effect, int currentDuration)
        {
            this.effect = effect;
            this.currentDuration = currentDuration;
        }
    }

    [System.Serializable]
    public class TimeAndSprite
    {
        public Text time;
        public Image image;
    }
}
