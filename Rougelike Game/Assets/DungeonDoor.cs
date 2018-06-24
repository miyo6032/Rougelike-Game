using UnityEngine;
using UnityEngine.EventSystems;

public class DungeonDoor : PlayerEnterDetector, IPointerClickHandler
{
    public SpriteRenderer spriteRenderer;

    //When the player walks on top of the bag
    public override void PlayerEnter()
    {
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerEnter();
    }
}
