using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowTooltip : MonoBehaviour {

	private Item item;
	private string data;
	
	private GameObject tooltip;
	private GameObject inventoryCanvas;
	private PlayerStat playerStat;

	void Start(){
		tooltip = GameObject.Find ("Tooltip");
		inventoryCanvas = GameObject.Find ("InventoryCanvas");
		playerStat = GameObject.Find ("Player").GetComponent<PlayerStat> ();
		tooltip.SetActive (false);
		inventoryCanvas.SetActive (false);
	}

	void Update(){
		if (tooltip.activeSelf) {
            if (Input.mousePosition.y < 120)
            {
                tooltip.transform.position = Input.mousePosition + new Vector3(0, 60, 0);
            }
            else
            {
                tooltip.transform.position = Input.mousePosition;
            }
		}
	}

	public void ActivateDelete(){
		//this.item = item;
		data = "Delete Item";
		tooltip.transform.GetChild (0).GetComponent<Text>().text = data;
		tooltip.SetActive (true);
	}

	public void Activate(Item item, ItemData itemData){
		this.item = item;
		if (item.Id < -1) {
			ConstructSkillDataString();
		} else {
			ConstructDataString (itemData);
		}

		tooltip.SetActive (true);
	}

	public void Deactivate(){
		tooltip.SetActive (false);
	}

	public void ConstructDataString(ItemData itemData){

        data = "";//Reset Data

        if (item.Rarity == 1) {
            data = "<color=#f4b042>Empowered </color>\n";
        }
        if (item.Rarity == 2)
        {
            data = "<color=#c542f4>Enchanted </color>\n";
        }
        if (item.Rarity == 3)
        {
            data = "<color=#f8ff49>Immortalized </color>\n";
        }

        data = data + "<color=#FFFFFF>" + item.Title + "</color>\n";
		if(item.Attack > 0 || item.MaxAttack > 0){
            if (item.Attack == item.MaxAttack)
            {
                data = data + "+" + (item.Attack) + " attack" + "\n";
            }
            else
            {
                data = data + "+" + (item.Attack + "-" + item.MaxAttack) + " attack" + "\n";
            }
		}
		if(item.Defence > 0){
			data = data + "+" + (item.Defence + item.Rarity * Mathf.Clamp(item.Defence, 0, 2)) + " defence" ;
		}

        if(item.Skill != 0)
        {
            switch (item.Skill)
            {
                case -2:
                    data = data + "\n\n" + "Skill: Critical Hit";
                    break;

            }
        }
			data = data + "\n\n" + item.Description;

		if (item.ItemLevel > playerStat.level) {
			data = data + "<color=#cc0000>" + "\n\nEquip level: " + item.ItemLevel + "</color>";
		} else if (itemData.equipped) {
			data = data + "\n\nItem is equipped";
		} else {
			data = data + "\n\nDoubleclick to equip";
		}

		tooltip.transform.GetChild (0).GetComponent<Text>().text = data;
	}

	public void ConstructSkillDataString(){
		
		data = "<color=#FFFFFF>" + item.Title + "</color>\n"
			+ "\nRequired Focus:" + item.Attack
				+ "\n\n" + item.Description;

		if (item.ItemLevel > playerStat.level) {
			data = data + "\n\nSkill level: " + item.ItemLevel;
		}

		tooltip.transform.GetChild (0).GetComponent<Text>().text = data;
	}

}
