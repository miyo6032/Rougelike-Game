using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    public static bool enemyInfoOpen = false;

    public string type = "citizen";
    public string npcName = "";
    public string desc = "";
    public int[] saleItems;
    public int[] quests = new int[5];
    public int dialogueID;

    protected Animator animator;
    protected Transform player;
    protected bool altMove = true;
    protected int moveCounter = 0;
    protected Inventory inventory;
    protected BoardManager board;
    protected QuestDatabase qd;

    private int index = 0;

    protected void Start()
    {
        board = GameObject.Find("GameManager").GetComponent<BoardManager>();
        inventory = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        qd = inventory.gameObject.GetComponent<QuestDatabase>();
    }

    public void DeleteQuest()
    {
        quests[index] = -1;
    }

    public Quest currentQuest()
    {
        if(quests[index] == -1)
        {
            return null;
        }
        return qd.GetQuestByID(quests[index]);
    }

    public Quest nextQuest()
    {
        int counter = 0;
        do
        {
            if (index == quests.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            counter++;
            if (counter > quests.Length)
            {
                break;
            }
        } while (quests[index] == -1);

        if (quests[index] == -1)
        {
            return null;
        }
        return qd.GetQuestByID(quests[index]);
    }

    public Quest prevQuest()
    {
        int counter = 0;
        do
        {
            if (index == 0)
            {
                index = quests.Length - 1;
            }
            else
            {
                index--;
            }
            counter++;
            if (counter > quests.Length)
            {
                break;
            }
        } while (quests[index] == -1);

        if (quests[index] == -1)
        {
            return null;
        }
        return qd.GetQuestByID(quests[index]);
    }

    public void nextItem()
    {
        if(index == saleItems.Length - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        inventory.addItemToSpecificSlot(saleItems[index], 33);
    }

    public void prevItem()
    {
        if (index == 0)
        {
            index = saleItems.Length - 1;
        }
        else
        {
            index--;
        }
        inventory.addItemToSpecificSlot(saleItems[index], 33);
    }

    public void OnMouseDown()
    {

        if (enemyInfoOpen)
        {
            return;
        }
        else if(Mathf.Abs(player.position.x - transform.position.x) <= 1 && Mathf.Abs(player.position.y - transform.position.y) <= 1)
        {
            string data = npcName + "\n\n" + desc;
            if (type == "citizen")
            {
                inventory.ActivateDialogue(dialogueID);
            }
            else if(type == "vendor")
            {
                index = 0;
                inventory.activateVendor(data, true, this);
                inventory.addItemToSpecificSlot(saleItems[index], 33);
            }
            else if(type == "quest")
            {
                index = 0;
                inventory.activateQuester(this);
            }
            else if (type == "chest")
            {
                inventory.ToggleChest();
            }
        }
    }
}
