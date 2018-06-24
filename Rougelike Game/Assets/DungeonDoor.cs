using UnityEngine;

public class DungeonDoor : PlayerEnterDetector
{
    public Sprite openSprite;
    public SpriteRenderer spriteRenderer;

    //When the player walks on top of the bag
    public override void PlayerEnter()
    {
        spriteRenderer.sprite = openSprite;
    }
}
