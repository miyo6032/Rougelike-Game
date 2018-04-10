using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedPlateau : GeneratedStructure {

    public int size = 3;

    public int edgeRoughness = 1;//Determines how rough the edge will be from 0 to 100

    public GameObject[] wallObjects;

    public GameObject[] edgeObjects;

	void Awake () {

        Generate();
		
	}
	
	void Generate () {//Generates a blob of walls
		
        for(int x = -(size - 1); x < size + 2; x++)
        {
            for (int y = -(size - 1); y < size + 2; y++)
            {

                float distance = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));

                if (distance < size)//Makes the structure more circular
                {
                    bool generateWall = true;

                    if(Mathf.CeilToInt(distance) == size)//If on edge
                    {
                        generateWall = Random.Range(0, 100) < Mathf.Clamp(100 - edgeRoughness, 0, 100);
                    }

                    if (generateWall)
                    {

                        float distanceBelow = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y - 1, 2));

                        GameObject toInstantiate = toInstantiate = wallObjects[Random.Range(0, wallObjects.Length - 1)];

                        GameObject instance = Instantiate(toInstantiate, new Vector3(x, y), Quaternion.identity);
                        instance.transform.SetParent(gameObject.transform);

                    }

                }
            }
        }
	}
}
