using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Delete : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	
	public int id;
	private Inventory inv;
	private ShowTooltip tooltip;
	private PlayerStat playerStat;
	
	void Start(){
		inv = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
		tooltip = inv.GetComponent<ShowTooltip> ();
		playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
	}
	
	public void OnDrop(PointerEventData eventData){
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData> ();

        if(droppedItem.item.Id < 0)
        {
            return;
        }

		if (droppedItem.equipped) {
			inv.items [droppedItem.slot] = new Item ();
			droppedItem.equipped = false;
			playerStat.equippedItems [droppedItem.item.EquippedSlot] = new Item();
			playerStat.UpdateGearStats();
            if (droppedItem.item.Skill != 0)
            {
                ItemData itemd = inv.GetItemdata(droppedItem.item.Skill);
                inv.items[itemd.slot] = new Item();
                Debug.Log("destroyed");
                GameObject.Destroy(itemd.gameObject);
            }
        } else {
			inv.items [droppedItem.slot] = new Item ();
		}
		GameObject.Destroy (droppedItem.gameObject);
		
	}

	public void OnPointerEnter(PointerEventData eventData){
		tooltip.ActivateDelete ();
	}
	
	public void OnPointerExit(PointerEventData eventData){
		tooltip.Deactivate ();
	}
	
}
