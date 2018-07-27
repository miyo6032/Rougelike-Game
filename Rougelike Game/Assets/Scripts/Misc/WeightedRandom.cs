using UnityEngine.Tilemaps;
using UnityEngine;

/// <summary>
/// Weighted classes to easily add weighted randomness to object spawning
/// </summary>
[System.Serializable]
public class WeightedTile
{
    public Tile tile;
    [Range(1, 100)]
    public float weight = 1;
}

[System.Serializable]
public class TileType
{
    public WeightedTile[] tiles;

    public Tile GetTile()
    {
        float[] probs = new float[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            probs[i] = tiles[i].weight;
        }
        return tiles[HelperScripts.GetWeigtedIndex(probs)].tile;
    }
}

[System.Serializable]
public class WeightedEnemy
{
    public Enemy enemy;
    [Range(1, 100)]
    public int weight = 1;
}

[System.Serializable]
public class EnemyType
{
    public WeightedEnemy[] enemies;

    public Enemy GetEnemy()
    {
        float[] probs = new float[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            probs[i] = enemies[i].weight;
        }
        return enemies[HelperScripts.GetWeigtedIndex(probs)].enemy;
    }
}

[System.Serializable]
public class WeightedDestructible
{
    public DestructibleScriptableObject destructible;
    [Range(1, 100)]
    public int weight = 1;
}

[System.Serializable]
public class DestructibleType
{
    public WeightedDestructible[] destructibles;

    public DestructibleScriptableObject GetDestructible()
    {
        float[] probs = new float[destructibles.Length];
        for (int i = 0; i < destructibles.Length; i++)
        {
            probs[i] = destructibles[i].weight;
        }
        return destructibles[HelperScripts.GetWeigtedIndex(probs)].destructible;
    }
}