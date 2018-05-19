using UnityEngine;

public class CanvasInput : MonoBehaviour {

	void Update () {
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
    }
    
}
