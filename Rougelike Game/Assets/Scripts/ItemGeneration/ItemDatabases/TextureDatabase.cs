using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all of the item textures for lookup
/// </summary>
public class TextureDatabase : MonoBehaviour
{
    private readonly Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();
    private readonly string[] itemCategories = {"Armor", "Swords", "Helmets"};

    /// <summary>
    /// Load every texture into the database
    /// </summary>
    public void LoadAllTextures()
    {
        // Load all item textures
        Sprite[] sprites = Resources.LoadAll<Sprite>("ItemIcons/Items");
        foreach (Sprite sprite in sprites)
        {
            string path = sprite.ToString();
            path = path.Substring(0, path.IndexOf(" ", StringComparison.Ordinal));
            textures.Add(path, sprite);
        }

        //Load the textures from the item modules
        LoadModuleTextures();
        textures.Add("Invisible", Resources.Load<Sprite>("ItemIcons/Invisible"));
    }

    /// <summary>
    /// Loads textures for item pieces for equipment
    /// </summary>
    private void LoadModuleTextures()
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
