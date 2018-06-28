using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An object that follows the mouse and does things like automoving when clicked
/// CURRENTY NOT IN USE
/// </summary>
public class MouseClicker : MonoBehaviour, IPointerClickHandler
{
    public PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playerMovement.StartAutomove(new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)));
    }
}
