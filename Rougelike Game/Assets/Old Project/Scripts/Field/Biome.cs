using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour {

    public GeneratedStructure[] generatedStructures;
    public int generationSparseness = 10;//How sparse a generation will be, namely a 1 in generationsparseness chance.

    public int[] biomeFloors;
    public int[] biomeWalls;
    public int[] biomeEnemies;
    public int wallSparseness = 15;
    public int edgeHardness = 2;
    public int biomePriority = 0;
    public bool useWallsAsFloors = false;
    public int randomBiomeMergeFactor = 0;

    public int weightSum;//Used for generateed structures
    public int[] weightMap;

    void Awake()
    {

        weightMap = new int[generatedStructures.Length];//Creates a map that distributes the weights evenly in consecutive order. Used by attemptStructureGeneration()

        int weightSum = 0;

        for (int i = 0; i < generatedStructures.Length; i++)
        {

            weightMap[i] = weightSum;

            weightSum += generatedStructures[i].generationWeight;

        }
    }

}
