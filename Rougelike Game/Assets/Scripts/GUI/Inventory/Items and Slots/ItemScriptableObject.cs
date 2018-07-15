using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Custom/Item")]
public class ItemScriptableObject : ScriptableObject
{

	public Item item;

    public Sprite itemSprite;

    public Color equipColor = Color.gray;

    public void Start()
    {
        string path = itemSprite.ToString();
        int index = path.IndexOf(" ", StringComparison.Ordinal);
        if (index < 0)
        {
            item.Sprite = "invisible";
        }
        else {
            path = path.Substring(0, index);
            item.Sprite = path;
        }
        item.ItemColor = "#" + ColorUtility.ToHtmlStringRGBA(equipColor);
    }

}
