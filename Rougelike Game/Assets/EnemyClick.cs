using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyClick : MonoBehaviour, IPointerClickHandler {

    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    //Direct the player to attack this enemy
    public void OnPointerClick(PointerEventData eventData)
    {
        playerMovement.StartAutomoveWithTarget(transform);
    }

}
