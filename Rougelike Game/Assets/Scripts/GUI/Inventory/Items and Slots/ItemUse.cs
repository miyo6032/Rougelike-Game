using UnityEngine;

/// <summary>
/// Sort of like a lookup for a few items that will have item use effects
/// </summary>
public class ItemUse : MonoBehaviour
{
    public PlayerStats player;
    public SkillManager skillManager;

    /// <summary>
    /// Look the items based on its title and apply an existing value
    /// </summary>
    /// <param name="itemInstance"></param>
    public void ApplyItemEffect(ItemInstance itemInstance)
    {
        player.ApplyStats(itemInstance.item.ModifiersAffected);
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
