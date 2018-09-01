using UnityEngine;

namespace UnityEditor
{
    /// <summary>
    /// Adds a new brush to erase large amounts in the tilemap
    /// </summary>
    [CustomGridBrush(false, true, false, "Big Erase")]
    public class LargeErase : GridBrush
    {
        public new int size = 5;

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    base.Erase(gridLayout, brushTarget, position + new Vector3Int(x - Mathf.FloorToInt(size / 2), y - Mathf.FloorToInt(size / 2), 0));
                }
            }
        }

        [MenuItem("Assets/Create/Big Eraser")]
        public static void CreateBrush()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Big Eraser", "New Big Eraser", "Asset", "Save Big Eraser", "Assets");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(CreateInstance<LargeErase>(), path);
        }
    }

    [CustomEditor(typeof(LargeErase))]
    public class BigEraseEditor : GridBrushEditor
    {
        private LargeErase lineBrush { get { return target as LargeErase; } }
    }
}