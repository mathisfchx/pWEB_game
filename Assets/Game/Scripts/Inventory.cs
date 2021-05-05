using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Inventory : NetworkBehaviour
{
    private const int SLOTS = 11;
    readonly SyncList<IInventoryItem> mItems = new SyncList<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;

    public void AddItem(IInventoryItem item){
        
        if (mItems.Count < SLOTS){

            mItems.Add(item);

            item.OnPickup();

            if (ItemAdded != null) {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }
}
