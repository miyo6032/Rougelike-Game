/// <summary>
/// A piece of an item to be constructed in ItemGenerator
/// </summary>
public class ItemModule
{
    public string Title;
    public string Sprite;
    public string Color;
    public int MinAttack;
    public int MaxAttack;
    public int Defense;

    public ItemModule(string title, string sprite, string color)
    {
        Title = title;
        Sprite = sprite;
        Color = color;
    }

    public ItemModule(string title, string sprite, string color, int minAttack, int maxAttack, int defense)
    {
        Title = title;
        Sprite = sprite;
        Color = color;
        MinAttack = minAttack;
        Defense = defense;
        MaxAttack = maxAttack;
    }

}
