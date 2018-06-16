using UnityEngine;
using System.Collections.Generic;
using TriangleNet.Geometry;

public class DungeonGenerator : TerrainGenerator
{
    public int Height;
    public int Width;
    public int InitialRoomDensity;
    public Vector2Int RoomHeightBounds;
    public Vector2Int RoomWidthBounds;
    private Tiles[,] map;
    public TEST mst;

    public override void Generate()
    {
        InitMap();
        GenerateDungeon();;
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
        List<Room> rooms = GenerateRooms();
        List<Vertex> vertices = new List<Vertex>();
        rooms.ForEach((room) => vertices.Add(new Vertex(room.GetCenter().x, room.GetCenter().y)));
        mst.Generate(vertices);

        foreach (var room in rooms)
        {
            for (int y = 0; y < room.GetHeight(); y++)
            {
                for (int x = 0; x < room.GetWidth(); x++)
                {
                    map[room.lowerLeftCorner.x + x, room.lowerLeftCorner.y + y] = Tiles.floorTile;
                }
            }
        }
    }

    List<Room> GenerateRooms()
    {
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < InitialRoomDensity; i++)
        {
            int roomHeight = Random.Range(RoomHeightBounds.x, RoomHeightBounds.y);
            int roomWidth = Random.Range(RoomWidthBounds.x, RoomWidthBounds.y);
            Vector2Int lowerLeftCorner = new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
            Vector2Int upperRightCorner = lowerLeftCorner + new Vector2Int(roomWidth, roomHeight);

            Room room = new Room(lowerLeftCorner, upperRightCorner);
            if (CanPlaceRoom(room, rooms))
            {
                rooms.Add(room);
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
                else if(map[x, y] == Tiles.voidTile)
                {
                    walls.SetTile(new Vector3Int(x, y, 0), voidTile);
                }
            }
        }
    }

    class Room
    {
        public Vector2Int upperRightCorner;
        public Vector2Int lowerLeftCorner;

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
    }

}
