using UnityEngine;
using System.Collections.Generic;

namespace Rougelike_Game.Assets.Scripts.GUI.Inventory.Inventory_UI
{
    public class ItemDatabase
    {
        [SerializeField]
        private ItemScriptableObject[] items;

        private Dictionary<int, ItemScriptableObject> database = new Dictionary<int, ItemScriptableObject>();

        public ItemDatabase(){
            foreach(var item in items){
                database.Add(0, item);
            }
        }

        public ItemScriptableObject GetItemById(int id){
            ItemScriptableObject item;
            if(database.TryGetValue(id, out item)){
                return item;
            }
            Debug.LogError("Tried to get item of id " + id + " which isn't part of the database");
            return null;
        }
    }
}