using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds room data to make DungeonLevel generation more managable
/// </summary>
public class Room
{
    public Vector2Int upperRightCorner;
    public Vector2Int lowerLeftCorner;

    private int objectCount;
    private readonly List<Vector2Int> takenspots = new List<Vector2Int>();

    public Room(Vector2Int lowerLeftCorner, Vector2Int upperRightCorner)
    {
        this.upperRightCorner = upperRightCorner;
        this.lowerLeftCorner = lowerLeftCorner;
    }

    public Vector2Int GetCenter()
    {
        return lowerLeftCorner + Vector2Int.FloorToInt(new Vector2(GetWidth() / 2f, GetHeight() / 2f));
    }

    public int GetHeight()
    {
        return Mathf.Abs(upperRightCorner.y - lowerLeftCorner.y) + 1;
    }

    public int GetWidth()
    {
        return Mathf.Abs(upperRightCorner.x - lowerLeftCorner.x) + 1;
    }

    public bool SpotTaken(Vector2Int vector2Int)
    {
        // Too many object for this room, do not allow any more
        if (objectCount / 3 > GetHeight() * GetWidth()) return true;

        return takenspots.Contains(vector2Int);
    }

    public void ClaimRoomSpot(Vector2Int vector2Int)
    {
        objectCount++;
        takenspots.Add(vector2Int);
    }
}
