using UnityEngine;

public class DungeonUpstairs : PlayerEnterDetector
{
    public override void PlayerEnter()
    {
        DungeonGenerator.Instance.Upstairs();
    }
}
