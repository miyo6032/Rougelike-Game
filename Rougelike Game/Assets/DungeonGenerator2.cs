using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator2 : TerrainGenerator
{
    public int Height;
    public int Width;
    public int InitialRoomDensity;
    public Vector2Int RoomHeightBounds;
    public Vector2Int RoomWidthBounds;
    private Tiles[,] map;
    private Dictionary<Vector2Int, List<Vector2Int>> links;
    private List<Edge> edges;

    public void OnDrawGizmos()
    {
        if (edges == null)
        {
            // We're probably in the editor
            return;
        }

        Gizmos.color = Color.red;
        //foreach (var kv in links)
        //{
        //    foreach (var vertex in kv.Value)
        //    {
        //        Vector3 p0 = new Vector3(vertex.x, vertex.y);
        //        Vector3 p1 = new Vector3(kv.Key.x, kv.Key.y);
        //        Gizmos.DrawLine(p0, p1);
        //    }
        //}
        //foreach (var edge in edges)
        //{
        //    Gizmos.DrawLine((Vector2)edge.v0, (Vector2)edge.v1);
        //}
    }

    public override void Generate()
    {
        InitMap();
        GenerateDungeon(); ;
        MapToTilemap();
    }

    // Initialize, scattering living cells randomly in the map based on wallChance
    void InitMap()
    {
        map = new Tiles[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                map[x, y] = Tiles.voidTile;
            }
        }
    }

    void GenerateDungeon()
    {
        Triangulator triangulator = new Triangulator();
        Dictionary<Vector2Int, Room> rooms = GenerateRooms();
        links = triangulator.GetLinks(rooms.Keys.ToList());
        edges = LinksIntoHallways(rooms);

        foreach (var room in rooms.Values)
        {
            for (int y = 0; y < room.GetHeight(); y++)
            {
                for (int x = 0; x < room.GetWidth(); x++)
                {
                    map[room.lowerLeftCorner.x + x, room.lowerLeftCorner.y + y] = Tiles.floorTile;
                }
            }
        }

        foreach (var edge in edges)
        {
            Vector2 step = edge.v1 - edge.v0;
            step.Normalize();
            Vector2Int start = edge.v0;
            while (start != edge.v1)
            {
                map[start.x, start.y] = Tiles.floorTile;
                start += Vector2Int.FloorToInt(step);
            }
        }
    }

    /// <summary>
    /// Turn linked vertices into hallways depending on the rooms
    /// </summary>
    List<Edge> LinksIntoHallways(Dictionary<Vector2Int, Room> rooms)
    {
        List<Edge> edges = new List<Edge>();
        RemoveDuplicates();
        foreach (var kV in links)
        {
            foreach (var vector in kV.Value)
            {
                Edge xEdge = new Edge(kV.Key, new Vector2Int(vector.x, kV.Key.y));
                Edge yEdge = new Edge(new Vector2Int(vector.x, kV.Key.y), vector);
                edges.Add(yEdge);
                edges.Add(xEdge);
            }
        }

        return edges;
    }

    void RemoveDuplicates()
    {
        foreach (var kV in links)
        {
            foreach (var vector in kV.Value)
            {
                if (links[vector].Contains(kV.Key))
                {
                    links[vector].Remove(kV.Key);
                }
            }
        }
    }

    /// <summary>
    /// If the rooms can be connected by a straight hallway, without any turns
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    bool RoomsAreAligned(Room room1, Room room2)
    {
        if (room1.lowerLeftCorner.x > room2.upperRightCorner.x && room1.lowerLeftCorner.y > room2.lowerLeftCorner.x)
        {
            return false;
        }

        if (room2.lowerLeftCorner.x > room1.upperRightCorner.x && room2.lowerLeftCorner.y > room1.lowerLeftCorner.x)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Generate a bunch of randomly size rooms in the map
    /// </summary>
    /// <returns></returns>
    Dictionary<Vector2Int, Room> GenerateRooms()
    {
        Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
        for (int i = 0; i < InitialRoomDensity; i++)
        {
            int roomHeight = Random.Range(RoomHeightBounds.x, RoomHeightBounds.y);
            int roomWidth = Random.Range(RoomWidthBounds.x, RoomWidthBounds.y);
            Vector2Int lowerLeftCorner = new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
            Vector2Int upperRightCorner = lowerLeftCorner + new Vector2Int(roomWidth, roomHeight);

            Room room = new Room(lowerLeftCorner, upperRightCorner);
            if (CanPlaceRoom(room, rooms.Values.ToList()))
            {
                rooms.Add(room.GetCenter(), room);
            }
        }

        return rooms;
    }

    bool CanPlaceRoom(Room room, List<Room> rooms)
    {
        if (!RoomInBounds(room)) return false;

        foreach (var r in rooms)
        {
            if (RoomsOverlap(room, r))
            {
                return false;
            }
        }

        return true;
    }

    bool RoomInBounds(Room room)
    {
        if (room.lowerLeftCorner.x + room.GetWidth() >= Width)
        {
            return false;
        }

        if (room.lowerLeftCorner.y + room.GetHeight() >= Height)
        {
            return false;
        }

        return true;
    }

    bool RoomsOverlap(Room room1, Room room2)
    {
        if (room1.lowerLeftCorner.y > room2.upperRightCorner.y || room2.lowerLeftCorner.y > room1.upperRightCorner.y)
        {
            return false;
        }

        if (room1.lowerLeftCorner.x > room2.upperRightCorner.x || room2.lowerLeftCorner.x > room1.upperRightCorner.x)
        {
            return false;
        }

        return true;
    }

    // Converts our integer 2d array into the tilemap!
    void MapToTilemap()
    {
        floor.ClearAllTiles();
        walls.ClearAllTiles();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (map[x, y] == Tiles.floorTile)// 1 means a floor tile
                {
                    // Offset to keep the tilemap at the expected position
                    floor.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
                //If there is a floor (no wall) below
                else if (map[x, y] == Tiles.wallTile)
                {
                    if (y > 0 && map[x, y - 1] == Tiles.floorTile)
                    {
                        walls.SetTile(new Vector3Int(x, y, 0), freeStandingWallTile);
                    }
                    else
                    {
                        walls.SetTile(new Vector3Int(x, y, 0), wallTile);
                    }
                }
                else if (map[x, y] == Tiles.voidTile)
                {
                    walls.SetTile(new Vector3Int(x, y, 0), voidTile);
                }
            }
        }
    }

    class Edge
    {
        public Vector2Int v0;
        public Vector2Int v1;

        public Edge(Vector2Int v0, Vector2Int v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
    }
}
