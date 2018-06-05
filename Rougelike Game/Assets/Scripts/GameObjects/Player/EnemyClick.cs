using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// When the enemy is clicked upon, set the player to move and attack the enemy
/// </summary>
public class EnemyClick : MonoBehaviour, IPointerClickHandler
{
    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playerMovement.StartAutomoveWithTarget(transform);
    }
}
