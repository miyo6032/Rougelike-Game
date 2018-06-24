using UnityEngine;

public class DungeonDownstairs : PlayerEnterDetector
{
    public override void PlayerEnter()
    {
        DungeonGenerator.Instance.Downstairs();
    }
}
