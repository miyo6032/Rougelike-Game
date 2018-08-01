using UnityEngine;
using UnityEditor;

//Adds a super fancy button to generate the tiles in the inspector
[CustomEditor(typeof(TerrainGenerator), true)]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGenerator generator = (TerrainGenerator)target;
        if (GUILayout.Button("Build Map"))
        {
            generator.Generate();
        }

        if (GUILayout.Button("Clear Tiles"))
        {
            generator.ClearTilemap();
        }
    }
}