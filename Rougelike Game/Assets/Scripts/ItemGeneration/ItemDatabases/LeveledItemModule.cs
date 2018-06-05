/// <summary>
/// An item piece that has strength based on its level
/// </summary>
public class LeveledItemModule : ItemModule
{
    public int level;

    public LeveledItemModule(string title, string sprite, string color, int level) : base(title, sprite, color)
    {
        this.level = level;
    }
}