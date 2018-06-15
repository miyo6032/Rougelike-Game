using UnityEngine;

/// <summary>
/// Sort of like a lookup for a few items that will have item use effects
/// </summary>
public class ItemUse : MonoBehaviour
{
    public PlayerStats player;

    /// <summary>
    /// Look the items based on its title and apply an existing value
    /// </summary>
    /// <param name="itemInstance"></param>
    public void ApplyItemEffect(ItemInstance itemInstance)
    {
        if (itemInstance.item.Title == "Minor Health Potion")
        {
            HealPlayer(0.2f);
            itemInstance.ChangeAmount(-1);
        }
    }

    void HealPlayer(float percentage)
    {
        int healAmount = Mathf.CeilToInt(player.maxHealth.GetValue() * percentage);
        player.Heal(healAmount);
    }
}
