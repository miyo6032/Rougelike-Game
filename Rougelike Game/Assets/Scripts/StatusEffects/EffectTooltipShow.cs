using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Show the effect stats when hovered over in the in game ui
/// </summary>
public class EffectTooltipShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Effect effect;

    public void SetEffect(Effect effect)
    {
        this.effect = effect;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (effect != null)
        {
            Tooltip.instance.ShowEffectTooltip(effect);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.gameObject.SetActive(false);
    }
}
