using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;

public class QuestDatabase : MonoBehaviour
{
    private List<Quest> database = new List<Quest>();
    private JsonData questData;

    void Start()
    {

        questData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Quests.json"));
        ConstructItemDatabase();

    }

    public Quest GetQuestByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (id == database[i].Id)
                return database[i];
        }
        return null;
    }

    void ConstructItemDatabase()
    {
        for (int i = 0; i < questData.Count; i++)
        {
                database.Add(new Quest(
                (int)questData[i]["id"],
                questData[i]["title"].ToString(),
                (int)questData[i]["value"],
                 questData[i]["description"].ToString(),
                (string)questData[i]["slug"]
            ));
        }
    }

}

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }
    public bool Completed { get; set; }

    public Quest(int id, string title, int value, string description, string slug)
    {
        this.Id = id;
        this.Title = title;
        this.Value = value;       
        this.Description = description;        
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("Quests/" + slug);
        Completed = false;
    }

    public Quest()
    {
        this.Id = -1;
    }
}
