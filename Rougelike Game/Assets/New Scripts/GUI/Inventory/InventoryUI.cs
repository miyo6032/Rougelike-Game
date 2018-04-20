using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public GameObject gameUI;

	public void ExitInventory()
    {
        gameObject.SetActive(false);
        gameUI.SetActive(true);
    }

}
