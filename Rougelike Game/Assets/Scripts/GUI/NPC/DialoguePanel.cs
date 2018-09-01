using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages showing the dialogue
/// </summary>
public class DialoguePanel : MonoBehaviour
{
    public static DialoguePanel instance;
    private Queue<Dialogue> dialogueQueue;
    private Dialogue[] currentDialogue;

    // Keeps track of the current dialogue to determine if startDialogue should just continue the current dialogue
    public Text dialogueText;

    public Image dialogueImage;
    public Text title;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
        dialogueQueue = new Queue<Dialogue>();
    }

    /// <summary>
    /// Will either load and start new dialogue, or continue the existing dialogue
    /// </summary>
    public void StartDialogue(Dialogue[] dialogue)
    {
        gameObject.SetActive(true);

        //Continue current dialogue?
        if (dialogue == currentDialogue) return;

        currentDialogue = dialogue;
        dialogueQueue.Clear();

        foreach (var sentence in dialogue)
        {
            dialogueQueue.Enqueue(sentence);
        }

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
        Dialogue dialogue = dialogueQueue.Dequeue();
        title.text = dialogue.name;
        dialogueImage.sprite = dialogue.portrait;
        StartCoroutine(TypeSentence(dialogue.sentence));
    }

    /// <summary>
    /// Type the words out in sequence
    /// </summary>
    private IEnumerator TypeSentence(string sentence)
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