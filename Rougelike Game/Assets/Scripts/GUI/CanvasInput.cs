using UnityEngine;

/// <summary>
/// Handles the player's input for canvases
/// </summary>
public class CanvasInput : MonoBehaviour
{
    public HotbarSlot[] hotbarSlots;

    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if ((Input.GetButtonDown("Inventory")))
        {
            StaticCanvasList.instance.inventoryManager.Toggle();
        }
        else if (Input.GetButtonDown("PlayerStat"))
        {
            StaticCanvasList.instance.statUI.Toggle();
        }
        else if (Input.GetButtonDown("SkillTree"))
        {
            StaticCanvasList.instance.skillTree.Toggle();
        }
        else if ((Input.GetButtonDown("Hotbar1")))
        {
            hotbarSlots[0].UseItem();
        }
        else if ((Input.GetButtonDown("Hotbar2")))
        {
            hotbarSlots[1].UseItem();
        }
        else if ((Input.GetButtonDown("Hotbar3")))
        {
            hotbarSlots[2].UseItem();
        }
        else if ((Input.GetButtonDown("Hotbar4")))
        {
            hotbarSlots[3].UseItem();
        }
    }
}
