using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The trigger for the dungeon door - just destroys it when walked upon
/// </summary>
public class DungeonDoor : PlayerEnterDetector, IPointerClickHandler
{
    public SpriteRenderer spriteRenderer;

    //When the player walks on top of the bag
    public override void PlayerEnter()
    {
        Destroy(gameObject);
    }

    // Not sure if this works right now
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerEnter();
    }
}
