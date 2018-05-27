using System.Collections.Generic;
using UnityEngine;

//Holds all item textures
public class TextureDatabase : MonoBehaviour {

    Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();

    string[] itemCategories = { "Armor", "Swords", "Helmets" };

    public void LoadAllTextures()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("ItemIcons/Items");
        foreach (Sprite sprite in sprites)
        {
            string path = sprite.ToString();
            path = path.Substring(0, path.IndexOf(" "));
            textures.Add(path, sprite);
        }
        //Load the textures from the item modules
        Dictionary<string, Sprite> itemModuleTextures = LoadModuleTextures();
        foreach (KeyValuePair<string, Sprite> texture in itemModuleTextures)
        {
            textures.Add(texture.Key, texture.Value);
        }
        textures.Add("Invisible", Resources.Load<Sprite>("ItemIcons/Invisible"));
    }

    Dictionary<string, Sprite> LoadModuleTextures()
    {
        Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();
        foreach (string category in itemCategories)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("ItemIcons/" + category);
            foreach (Sprite sprite in sprites)
            {
                string path = sprite.ToString();
                path = path.Substring(0, path.IndexOf(" "));
                textures.Add(path, sprite);
            }
        }
        return textures;
    }

    public Sprite LoadTexture(string texture)
    {
        Sprite sprite;
        textures.TryGetValue(texture, out sprite);
        return sprite;
    }

}
