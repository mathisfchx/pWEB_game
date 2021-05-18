using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Inventory : NetworkBehaviour
{
    private const int SLOTS = 11;
    private List<string> mItems = new List<string>();
    public event EventHandler<InventoryEventArgs> ItemAdded;

    public void AddItem(string item){
        
        if (mItems.Count < SLOTS){

            mItems.Add(item);

            if (ItemAdded != null) {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }
}

