using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Custom/Item")]
public class ItemScriptableObject : ScriptableObject
{
	public Item item;

    public Sprite itemSprite;

    public Color equipColor = Color.gray;

    /// <summary>
    /// Automatically update the color and item texture in string mode so the engine can load the textures and colors 
    /// </summary>
    public void OnValidate()
    {
        string path = itemSprite.ToString();
        int index = path.IndexOf(" ", StringComparison.Ordinal);
        if (index < 0)
        {
            item.Sprite = "invisible";
        }
        else
        {
            path = path.Substring(0, index);
            item.Sprite = path;
        }
        item.ItemColor = "#" + ColorUtility.ToHtmlStringRGBA(equipColor);
    }

}
