using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {

    public int gold;

    public void AddGold(int amount)
    {
        gold += amount;
    }

    /// <summary>
    /// Will attempt to "buy" gold, and return a bool as to whether there is enough gold left
    /// </summary>
    public bool RemoveGold(int amount)
    {
        if(amount > gold)
        {
            return false;
        }
        gold -= amount;
        return true;
    }

}
