using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages showing the dialogue
/// </summary>
public class DialoguePanel : MonoBehaviour
{
    private Queue<string> dialogueQueue;
    private DialogueString currentDialogue;

    // Keeps track of the current dialogue to determine if startDialogue should just continue the current dialogue
    public Text dialogueText;
    public Text title;

    private void Start()
    {
        gameObject.SetActive(false);
        dialogueQueue = new Queue<string>();
    }

    /// <summary>
    /// Will either load and start new dialogue, or continue the existing dialogue
    /// </summary>
    public void StartDialogue(DialogueString dialogue)
    {
        gameObject.SetActive(true);

        //Continue current dialogue?
        if (dialogue == currentDialogue)
        {
            NextSentence();
            return;
        }

        currentDialogue = dialogue;
        dialogueQueue.Clear();

        foreach (var sentence in dialogue.sentences)
        {
            dialogueQueue.Enqueue(sentence);
        }

        title.text = dialogue.name;
        NextSentence();
    }

    /// <summary>
    /// Move dialogue onto next sentence
    /// </summary>
    public void NextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialogueQueue.Dequeue()));
    }

    /// <summary>
    /// Type the words out in sequence
    /// </summary>
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (var letter in sentence)
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    /// <summary>
    /// When the last dialogue is finished, close the dialogue screen
    /// </summary>
    public void EndDialogue()
    {
        gameObject.SetActive(false);
        currentDialogue = null;
    }

}
