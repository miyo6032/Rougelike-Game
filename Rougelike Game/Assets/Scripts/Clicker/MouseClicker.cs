using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An object that follows the mouse and does things like automoving when clicked
/// </summary>
public class MouseClicker : MonoBehaviour, IPointerClickHandler
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void OnPointerClick(PointerEventData eventData)
    { }
}