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
    /// <param name="itemSlot"></param>
    public void ApplyItemEffect(ItemSlot itemSlot)
    {
        foreach (Effect effect in itemSlot.itemStack.item.ConsumptionEffects)
        {
            effectManager.AddNewEffect(effect);
        }
        if (itemSlot.itemStack.item.Title == "Critical Hit")
        {
            skillManager.DoTheSkill(Skills.CriticalHit, itemSlot.itemStack.item.focusConsumption);
        }
        if (itemSlot.itemStack.item.Consumable)
        {
            itemSlot.ChangeAmount(-1);
        }
    }
}
