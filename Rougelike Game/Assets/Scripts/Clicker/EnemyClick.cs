using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// When the enemy is clicked upon, set the player to move and attack the enemy
/// </summary>
public class EnemyClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerIsDown;

    public void Update()
    {
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