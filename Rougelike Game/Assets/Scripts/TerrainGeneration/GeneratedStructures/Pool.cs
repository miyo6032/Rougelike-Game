using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generate a pool shaped object in a room
/// </summary>
[CreateAssetMenu(menuName = "Custom/Generated Structure/Pool")]
public class Pool : GeneratedStructure
{
    public SpriteRenderer holePrefab;
    public Sprite hole;
    public Sprite holeEdge;
    public Effect effect;

    public override bool CanGenerate(Tilemap walls, Tilemap upperFloor, Tilemap ceiling, Room room)
    {
        return !room.SpotTaken(room.GetCenter());
    }

    public override void Generate(Tilemap walls, Tilemap upperFloor, Tilemap ceiling, Room room)
    {
        int roomWidth2 = Mathf.FloorToInt(room.GetWidth() / 2f) - 1;
        int roomHeight2 = Mathf.FloorToInt(room.GetHeight() / 2f) - 1;

        List<Vector2Int> tiles = new List<Vector2Int>();

        for (int x = -roomWidth2; x < roomWidth2; x++)
        {
            for (int y = -roomHeight2; y < roomHeight2; y++)
            {
                Vector2Int pos = new Vector2Int(x + room.GetCenter().x, y + room.GetCenter().y);
                if (x != roomWidth2 && x != -roomWidth2 && y != roomHeight2 && y != -roomHeight2)
                {
                    room.ClaimRoomSpot(pos);
                    tiles.Add(pos);
                }
                else if (Random.Range(0, 2) == 0)
                {
                    room.ClaimRoomSpot(pos);
                    tiles.Add(pos);
                }
            }
        }

        foreach (Vector2Int tile in tiles)
        {
            SpriteRenderer instance = Instantiate(holePrefab, (Vector2)tile, Quaternion.identity, DungeonLevelGenerator.instance.mapGameObjects);
            instance.sprite = (tiles.Contains(tile + Vector2Int.up)) ? hole : holeEdge;
            if (Lighting.instance.LightingType == Lighting.LightType.smooth)
            {
                instance.material = Lighting.instance.SmoothLighting;
            }
            Liquid liquid = holePrefab.GetComponent<Liquid>();
            if (liquid)
            {
                liquid.effect = effect;
            }
        }
    }
}