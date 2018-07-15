﻿using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// When the npc is clicked, activate the dialogue if the player is in range.
/// </summary>
public class NPC : MonoBehaviour, IPointerClickHandler
{
    public DialogueString dialogue;
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
            StaticCanvasList.instance.dialoguePanel.StartDialogue(dialogue);
        }
    }
}