using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generate webs in four corners of a room
/// </summary>
[CreateAssetMenu(menuName = "Custom/Generated Structure/Web")]
public class CornerWebs : GeneratedStructure
{

    public Tile upLeftWeb;
    public Tile upRightWeb;
    public Tile downLeftWeb;
    public Tile downRightWeb;

    public override void Generate(Tilemap walls, Tilemap upperFloor, Room room)
    {
        if (Random.Range(0, 2) == 1) upperFloor.SetTile(Vector3Int.RoundToInt((Vector2)room.lowerLeftCorner), downLeftWeb);
        if (Random.Range(0, 2) == 1) upperFloor.SetTile(Vector3Int.RoundToInt(room.lowerLeftCorner + new Vector2(0, room.GetHeight() - 1)), upLeftWeb);
        if (Random.Range(0, 2) == 1) upperFloor.SetTile(Vector3Int.RoundToInt(room.lowerLeftCorner + new Vector2(room.GetWidth() - 1, 0)), downRightWeb);
        if (Random.Range(0, 2) == 1) upperFloor.SetTile(Vector3Int.RoundToInt((Vector2)room.upperRightCorner), upRightWeb);
    }

    // Always try to generate the webs
    public override bool CanGenerate(Tilemap walls, Tilemap upperFloor, Room room)
    {
        return true;
    }
}
