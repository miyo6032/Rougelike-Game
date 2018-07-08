using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Custom/Item")]
public class ItemScriptableObject : ScriptableObject
{

	public Item Item;

    public Sprite[] ItemSprites;

    public Color EquipColor;

    public void Start()
    {
        if (ItemSprites.Length > 0)
        {
            Item.Sprites = new string[Mathf.Min(ItemSprites.Length, 3)];
            for (int i = 0; i < ItemSprites.Length && i < 3; i++)
            {
                string path = ItemSprites[i].ToString();
                path = path.Substring(0, path.IndexOf(" ", StringComparison.Ordinal));
                Item.Sprites[i] = path;
            }
        }
        Item.ItemColor = "#" + ColorUtility.ToHtmlStringRGBA(EquipColor);
    }

}
