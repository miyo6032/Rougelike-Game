using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// When the npc is clicked, activate the dialogue if the player is in range.
/// </summary>
public class NPC : MonoBehaviour, IPointerClickHandler
{
    public Dialogue[] dialogue;
    public float speakingDistance = 2;
    private Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    /// <summary>
    /// Only activates if the player is within a certain distance
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Vector3.Distance(player.position, transform.position) < speakingDistance)
        {
            OnNPCClicked();
        }
    }

    /// <summary>
    /// When the npc is clicked, and thus activated
    /// </summary>
    public virtual void OnNPCClicked()
    {
        DialoguePanel.instance.StartDialogue(dialogue);
    }
}