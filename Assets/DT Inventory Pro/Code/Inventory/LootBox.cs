using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTInventory {

    public class LootBox : MonoBehaviour
    {
        public bool spawnRandomItems = false;
        public List<Item> lootBoxItems;
        public List<Item> randomItems;
        public int randomItemsCount;

        private void Awake()
        {
            if(lootBoxItems != null)
            {
                List<Item> initializedItems = new List<Item>();

                foreach(Item item in lootBoxItems)
                {
                    var _item = Instantiate(item);
                     initializedItems.Add(_item);
                    _item.gameObject.SetActive(false);
                }

                lootBoxItems.Clear();
                lootBoxItems = initializedItems;
            }

            if (spawnRandomItems && randomItems != null && SaveData.instance == null)
            {
                for (int i = 0; i < randomItemsCount; i++)
                {
                    var _item = Instantiate(randomItems[Random.Range(0, randomItems.Count)]);
                    _item.gameObject.SetActive(false);

                    lootBoxItems.Add(_item);
                }
            }
        }
    }
}