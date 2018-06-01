using UnityEngine;
using UnityEngine.EventSystems;

//An object that follows the mouse and does things like automoving when clicked
public class MouseClicker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public PlayerMovement playerMovement;

    bool pointerIsDown = false;

	void Update()
    {
        //Snap to nearest block
        float pixelsToWorldUnits = Camera.main.orthographicSize * 2 / Screen.height;

        Vector2 cameraOffset = new Vector2(Screen.width / 2 * pixelsToWorldUnits, Screen.height / 2 * pixelsToWorldUnits);

        Vector2 convertedMousePosition = Input.mousePosition * pixelsToWorldUnits;

        transform.position = convertedMousePosition + -1 * cameraOffset + (Vector2)Camera.main.transform.position;

        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        if (pointerIsDown)
        {
            playerMovement.StartAutomove(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
        }

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
