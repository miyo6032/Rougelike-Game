using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

//IT DOES EVERYTHING
public class Inventory : MonoBehaviour
{
	private bool inventoryDrawn = false;
    private bool mapDrawn = false;
    private bool buysellmode = false;
    private bool questDrawn = false;
    private bool lootDrawn = false;
    private bool pauseDrawn = false;
    private bool chestDrawn = false;
    private bool dialogueDrawn = false;
	public bool inventoryOpen;

    public Transform blackImage;
    public Transform canvas;
    private ItemDatabase database;
	private GameObject tooltip;
	private PlayerStat playerStat;
    private Player player;
	private Text enemyInfoText;
	private GameObject enemyInfo;
    private GameObject vendorDialogue;
    private GameObject buyCanvas;
    private Text buyGoldText;
    private Text vendorText;
	private GameObject skillPanel;
    private GameObject inventoryPanel;
    private GameObject buySellPanel;
	public GameObject slot;
	public GameObject item;
    private GameObject mapCanvas;
    private SellSlot sellSlot;
    public NPC currentVendor;
    private GameObject questCanvas;
    private Text questDesc;
    private Text questTitle;
    private Text questReward;
    private Text dropAddQuest;
    private Image questImage;
    private Text questLogTitle;
    private GameObject lootPanel;
    private Pickup lootBag;
    private GameObject pauseCanvas;
    private GameObject chestPanel;
    private GameObject deathCanvas;
    private DialogueQueue dialogue;
    private GameObject pausePanel;

    public GameObject[] lootSlots;
    public Hotbar[] hotbarSlots;
    public GameObject[] chestSlots;

	public List<Item> items = new List<Item> ();
	public List<GameObject> slots = new List<GameObject> ();
	
	// Use this for initialization
	void Start ()
	{

        pausePanel = GameObject.Find("GamePausePanel");

        dialogue = GameObject.Find("DialoguePanel").GetComponent<DialogueQueue>();
        dialogue.gameObject.SetActive(false);

        GameObject[] equipSlots = GameObject.FindGameObjectsWithTag("EquipSlot");
        GameObject[] invSlots = GameObject.FindGameObjectsWithTag("Slot");
        GameObject[] skillSlots = GameObject.FindGameObjectsWithTag("SkillSlot");

        mapCanvas = GameObject.Find("MapCanvas");
        mapCanvas.gameObject.SetActive(false);

        vendorDialogue = GameObject.Find("ShopCanvas");
        vendorText = vendorDialogue.GetComponentInChildren<Text>();
        vendorDialogue.SetActive(false);

		enemyInfo = GameObject.Find("EnemyInfoCanvas");
		enemyInfoText = enemyInfo.GetComponentInChildren<Text>();
		enemyInfo.SetActive (false);

        chestPanel = GameObject.Find("ChestPanel");
        chestPanel.SetActive(false);

        buyCanvas = GameObject.Find("BuyCanvas");
        buyGoldText = GameObject.Find("PriceText").GetComponent<Text>();
        buyCanvas.SetActive(false);

        sellSlot = GameObject.Find("SellSlot").GetComponent<SellSlot>();

        buySellPanel = GameObject.Find("BuysellPanel");
        buySellPanel.SetActive(false);

        lootPanel = GameObject.Find("LootPanel");
        lootPanel.SetActive(false);

        questCanvas = GameObject.Find("QuestCanvas");
        questDesc = GameObject.Find("QuestDesc").GetComponent<Text>();
        questTitle = GameObject.Find("QuestTitle").GetComponent<Text>();
        questReward = GameObject.Find("QuestReward").GetComponent<Text>();
        dropAddQuest = GameObject.Find("DropAddQuest").GetComponentInChildren<Text>();
        questImage = GameObject.Find("QuestImage").GetComponent<Image>();
        questLogTitle = GameObject.Find("QuestLogTitle").GetComponent<Text>();
        questImage.gameObject.SetActive(false);
        questCanvas.SetActive(false);

        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);

        deathCanvas = GameObject.Find("DeathCanvas");
        deathCanvas.SetActive(false);

        inventoryPanel = GameObject.Find("InventoryPanel");
		skillPanel = GameObject.Find("SkillsPanel");
		database = GameObject.FindGameObjectWithTag ("ItemDatabase").GetComponent<ItemDatabase> ();
		tooltip = GameObject.Find ("Tooltip");
		playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
        player = playerStat.GetComponent<Player>();

        bubbleSort(equipSlots);
        bubbleSort(invSlots);
        bubbleSort(skillSlots);
        bubbleSort(lootSlots);
        bubbleSort(chestSlots);

        for (int i = 0; i < 8; i++)
        {//Inventory slots for equipment
            items.Add(new Item());
            slots.Add(equipSlots[i]);
            slots[i].GetComponent<Slot>().id = i;
        }

        for (int i = 0; i < 16; i++)
        {//Inventory slots for the inventory
            items.Add(new Item());
            slots.Add(invSlots[i]);
            slots[i + 8].GetComponent<Slot>().id = i + 8;
        }

        for (int i = 0; i < 8; i++)
        {//Slots for the skills
            items.Add(new Item());
            slots.Add(skillSlots[i]);
            slots[i + 24].GetComponent<Slot>().id = i + 24;
            slots[i + 24].GetComponent<Slot>().isSkillSlot = true;
        }

        items.Add(new Item());
        slots.Add(invSlots[16]);

        items.Add(new Item());
        slots.Add(invSlots[17]);

        for(int i = 0; i < 4; i++)
        {
            items.Add(new Item());
            slots.Add(lootSlots[i]);
        }

        for (int i = 0; i < 16; i++)
        {
            items.Add(new Item());
            slots.Add(chestSlots[i]);
        }

        if(LoadData.instance.dataLoaded)
        {
            Load();
        }
        else
        {
            AddItem(0);
            AddItem(1);
        }
    }

    public void ActivateDialogue(int id)
    {
        dialogueDrawn = true;
        dialogue.ActivateDialogue(id);
        dialogue.gameObject.SetActive(true);
        pausePanel.gameObject.SetActive(false);
    }

    public void DeactivateDialogue()
    {
        dialogueDrawn = false;
        dialogue.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
    }

    public bool DialogueDrawn()
    {
        return dialogueDrawn;
    }

    public void ToggleChest()
    {
        if (!chestDrawn)
        {
            altInv();

            chestPanel.SetActive(true);
            chestDrawn = true;
        }
        else
        {
            chestPanel.SetActive(false);
            chestDrawn = false;
        }
    }

    public void ActivatePause()
    {
        pauseCanvas.SetActive(true);
        pauseDrawn = true;
    }

    public void DeactivatePause() {
        pauseCanvas.SetActive(false);
        pauseDrawn = false;
    }

    public void UpdateHotbar()
    {
        for(int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].UpdateHotbar();
        }
    }

    public void EquipHotbar(int id)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].ToggleImage(id);
        }
    }

    public void openLootBag(Pickup bag)
    {
        altInv();

        lootDrawn = true;

        lootBag = bag;

        for(int i = 0; i < 4; i++)
        {
            if(bag.itemId[i] != -1)
            {
                addItemToSpecificSlot(bag.itemId[i], 34 + i);
            }
            else
            {
                addItemToSpecificSlot(-1, 34 + i);
            }
        }

        lootPanel.SetActive(true);
    }

    public void DeactivateLootBag()
    {
        lootPanel.SetActive(false);
        lootDrawn = false;
        if (lootBag.itemId[0] == -1 && lootBag.itemId[1] == -1 && lootBag.itemId[2] == -1 && lootBag.itemId[3] == -1)
        {
            Destroy(lootBag.gameObject);
        }
    }

    public bool LootDrawn()
    {
        return lootDrawn;
    }

    public bool buySellMode()
    {
        return buysellmode;
    }

    public bool ChestDrawn()
    {
        return chestDrawn;
    }

    public void deactivateInfo()
    {
        if (NPC.enemyInfoOpen)
        {
            if (buysellmode)
            {
                activateVendor("", false, null);
            }
            else
            {
                activateNPCInfo("", false);
            }
        }

        if (Enemy.enemyInfoOpen) activateEnemyInfo("", false);
    }

	public void activateEnemyInfo(string text, bool active){

		if (active && !mapDrawn) {
			enemyInfo.SetActive (true);
			inventoryOpen = true;
			enemyInfoText.text = text;
			Enemy.enemyInfoOpen = true;
		} else {
			enemyInfo.SetActive(false);
			inventoryOpen = false;
			Enemy.enemyInfoOpen = false;
		}

	}

    public void activateNPCInfo(string text, bool active)
    {
        if (active && !mapDrawn)
        {
            enemyInfo.SetActive(true);
            inventoryOpen = true;
            enemyInfoText.text = text;
            NPC.enemyInfoOpen = true;
        }
        else
        {
            enemyInfo.SetActive(false);
            inventoryOpen = false;
            NPC.enemyInfoOpen = false;
        }

    }

    public void activateVendor(string text, bool active, NPC vendor)
    {
        if (active && !mapDrawn)
        {
            vendorDialogue.SetActive(true);
            inventoryOpen = true;
            vendorText.text = text;
            NPC.enemyInfoOpen = true;
            buysellmode = true;
            currentVendor = vendor;
        }
        else
        {
            vendorDialogue.SetActive(false);
            inventoryOpen = false;
            NPC.enemyInfoOpen = false;
            buysellmode = false;
        }

    }

    private void setCurrentQuest()
    {

        Quest q;

        if (currentVendor == null)
        {
            q = playerStat.currentQuest();
            if (q == null)
            {
                questTitle.text = "Empty";
                questDesc.text = "";
                questReward.text = "";
                dropAddQuest.transform.parent.gameObject.SetActive(false);
                questImage.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            q = currentVendor.currentQuest();
            if (q == null)
            {
                questTitle.text = "This quest has been completed!";
                questImage.gameObject.SetActive(false);
                dropAddQuest.transform.parent.gameObject.SetActive(false);
                return;
            }
        }

        if (currentVendor != null)
        {
            dropAddQuest.text = "Accept this quest";
            questLogTitle.text = currentVendor.name;
        }
        else
        {
            dropAddQuest.text = "Give up on this quest";
            questLogTitle.text = "Quest Log";
        }

        dropAddQuest.transform.parent.gameObject.SetActive(true);
        questTitle.text = q.Title;
        questDesc.text = q.Description;
        questReward.text = "Reward: " + q.Value.ToString();
        questImage.gameObject.SetActive(true);
        questImage.sprite = q.Sprite;
        if (q.Completed)
        {
            dropAddQuest.text = "Claim Reward";
        }
    }

    public void activateQuester(NPC vendor)
    {
        if (!mapDrawn)
        {
            questCanvas.SetActive(true);
            questDrawn = true;
            NPC.enemyInfoOpen = true;
            currentVendor = null;
            currentVendor = vendor;

            if (currentVendor != null)
            {
                dropAddQuest.text = "Accept this quest";
                questLogTitle.text = currentVendor.name;
            }
            else
            {
                dropAddQuest.text = "Give up on this quest";
                questLogTitle.text = "Quest Log";
            }

            setCurrentQuest();

        }
        else
        {
            questCanvas.SetActive(false);
            NPC.enemyInfoOpen = false;
            questDrawn = false;
        }
    }

    public bool IsQuestDrawn()
    {
        return questDrawn;
    }

    public void nextQuest()
    {
        if(currentVendor == null)
        {
            playerStat.nextQuest();
        }
        else
        {
            currentVendor.nextQuest();
        }
        setCurrentQuest();
    }

    public void prevQuest()
    {
        if (currentVendor == null)
        {
            playerStat.prevQuest();
        }
        else
        {
            currentVendor.prevQuest();
        }
        setCurrentQuest();
    }

    public void DropAddQuest()//Either add a quest, drop a quest, or give reward for the quest
    {
        if (currentVendor == null)
        {
            playerStat.DeleteQuest();
        }
        else
        {
            if (currentVendor.currentQuest().Completed)
            {
                playerStat.UpdateGold(currentVendor.currentQuest().Value);
                currentVendor.DeleteQuest();
                nextQuest();
            }
            else
            {
                if (!playerStat.addQuest(currentVendor.currentQuest()))
                {
                    dropAddQuest.text = "Quest already accepted, or quest log full!";
                }
            }
        }
    }

    public void activateBuyCanvas()
    {
        buyCanvas.SetActive(true);
    }

    private void bubbleSort(GameObject[] arr)
    {
        GameObject temp;
        for(int i = 0; i < arr.Length; i++)
        {
            for(int sort = 0; sort < arr.Length - 1; sort++)
            {
                if(arr[sort].GetComponent<Slot>().id > arr[sort + 1].GetComponent<Slot>().id)
                {
                    temp = arr[sort + 1];
                    arr[sort + 1] = arr[sort];
                    arr[sort] = temp;
                }
            }
        }
    }

	public void RemoveItem(int slot){
		items [slot] = new Item ();

        if(slots[slot].transform.childCount > 0)
            Destroy(slots[slot].GetComponentInChildren<ItemData>().gameObject);
	}

	public void AddItem(int id){
		Item itemToAdd = database.GetItemByID (id);//Searches the database for the item

		if (itemToAdd.Stackable && ItemInInventory (itemToAdd))
        {//Adds and existing item to the inventory
            for (int i = 0; i < items.Count; i++) {
				if (items [i].Id == id) {
					ItemData data = slots [i].transform.GetChild (0).GetComponent<ItemData> ();
					data.amount++;
					data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
					break;
				}
			}
		} else {
			for (int i = 8; i < items.Count; i++) {//Adds new item
				if ((items [i].Id == -1 && !slots[i].GetComponent<Slot>().isSkillSlot && id > -1) || (items [i].Id == -1 && slots[i].GetComponent<Slot>().isSkillSlot && id < -1)){
					items [i] = itemToAdd;
					GameObject itemObj = Instantiate (item);
					itemObj.GetComponent<ItemData> ().item = itemToAdd;
					itemObj.GetComponent<ItemData> ().amount = 1;
					itemObj.GetComponent<ItemData> ().slot = i;
                    itemObj.transform.SetParent (slots [i].transform);
					itemObj.GetComponent<Image> ().sprite = itemToAdd.Sprite;
					itemObj.name = itemToAdd.Title;
					ItemData data = slots [i].transform.GetChild (0).GetComponent<ItemData> ();
                    data.transform.localScale = new Vector3(1, 1, 1);
					if(id > -1){
						data.transform.GetChild (0).GetComponent<Text> ().text = "";
					}
					break;
				}
			}
		}
	}

    public void addItemToSpecificSlot(int id, int slot)
    {
        if (items[slot].Id != -1) Destroy(slots[slot].transform.GetChild(0).gameObject);

        if (id == -1)
        {
            items[slot] = new Item();
            return;
        }

        Item itemToAdd = database.GetItemByID(id);
        items[slot] = itemToAdd;

        if (slot == 33)
        {
            buyGoldText.text = "Gold: " + itemToAdd.Value * 2;
        }

        GameObject itemObj = Instantiate(item);
        ItemData itemdata = itemObj.GetComponent<ItemData>();
        itemdata.item = itemToAdd;
        itemdata.amount = 1;
        itemdata.slot = slot;
        itemObj.transform.SetParent(slots[slot].transform);
        itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
        itemObj.name = itemToAdd.Title;
        ItemData data = slots[slot].transform.GetChild(0).GetComponent<ItemData>();

        if(slot < 8)
        {
            itemdata.EquipItem();
        }

        if (id > -1)
        {
            data.transform.GetChild(0).GetComponent<Text>().text = "";
        }

        data.transform.localScale = new Vector3(1, 1, 1);
    }

    public void LoadItem(int id, int slot)
    {
        if (items[slot].Id != -1) Destroy(slots[slot].transform.GetChild(0).gameObject);

        if (id == -1)
        {
            items[slot] = new Item();
            return;
        }

        Item itemToAdd = database.GetItemByID(id);
        items[slot] = itemToAdd;

        if (slot == 33)
        {
            buyGoldText.text = "Gold: " + itemToAdd.Value * 2;
        }

        GameObject itemObj = Instantiate(item);
        ItemData itemdata = itemObj.GetComponent<ItemData>();
        itemdata.item = itemToAdd;
        itemdata.amount = 1;
        itemdata.slot = slot;
        itemObj.transform.SetParent(slots[slot].transform);
        itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
        itemObj.name = itemToAdd.Title;
        ItemData data = slots[slot].transform.GetChild(0).GetComponent<ItemData>();

        if (id > -1)
        {
            data.transform.GetChild(0).GetComponent<Text>().text = "";
        }

        if (slot < 8)
        {
            itemdata.equipped = true;
            itemdata.transform.GetComponentInChildren<Text>().text = "E";
            playerStat.equippedItems[itemdata.item.EquippedSlot] = itemdata.item;
            if (itemToAdd.Skill != 0)
            {
                AddItem(itemToAdd.Skill);
            }
        }

        data.transform.localScale = new Vector3(1, 1, 1);
    }

    private int ItemRarity()
    {
        int rand = Random.Range(0, 100);
        if (rand < 60) {
            return 0;
        }
        else if(rand >= 60 && rand <= 88)
        {
            return Random.Range(1, 2);
        }
        else if(rand > 88 && rand <= 97)
        {
            return Random.Range(3, 4); ;
        }
        else
        {
            return Random.Range(5, 7); ;
        }
    }

	public bool itemIsAlreadyEquipped(int id){
		for (int i = 0; i < 8; i++) {
			if(items[i].Id == id){
				if(slots[i].gameObject.GetComponentInChildren<ItemData>().equipped)
				{
					return true;
				}
			}
		}
		return false;
	}

    public int findEmptySlot()
    {
        for (int i = 8; i < 24; i++)
        {
            if (items[i].Id == -1)
            {
                return i;
            }
        }
        return -1;
    }

	public void unequipItemType(int id){
		for (int i = 0; i < 24; i++) {
			if(items[i].Id == id){
				if(slots[i].gameObject.GetComponentInChildren<ItemData>() != null && slots[i].gameObject.GetComponentInChildren<ItemData>().equipped)
				{
                    ItemData data = slots[i].gameObject.GetComponentInChildren<ItemData>();
                    data.equipped = false;
					playerStat.equippedItems [items[i].EquippedSlot] = new Item ();
					playerStat.UpdateGearStats ();
                    data.transform.GetChild(0).GetComponent<Text>().text = "";
                    if (data.item.Skill != 0)
                    {
                        ItemData itemd = GetItemdata(data.item.Skill);
                        items[itemd.slot] = new Item();
                        GameObject.Destroy(itemd.gameObject);
                    }
                }
			}
		}
	}

	public bool ItemInInventory(Item item){
		for (int i = 0; i < items.Count; i++) {
			if(items[i].Id == item.Id){
				return true;
			}
		}
		return false;
	}

    public ItemData GetItemdata(int id)//Finds an item id and returns the item data if exisitng
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount!= 0 && slots[i].gameObject.GetComponentInChildren<ItemData>().item.Id == id)
            {
                return slots[i].gameObject.GetComponentInChildren<ItemData>();
            }
        }
        return null;
    }

    public bool IsMapDrawn()
    {
        return mapDrawn;
    }
	
	// Update is called once per frame
	void Update ()
	{

        if ((player != null && player.isDead) || dialogueDrawn) return;

        if (lootPanel.activeSelf)
        {
            lootBag.itemId[0] = items[34].Id;
            lootBag.itemId[1] = items[35].Id;
            lootBag.itemId[2] = items[36].Id;
            lootBag.itemId[3] = items[37].Id;
        }
		if (Input.GetButtonDown ("Inventory")) {
            if (!lootDrawn)
            {
                bool foundPickup = false;
                GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
                for (int i = 0; i < pickups.Length; i++)
                {
                    if (pickups[i].transform.position == playerStat.gameObject.transform.position)
                    {
                        openLootBag(pickups[i].GetComponent<Pickup>());
                        foundPickup = true;
                    }
                    else if (Vector3.Distance(pickups[i].transform.position, playerStat.gameObject.transform.position) > 20)
                    {
                        Destroy(pickups[i]);
                    }
                }
                if (foundPickup) return;
            }
            if (!mapDrawn && !questDrawn && !pauseDrawn)
            {
                if (NPC.enemyInfoOpen)
                {
                    if (buysellmode) {
                        
                        if (buyCanvas.activeSelf)
                        {
                            buyCanvas.SetActive(false);
                        }
                        else if (canvas.gameObject.activeSelf)
                        {
                            deactivateInv();
                        }
                        else
                        {
                            activateVendor("", false, null);
                            buysellmode = false;
                        }
                    }
                    else
                    {
                        activateNPCInfo("", false);
                    }
                }
                else if(Enemy.enemyInfoOpen)
                {
                    activateEnemyInfo("", false);
                }
                else
                {
                    altInv();
                }
                
            }
            else if(mapDrawn) {
                mapCanvas.gameObject.SetActive(false);
                mapDrawn = false;
            }
            else if (questDrawn)
            {
                deactivateQuest();
            }
            else if (pauseDrawn)
            {
                DeactivatePause();
            }
        }
        else if (Input.GetButtonDown("Map") && !inventoryOpen && !questDrawn && !pauseDrawn)
        {
            if (!mapDrawn)
            {
                mapDrawn = true;
                mapCanvas.gameObject.SetActive(true);
            }
            else
            {
                mapDrawn = false;
                mapCanvas.gameObject.SetActive(false);
            }
        }
        else if(Input.GetButtonDown("Quest") && !inventoryOpen && !mapDrawn && !pauseDrawn)
        {
            if (!questDrawn)
            {
                activateQuester(null);
            }
            else
            {
                deactivateQuest();
            }
        }
	}

    public void toggleMap()
    {
        if (!mapDrawn)
        {
            mapDrawn = true;
            mapCanvas.gameObject.SetActive(true);
        }
        else
        {
            mapDrawn = false;
            mapCanvas.gameObject.SetActive(false);
        }
    }

    public void deactivateQuest()
    {
        questDrawn = false;
        questCanvas.SetActive(false);
        NPC.enemyInfoOpen = false;
    }

    public void DestroyLootBag()
    {
        lootBag.itemId[0] = -1;
        lootBag.itemId[1] = -1;
        lootBag.itemId[2] = -1;
        lootBag.itemId[3] = -1;
        altInv();
    }

    public void altInv()//Open or close any inventory, no matter what it is.
    {
        inventoryOpen = !inventoryOpen;

        if (!inventoryDrawn)
        {
            inventoryDrawn = true;
        }

        if (inventoryOpen && playerStat.health > 0)
        {
            canvas.gameObject.SetActive(true);
        }

        if (!inventoryOpen)
        {
            UpdateHotbar();

            canvas.gameObject.SetActive(false);
            tooltip.SetActive(false);

            if (lootPanel.activeSelf)
            {
                lootDrawn = false;
                lootPanel.SetActive(false);
                if (lootBag.itemId[0] == -1 && lootBag.itemId[1] == -1 && lootBag.itemId[2] == -1 && lootBag.itemId[3] == -1)
                {
                    Destroy(lootBag.gameObject);
                }
            }

            if (chestDrawn)
            {
                chestDrawn = false;
                chestPanel.SetActive(false);
            }

        }
    }

    public void activateInv()
    {
        canvas.gameObject.SetActive(true);
        if (buysellmode)
        {
            buySellPanel.SetActive(true);
        }
    }

    public void deactivateInv()
    {
        buySellPanel.SetActive(false);
        canvas.gameObject.SetActive(false);
        tooltip.SetActive(false);

        if(items[32].Id != -1)//Handles the case when the player exits the sell dialogue with and item in the sell slot
        {
            for(int i = 0; i < sellSlot.item.amount; i++)
            {
                AddItem(sellSlot.item.item.Id);
            }
            RemoveItem(32);
            
        }

    }

    public void nextItem()
    {
        currentVendor.nextItem();
    }

    public void prevItem()
    {
        currentVendor.prevItem();
    }

    public void buyItem()
    {
        ItemData data = slots[33].transform.GetChild(0).GetComponent<ItemData>();

        if(playerStat.gold >= data.item.Value * 2)
        {
            playerStat.UpdateGold(-data.item.Value * 2);
            AddItem(data.item.Id);
        }

    }

    public void ActivateDeathCanvas()
    {
        deathCanvas.SetActive(true);
    }

    public void SaveAll()
    {
        Save();
        playerStat.Save();
        playerStat.GetComponent<Player>().Save();
        playerStat.GetComponent<Player>().board.Save();
    }

    public void Reload()
    {
        LoadData.instance.Load();
    }

    public void LoadTitleScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/inventory.dat");

        InventorySave pss = new InventorySave();

        for(int i = 0; i < items.Count; i++)
        {
            pss.items.Add(items[i].Id);
        }

        bf.Serialize(file, pss);
        file.Close();

    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/inventory.dat", FileMode.Open);
        InventorySave pss = (InventorySave)bf.Deserialize(file);
        file.Close();

        for (int i = 0; i < 10; i++)
        {
            playerStat.equippedItems.Add(new Item());
        }

        for (int i = 0; i < pss.items.Count; i++)
        {
            items.Add(new Item());
            Item item = database.GetItemByID(pss.items[i]);
            if (item != null)
            {
                LoadItem(item.Id, i);
            }
            else
            {
                LoadItem(-1, i);
            }
        }
    }
}

[System.Serializable]
class InventorySave
{
    public List<int> items = new List<int>();
}
