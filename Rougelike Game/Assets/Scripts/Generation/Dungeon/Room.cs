using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds room data to make DungeonLevel generation more managable
/// </summary>
public class Room
{
    public Vector2Int upperRightCorner;
    public Vector2Int lowerLeftCorner;

    List<Vector2Int> takenspots = new List<Vector2Int>();

    public Room(Vector2Int lowerLeftCorner, Vector2Int upperRightCorner)
    {
        this.upperRightCorner = upperRightCorner;
        this.lowerLeftCorner = lowerLeftCorner;
    }

    public Vector2Int GetCenter()
    {
        return lowerLeftCorner + Vector2Int.CeilToInt(new Vector2(GetWidth() / 2f, GetHeight() / 2f));
    }

    public int GetHeight()
    {
        return Mathf.Abs(upperRightCorner.y - lowerLeftCorner.y);
    }

    public int GetWidth()
    {
        return Mathf.Abs(upperRightCorner.x - lowerLeftCorner.x);
    }

    public bool SpotTaken(Vector2Int vector2Int)
    {
        return takenspots.Contains(vector2Int);
    }

    public void ClaimRoomSpot(Vector2Int vector2Int)
    {
        takenspots.Add(vector2Int);
    }
}
