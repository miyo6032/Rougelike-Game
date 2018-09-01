using UnityEngine;

/// <summary>
/// The trigger for the dungeon Door - just destroys it when walked upon
/// </summary>
public class DungeonDoor : PlayerEnterDetector
{
    public SpriteRenderer spriteRenderer;

    //When the player walks on top of the bag
    public void Open()
    {
        Destroy(gameObject);
    }
}