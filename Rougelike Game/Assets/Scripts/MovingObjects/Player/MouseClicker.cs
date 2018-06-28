using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An object that follows the mouse and does things like automoving when clicked
/// CURRENTY NOT IN USE
/// </summary>
public class MouseClicker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerMovement playerMovement;
    bool pointerIsDown;

    void Update()
    {
        // Snap to nearest block
        Vector2 convertedPosition = MousePositionToWorldPosition(Input.mousePosition);
        transform.position = new Vector2(Mathf.Round(convertedPosition.x), Mathf.Round(convertedPosition.y));
        //if (pointerIsDown)
        //{
        //    playerMovement.StartAutomove(new Vector2(Mathf.Round(transform.position.x),
        //        Mathf.Round(transform.position.y)));
        //}
    }

    /// <summary>
    /// Convert from mouse position (in pixels) to world position
    /// </summary>
    private Vector2 MousePositionToWorldPosition(Vector2 mousePosition)
    {
        float pixelsToWorldUnits = Camera.main.orthographicSize * 2 / Screen.height;
        Vector2 cameraOffset = new Vector2(Screen.width / 2f * pixelsToWorldUnits, Screen.height / 2f * pixelsToWorldUnits);
        Vector2 convertedMousePosition = mousePosition * pixelsToWorldUnits;
        return convertedMousePosition + -1 * cameraOffset + (Vector2) Camera.main.transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerIsDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerIsDown = false;
    }
}
