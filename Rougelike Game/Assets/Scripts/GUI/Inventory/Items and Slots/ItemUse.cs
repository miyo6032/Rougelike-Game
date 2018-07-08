using UnityEngine;

/// <summary>
/// Sort of like a lookup for a few items that will have item use effects
/// </summary>
public class ItemUse : MonoBehaviour
{
    public EffectManager effectManager;
    public SkillManager skillManager;

    /// <summary>
    /// Look the items based on its title and apply an existing value
    /// </summary>
    /// <param name="itemInstance"></param>
    public void ApplyItemEffect(ItemInstance itemInstance)
    {
        foreach (Effect effect in itemInstance.item.ConsumptionEffects)
        {
            effectManager.AddNewEffect(effect);
        }
        if (itemInstance.item.Consumable)
        {
            itemInstance.ChangeAmount(-1);
        }

        if (itemInstance.item.Title == "Critical Hit")
        {
            skillManager.DoTheSkill(Skills.CriticalHit, itemInstance.item.focusConsumption);
        }
    }
}
