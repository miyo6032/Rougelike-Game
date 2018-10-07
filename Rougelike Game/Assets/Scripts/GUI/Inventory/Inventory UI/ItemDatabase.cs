using UnityEngine;
using System.Collections.Generic;

namespace Rougelike_Game.Assets.Scripts.GUI.Inventory.Inventory_UI
{
    public class ItemDatabase
    {
        private Dictionary<int, ItemScriptableObject> database = new Dictionary<int, ItemScriptableObject>();

        public ItemDatabase(){
            ItemScriptableObject[] items = Resources.LoadAll<ItemScriptableObject>("ScriptableObjects/Items/Level 1");
            foreach(var item in items){
                database.Add(item.GetId(), item);
            }
        }

        public ItemScriptableObject GetItemById(int id){
            ItemScriptableObject item;
            if(database.TryGetValue(id, out item)){
                return item;
            }
            if(id != -1){
                Debug.LogError("Tried to get item of id " + id + " which isn't part of the database");
            }
            return null;
        }
    }
}