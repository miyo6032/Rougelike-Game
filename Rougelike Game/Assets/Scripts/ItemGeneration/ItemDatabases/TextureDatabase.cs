using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all of the item textures for lookup
/// </summary>
public class TextureDatabase : MonoBehaviour
{
    public static TextureDatabase instance;
    private readonly Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();
    private readonly string[] itemCategories = {"Armor", "Swords", "Helmets", "Items", "Skills"};

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Load every texture into the database
    /// </summary>
    public void LoadAllTextures()
    {
        foreach (string category in itemCategories)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("ItemIcons/" + category);
            foreach (Sprite sprite in sprites)
            {
                string path = sprite.ToString();
                path = path.Substring(0, path.IndexOf(" ", StringComparison.Ordinal));
                textures.Add(path, sprite);
            }
        }

        textures.Add("Invisible", Resources.Load<Sprite>("ItemIcons/Invisible"));
    }

    /// <summary>
    /// Load a texture based on its resource name
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public Sprite LoadTexture(string texture)
    {
        Sprite sprite;
        textures.TryGetValue(texture, out sprite);
        return sprite;
    }
}
