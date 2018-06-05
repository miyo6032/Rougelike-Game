/// <summary>
/// A piece of an item to be constructed in ItemGenerator
/// </summary>
public class ItemModule
{
    public string Title;
    public string Sprite;
    public string Color;

    public ItemModule(string title, string sprite, string color)
    {
        Title = title;
        Sprite = sprite;
        Color = color;
    }

}
