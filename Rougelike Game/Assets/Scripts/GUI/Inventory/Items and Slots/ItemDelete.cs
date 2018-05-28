//When item is placed, will destroy the item
public class ItemDelete : ItemSlot {

    //Handles when an item is dropped upon the slot
    //Does item exchanging, equipping and stuff like that
    public override void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        //If there is an item attached to the mouse pointer
        if (droppedItem && item == null)
        {
            droppedItem.DestroyItem();
        }
    }

}
