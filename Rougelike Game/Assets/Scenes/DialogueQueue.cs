using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class DialogueQueue : MonoBehaviour {
    List<Dialogue> database = new List<Dialogue>();
    JsonData dialogueData;

    public Image sprite;
    public Text text;
    public Inventory inv;

    private Dialogue currentDialogue;

    public void Start()
    {
        dialogueData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Dialogue.json"));
        ConstructDatabase();
    }

    public Dialogue GetDialogueByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (id == database[i].Id)
                return database[i];
        }
        return null;
    }

    void ConstructDatabase()
    {
        for (int i = 0; i < dialogueData.Count; i++)
        {
            database.Add(new Dialogue(
            dialogueData[i]["sprite"].ToString(),
            dialogueData[i]["text"].ToString(),
            (int)dialogueData[i]["id"],
            (int)dialogueData[i]["next"]
        ));
        }
    }

    public void ActivateDialogue(int id)
    {
        currentDialogue = GetDialogueByID(id);
        text.text = currentDialogue.Text;
        sprite.sprite = Resources.Load<Sprite>(currentDialogue.Sprite);
    }

    void LoadNextDialogue()
    {
        if (currentDialogue.Next != -1)
        {
            ActivateDialogue(currentDialogue.Next);
        }
        else
        {
            inv.DeactivateDialogue();
        }
    }

}

public class Dialogue
{
    public string Sprite;
    public string Text;
    public int Id;
    public int Next;

    public Dialogue(string sprite, string text, int id, int next){

        this.Sprite = sprite;
        this.Text = text;
        this.Id = id;
        this.Next = next;
        
    }

}
