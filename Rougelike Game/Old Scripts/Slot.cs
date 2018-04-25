using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Slot <T> : MonoBehaviour
    where T : class
{
    [HideInInspector]
    public ItemInstance item;

    protected PlayerStats playerStat;

    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    public abstract void ItemDrop(ItemInstance droppedItem);

    //Link the item to a slot
    public abstract void LinkItemAndSlot(ItemInstance item, Slot<T> slot);

}
