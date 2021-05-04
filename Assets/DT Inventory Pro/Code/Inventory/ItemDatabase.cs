using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTInventory {

    [CreateAssetMenu(fileName = "New items database")]
    public class ItemDatabase : ScriptableObject
    {
        public string databaseName;
        public List<Item> items;

        public void ScanProjectItems(List<Item> _items)
        {
            items.Clear();
            
            foreach(var item in _items)
            {
                items.Add(item);
            }
        }

        public Item FindItem(string title)
        {
            foreach(var item in items)
            {
                if (item.title == title)
                    return item;
            }

            return null;
        }

    }
}