using UnityEngine;
using UnityEngine.EventSystems;

//When the enemy is clicked upon, the player goes and attacks that enemy
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
