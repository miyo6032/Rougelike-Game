using UnityEngine;

//NOT IN USE
// Procedurally generates the tilemap using cellular automata and a lot of tweaking variables
public class MapGenerator : TerrainGenerator
{
    public int initialHeight; // The inital dimensions - the dimensions will grow with more cycles
    public int initialWidth; 

    private int height; // The current dimensions
    private int width;

    public int detectRange; // How far every cell should look around itself to find neighbors

    [Range(0, 1)]
    public float wallChance; // The initial chance for each cell to be a wall

    public int deathLimit; // If the number of neighbors drops below this limit, then the cell will 'die' (go to 0)
    public int birthLimit; // If the number of neighbors drops above this limit, then a new cell can 'grow' there (go to 1)

    public int cycles; // Number of evolutionary cycles to do

    private Tiles[,] map; // The integer map that stores the cell data

    // Called to generate the entire map from start to finish
    public override void Generate()
    {
        InitMap();
        for(int i = 0; i < cycles; i++)
        {
            Cycle();
        }
    }

    // Initialize, scattering living cells randomly in the map based on wallChance
    private void InitMap()
    {
        map = new Tiles[initialHeight, initialWidth];
        height = initialHeight;
        width = initialWidth;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < height; x++)
            {
                if(Random.Range(0f, 1f) < wallChance)
                {
                    map[y, x] = Tiles.wallTile;// Cell does not exist
                }
                else
                {
                    map[y, x] = Tiles.floorTile;// Cell does exist
                }
            }
        }
    }

    // Perform a cycle which alters cells according to their neighbor count
    private void Cycle()
    {
        // We want the cells to potentially grow outside of their range, so we will allocate a slightly bigger array to do that.
        height = height + (detectRange * 2);
        width = width + (detectRange * 2);

        Tiles[,] newMap = new Tiles[height, width];

        // Offset a little bit because the array is a little bigger
        for (int y = -detectRange; y < height - detectRange; y++)
        {
            for (int x = -detectRange; x < width - detectRange; x++)
            {
                // Counts the neighbors
                int neighborCount = 0;
                for (int xx = -detectRange; xx < detectRange + 1; xx++)
                {
                    for (int yy = -detectRange; yy < detectRange + 1; yy++)
                    {
                        // Bounds of the original 2d array
                        if ((x + xx > 0 && x + xx < width - (detectRange * 2) && y + yy > 0 && y + yy < height - (detectRange * 2)))
                        {
                            if(map[y + yy, x + xx] == Tiles.floorTile)
                            {
                                neighborCount++;
                            }
                        }
                    }
                }
                // The neighbor count is great enough that a new cell will appear
                if(neighborCount >= birthLimit)
                {
                    newMap[y + detectRange, x + detectRange] = Tiles.floorTile;
                }
                // The neighbor count is small enough that the cell will die
                else if(neighborCount < deathLimit)
                {
                    newMap[y + detectRange, x + detectRange] = Tiles.wallTile;
                }
                // The neighbor count does not change anything. If the celldata already exists, use that data
                // Otherwise, the cell will initialize to 0.
                else
                {
                    if ((x > 0 && x < width - (detectRange * 2) && y > 0 && y < height - (detectRange * 2)))
                    {
                        newMap[y + detectRange, x + detectRange] = map[y, x];
                    }
                    else
                    {
                        newMap[y + detectRange, x + detectRange] = Tiles.wallTile;
                    }
                }
            }
        }
        map = newMap;
    }

    //// Converts our integer 2d array into the tilemap!
    //void IntToTile()
    //{
    //    floor.ClearAllTiles();
    //    walls.ClearAllTiles();
    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < height; x++)
    //        {
    //            if (map[y, x] == Tiles.floorTile)// 1 means a floor tile
    //            {
    //                // Offset to keep the tilemap at the expected position
    //                floor.SetTile(new Vector3Int(x - cycles * detectRange, y - cycles * detectRange, 0), floorTile.GetTile());
    //            }
    //            else
    //            {
    //                //If there is a floor (no wall) below
    //                if (y > 0 && map[y - 1, x] == Tiles.floorTile)
    //                {
    //                    //If there is no floor above
    //                    if(y < height && map[y + 1, x] == Tiles.floorTile)
    //                    {
    //                        walls.SetTile(new Vector3Int(x - cycles * detectRange, y - cycles * detectRange, 0), freeStandingWallTile.GetTile());
    //                    }
    //                    else
    //                    {
    //                        walls.SetTile(new Vector3Int(x - cycles * detectRange, y - cycles * detectRange, 0), wallTile.GetTile());
    //                    }
    //                }
    //                else
    //                {
    //                    walls.SetTile(new Vector3Int(x - cycles * detectRange, y - cycles * detectRange, 0), voidTile.GetTile());
    //                }
    //            }
    //        }
    //    }
    //}
}
