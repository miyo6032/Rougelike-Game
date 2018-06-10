using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{

    public PlayerStats playerStats;

    public Sprite invisible;

    public Effect TEST;

    public List<TimeAndSprite> ui;

    private List<ActiveEffect> activeEffects;

    public void AddNewEffect(Effect effect)
    {
        if (effect.applyOnce)
        {
            playerStats.ApplyStats(effect.statsAffected.ToArray());
        }
        activeEffects.Add(new ActiveEffect(effect, effect.duration));
    }

    private void Start()
    {
        activeEffects = new List<ActiveEffect>();
        StartCoroutine(ApplyEffects());
        AddNewEffect(TEST);
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
                    playerStats.ApplyStats(effect.effect.statsAffected.ToArray());
                }
                if (effect.currentDuration == 0)
                {
                    toRemove.Add(effect);
                    if (effect.effect.removeAfterDone)
                    {
                        playerStats.ReverseStats(effect.effect.statsAffected.ToArray());
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

    private void UpdateUI()
    {
        for (int i = 0; i < ui.Count; i++)
        {
            if (activeEffects.Count > i)
            {
                ui[i].time.text = activeEffects[i].currentDuration.ToString();
                ui[i].image.sprite = activeEffects[i].effect.sprite;
            }
            else
            {
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
