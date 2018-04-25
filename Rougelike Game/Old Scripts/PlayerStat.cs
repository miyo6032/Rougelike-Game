using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerStat : MonoBehaviour {

	public int health;
	public int maxHealth;
	public int attack;
    public int maxAttack;
	public int defence;
	public int experience;
	public int maxExperience;
	public int level;
	public int focus;
	public int maxFocus;
    public int gold;
	public List<Item> equippedItems = new List<Item> ();
    public Quest[] questsAccepted = new Quest[5];
    public Text defenceText;
    public Text damageText;
    public Slider experienceSlider;
    public Slider healthSlider;
    public Slider focusSlider;
    public Text healthText;
    public Text focusText;
    public Text maxHealthText;
    public Text experienceText;

    private int index;
    private Inventory inv;
    private ItemDatabase database;
    private QuestDatabase qd;

	private Player player;
    public Text GoldText;
    public Text BuyGoldText;

    // Use this for initialization
    void Start () {
        inv = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        qd = inv.GetComponent<QuestDatabase>();
        database = inv.GetComponent<ItemDatabase>();
		player = GetComponent<Player> ();

        if (LoadData.instance.dataLoaded)
        {
            Load();
            UpdateGold(0);
        }
        else
        {
            UpdateGold(100);
            for (int i = 0; i < 10; i++)
            {
                equippedItems.Add(new Item());
            }
        }
        UpdateFocus(0);
        UpdateHealth();
        UpdateGearStats();
        player.UpdateExperience();
    }
    
    public Quest currentQuest()
    {
        return questsAccepted[index];
    }

    public Quest nextQuest()
    {
        int counter = 0;
        do
        {
            if (index == questsAccepted.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            counter++;
            if (counter > 4) break;
        } while (questsAccepted[index] == null);
        return questsAccepted[index];
    }

    public Quest prevQuest()
    {
        int counter = 0;
        do
        {
            if (index == 0)
            {
                index = questsAccepted.Length - 1;
            }
            else
            {
                index--;
            }

            counter++;
            if (counter > 4) break;
        } while (questsAccepted[index] == null);

        return questsAccepted[index];
    }

    public void DeleteQuest()
    {
        questsAccepted[index] = null;
        inv.nextQuest();
    }

    public bool addQuest(Quest quest)
    {
        for(int i = 0; i < 5; i++)
        {
            if(questsAccepted[i] == quest)
            {
                return false;
            }
        }

        for(int i = 0; i < 5; i++)
        {
            if(questsAccepted[i] == null)
            {
                questsAccepted[i] = quest;
                return true;
            }
        }
        return false;
    }

    public void UpdateGearStats(){

		this.defence = 0;
        this.attack = 1;
        this.maxAttack = 1;
		for(int i = 0; i < equippedItems.Count; i++){
			if(equippedItems[i] != null && equippedItems[i].Id != -1){
				//defence += equippedItems[i].Defence + equippedItems[i].Rarity * Mathf.Clamp(equippedItems[i].Defence, 0, 2);
				//attack += equippedItems[i].Attack + equippedItems[i].Rarity * Mathf.Clamp(equippedItems[i].Attack, 0, 2);
                //maxAttack += equippedItems[i].MaxAttack + equippedItems[i].Rarity * Mathf.Clamp(equippedItems[i].MaxAttack, 0, 2);
            }
		}

        if (attack == maxAttack)
        {
            damageText.text = "Damage: " + attack;
        }
        else
        {
            damageText.text = "Damage: " + attack + "-" + maxAttack;
        }
        defenceText.text = "Defence: " + defence;
    }

    public void UpdateFocus(int f)
    {

        focus = focus + f;

        if (maxFocus < focus)
            focus = maxFocus;

        if (focus < 0)
            focus = 0;

        if (focus < 0)
            focus = 0;

        focusSlider.value = (float)(focus) / (float)(maxFocus) * 100;

        focusText.text = "Focus: " + focus;
    }

    public void UpdateHealth()
    {

        if (maxHealth < health)
            health = maxHealth;

        healthSlider.value = (float)(health) / (float)(maxHealth) * 100;

        if (health < 0)
            health = 0;
        healthText.text = "Health: " + health;
    }

    public void UpdateGold(int change)
    {
        gold += change;
        if (gold < 0) gold = 0;
        GoldText.text = "Gold: " + gold;
        BuyGoldText.text = "Gold: " + gold;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerStat.dat");

        PlayerStatSave pss = new PlayerStatSave();
        pss.gold = gold;
        pss.focus = focus;
        pss.experience = experience;
        pss.health = health;
        pss.level = level;
        pss.maxExperience = maxExperience;
        pss.maxFocus = maxFocus;
        pss.maxHealth = maxHealth;

        for (int i = 0; i < 5; i++)
        {
            if (questsAccepted[i] != null)
            {
                pss.questsAccepted.Add(questsAccepted[i].Id);
            }
            else
            {
                pss.questsAccepted.Add(-1);
            }
        }

        bf.Serialize(file, pss);
        file.Close();

    }

    public void Load()
    {
            BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/playerStat.dat", FileMode.Open);
        PlayerStatSave pss = (PlayerStatSave)bf.Deserialize(file);
        file.Close();

        gold = pss.gold;
            focus = pss.focus;
            experience = pss.experience;
            health = pss.health;
            level = pss.level;
            maxExperience = pss.maxExperience;
            maxFocus = pss.maxFocus;
            maxHealth = pss.maxHealth;

        for (int i = 0; i < 5; i++)
        {
            if (pss.questsAccepted[i] != -1)
            {
                questsAccepted[i] = qd.GetQuestByID(pss.questsAccepted[i]);
            }
        }

    }
}

[System.Serializable]
class PlayerStatSave
{
    public int health;
    public int maxHealth;
    public int attack;
    public int maxAttack;
    public int defence;
    public int food;
    public int experience;
    public int maxExperience;
    public int level;
    public int focus;
    public int maxFocus;
    public int gold;
    public List<int> equippedItems = new List<int>();
    public List<int> questsAccepted = new List<int>();
}

