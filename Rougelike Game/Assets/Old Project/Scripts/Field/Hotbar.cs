using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour, IPointerClickHandler
{

    public int id;

    Inventory inv;
    Player player;
    Image item;
    Image image;
    bool equipped = false;

    void Start()
    {
        inv = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        player = GameObject.Find("Player").GetComponent<Player>();
        item = transform.GetChild(0).GetComponent<Image>();
        item.gameObject.SetActive(false);
        image = gameObject.GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inv.items[id].Id == -1) return;
        player.equipSkill(id);
    }

    public void ToggleImage(int slot)
    {
        if (slot == id)
        {
            if (equipped)
            {
                equipped = false;
                image.sprite = Resources.Load<Sprite>("Sprites/slot");
            }
            else
            {
                equipped = true;
                image.sprite = Resources.Load<Sprite>("Sprites/goldenSlot");
            }
        }
        else if(equipped)
        {
            equipped = false;
            image.sprite = Resources.Load<Sprite>("Sprites/slot");
        }
    }

    public void UpdateHotbar()
    {

        if (inv.items[id].Id == -1)
        {
            item.gameObject.SetActive(false);
        }
        else
        {
            item.gameObject.SetActive(true);
            item.sprite = inv.items[id].Sprite;
        }

    }
}
