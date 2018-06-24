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
            Room room = GenerateRoom(new Vector2Int(Random.Range(0, width), Random.Range(0, height)), roomHeightBounds,
                roomWidthBounds);
            if (CanPlaceRoom(room, rooms.Values.ToList(), height, width))
            {
                rooms.Add(room.GetCenter(), room);
            }
        }

        return rooms;
    }

    /// <summary>
    /// Generate a single room with a random position and size
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="roomHeightBounds"></param>
    /// <param name="roomWidthBounds"></param>
    /// <returns></returns>
    Room GenerateRoom(Vector2Int pos, Vector2Int roomHeightBounds, Vector2Int roomWidthBounds)
    {
        int roomHeight = Random.Range(roomHeightBounds.x, roomHeightBounds.y);
        int roomWidth = Random.Range(roomWidthBounds.x, roomWidthBounds.y);
        Vector2Int upperRightCorner = pos + new Vector2Int(roomWidth, roomHeight);
        return new Room(pos, upperRightCorner);
    }

    /// <summary>
    /// Determines whether or not the room will fit with the other existing rooms
    /// </summary>
    /// <param name="room"></param>
    /// <param name="rooms"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    bool CanPlaceRoom(Room room, List<Room> rooms, int height, int width)
    {
        if (!RoomInBounds(room, height, width)) return false;
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
    /// Determines whether or not the room is within the map width and height
    /// </summary>
    /// <param name="room"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    bool RoomInBounds(Room room, int height, int width)
    {
        if (room.lowerLeftCorner.x < 1 || room.lowerLeftCorner.y < 1)
        {
            return false;
        }

        if (room.upperRightCorner.x >= width - 1 || room.upperRightCorner.y >= height - 1)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if two rooms overlap or not
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    bool RoomsOverlap(Room room1, Room room2)
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
