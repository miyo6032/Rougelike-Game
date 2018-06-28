/// <summary>
/// The upstairs trigger for a dungeon
/// </summary>
public class DungeonUpstairs : PlayerEnterDetector
{
    public override void PlayerEnter()
    {
        DungeonGenerator.Instance.Upstairs();
    }
}
