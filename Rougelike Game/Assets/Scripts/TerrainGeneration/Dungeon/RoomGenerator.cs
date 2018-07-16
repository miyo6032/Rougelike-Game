using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates simple rooms for DungeonLevel Generator and makes sure they don't overlap
/// </summary>
public class RoomGenerator
{
    /// <summary>
    /// Generate a bunch of randomly size rooms in the map
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector2Int, Room> GenerateRooms(int numRooms, int width, int height, Vector2Int roomHeightBounds,
        Vector2Int roomWidthBounds)
    {
        Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
        for (int i = 0; i < numRooms; i++)
        {
            int roomHeight = Random.Range(roomHeightBounds.x - 1, roomHeightBounds.y - 1);
            int roomWidth = Random.Range(roomWidthBounds.x - 1, roomWidthBounds.y - 1);
            Vector2Int randomPos = new Vector2Int(Random.Range(1, width - roomWidth - 1), Random.Range(1, height - roomHeight - 1));
            Vector2Int upperRightCorner = randomPos + new Vector2Int(roomWidth, roomHeight);
            Room room = new Room(randomPos, upperRightCorner);
            if (CanPlaceRoom(room, rooms.Values.ToList(), height, width))
            {
                rooms.Add(room.GetCenter(), room);
            }
        }

        return rooms;
    }

    /// <summary>
    /// Determines whether or not the room will fit with the other existing rooms
    /// </summary>
    /// <param name="room"></param>
    /// <param name="rooms"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private bool CanPlaceRoom(Room room, List<Room> rooms, int height, int width)
    {
        foreach (var r in rooms)
        {
            if (RoomsOverlap(room, r))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if two rooms overlap or not
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    private bool RoomsOverlap(Room room1, Room room2)
    {
        if (room1.lowerLeftCorner.y > room2.upperRightCorner.y + 1 ||
            room2.lowerLeftCorner.y - 1 > room1.upperRightCorner.y)
        {
            return false;
        }

        if (room1.lowerLeftCorner.x > room2.upperRightCorner.x + 1 ||
            room2.lowerLeftCorner.x - 1 > room1.upperRightCorner.x)
        {
            return false;
        }

        return true;
    }
}
