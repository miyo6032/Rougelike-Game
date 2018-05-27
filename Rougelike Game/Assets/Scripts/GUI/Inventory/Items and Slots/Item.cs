[System.Serializable]
public class Item
{
    public string Title { get; set; }
    public int Value { get; set; }
    public int Attack { get; set; }
    public int MaxAttack { get; set; }
    public int Defence { get; set; }
    public string Description { get; set; }
    public bool Stackable { get; set; }
    public string[] Sprites { get; set; }
    public int EquippedSlot { get; set; }
    public int ItemLevel { get; set; }
    public string ItemColor { get; set; }

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, int equippedSlot, int itemLevel, string[] sprites, string itemColor)
    {
        this.Title = title;
        this.Value = value;
        this.Attack = attack;
        this.MaxAttack = maxAttack;
        this.Defence = defence;
        this.Description = description;
        this.Stackable = stackable;
        this.EquippedSlot = equippedSlot;
        this.Sprites = sprites;
        this.ItemLevel = itemLevel;
        this.ItemColor = itemColor;
    }

    public Item() { }
}