using UnityEngine.Tilemaps;
using UnityEngine;

/// <summary>
/// Base class for small generate structures in rooms
/// </summary>
public abstract class GeneratedStructure : ScriptableObject{

    public abstract bool CanGenerate(Tilemap walls, Tilemap upperFloor, Room room);
    public abstract void Generate(Tilemap walls, Tilemap upperFloor, Room room);
}

[System.Serializable]
public class WeightedGeneratedStructure
{
    public GeneratedStructure structure;
    [Range(1, 100)]
    public int amountPerLevel = 1;
}