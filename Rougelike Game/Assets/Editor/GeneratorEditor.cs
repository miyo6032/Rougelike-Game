using UnityEngine;
using UnityEditor;

//Adds a super fancy button to generate the tiles in the inspector
[CustomEditor(typeof(MapGenerator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapGenerator generator = (MapGenerator)target;
        if (GUILayout.Button("Build Map"))
        {
            generator.GenerateMap();
        }

    }
}