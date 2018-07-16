using UnityEngine;

/// <summary>
/// Handles the player's input for canvases
/// </summary>
public class CanvasInput : MonoBehaviour
{
    public HotbarSlot[] hotbarSlots;

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if ((Input.GetButtonDown("Inventory")))
        {
            InventoryManager.instance.Toggle();
        }
        else if (Input.GetButtonDown("PlayerStat"))
        {
            PlayerStatUI.instance.Toggle();
        }
        else if (Input.GetButtonDown("SkillTree"))
        {
            SkillTree.instance.Toggle();
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
