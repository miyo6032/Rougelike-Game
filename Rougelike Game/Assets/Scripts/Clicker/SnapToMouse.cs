using UnityEngine;

/// <summary>
/// Snaps the gameobject to the mouse
/// </summary>
public class SnapToMouse : MonoBehaviour {

    private bool pointerIsDown;

    private void Update()
    {
        // Snap to nearest block
        Vector2 convertedPosition = MousePositionToWorldPosition(Input.mousePosition);
        transform.position = new Vector2(Mathf.Round(convertedPosition.x), Mathf.Round(convertedPosition.y));
    }

    /// <summary>
    /// Convert from mouse position (in pixels) to world position
    /// </summary>
    private Vector2 MousePositionToWorldPosition(Vector2 mousePosition)
    {
        float pixelsToWorldUnits = Camera.main.orthographicSize * 2 / Screen.height;
        Vector2 cameraOffset = new Vector2(Screen.width / 2f * pixelsToWorldUnits, Screen.height / 2f * pixelsToWorldUnits);
        Vector2 convertedMousePosition = mousePosition * pixelsToWorldUnits;
        return convertedMousePosition + (-1 * cameraOffset) + (Vector2)Camera.main.transform.position;
    }
}
