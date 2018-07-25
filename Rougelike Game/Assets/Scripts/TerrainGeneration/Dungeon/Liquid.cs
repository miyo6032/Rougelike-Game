using UnityEngine;

/// <summary>
/// Applies a liquid effect when the player enters
/// </summary>
public class Liquid : PlayerEnterDetector {

    public Effect effect;

    public override void PlayerEnter(Collider2D player)
    {
        EffectManager.instance.AddNewEffect(effect, this);
    }

    public override void PlayerExit(Collider2D player)
    {
        EffectManager.instance.RemoveEffectBySource(this);
    }
}
