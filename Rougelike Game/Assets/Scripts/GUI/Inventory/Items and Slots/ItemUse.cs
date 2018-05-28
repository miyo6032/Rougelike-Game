using UnityEngine;

//Sort of like a lookup for a few items that will have item use effects
public class ItemUse : MonoBehaviour {

    public PlayerStats player;

    public void ApplyItemEffect(ItemInstance itemInstance)
    {
        if(itemInstance.item.Title == "Minor Health Potion")
        {
            HealPlayer(0.2f);
            itemInstance.UseItem();
        }
    }

    void HealPlayer(float percentage)
    {
        int healAmount = Mathf.CeilToInt(player.maxHealth * percentage);
        player.Heal(healAmount);
    }

}
