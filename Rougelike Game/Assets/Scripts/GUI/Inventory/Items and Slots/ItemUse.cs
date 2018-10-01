using UnityEngine;

/// <summary>
/// Sort of like a lookup for a few items that will have item use effects
/// </summary>
public class ItemUse : MonoBehaviour
{
    public SkillManager skillManager;

    /// <summary>
    /// Look the items based on its title and apply an existing value
    /// </summary>
    /// <param name="itemSlot"></param>
    public void ApplyItemEffect(ItemSlot itemSlot)
    {
        foreach (Effect effect in itemSlot.itemStack.item.GetConsumptionEffects())
        {
            EffectManager.instance.AddNewEffect(effect, null);
        }
        if (itemSlot.itemStack.item.GetTitle() == "Critical Hit")
        {
            skillManager.DoTheSkill(Skills.CriticalHit, itemSlot.itemStack.item.GetFocusConsumption());
        }
        if (itemSlot.itemStack.item.GetIsConsumable())
        {
            itemSlot.ChangeAmount(-1);
        }
    }
}
