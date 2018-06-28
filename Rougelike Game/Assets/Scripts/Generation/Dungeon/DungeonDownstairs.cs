/// <summary>
/// The downstairs trigger for the dungeon
/// </summary>
public class DungeonDownstairs : PlayerEnterDetector
{
    public override void PlayerEnter()
    {
        DungeonGenerator.Instance.Downstairs();
    }
}
