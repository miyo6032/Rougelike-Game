using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// When the enemy is clicked upon, set the player to move and attack the enemy
/// </summary>
public class EnemyClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PlayerMovement playerMovement;
    private bool pointerIsDown;

    private void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void Update()
    {
        if (pointerIsDown)
        {
            playerMovement.StartAutomoveWithTarget(transform);
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
