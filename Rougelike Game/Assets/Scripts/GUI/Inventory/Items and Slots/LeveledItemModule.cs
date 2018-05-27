public class LeveledItemModule : ItemModule
{
    public int level;

    public LeveledItemModule(string title, string sprite, string color, int level) 
        : base(title, sprite, color)
    {
        this.level = level;
    }
}