using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(string item)
    {
        Item = item;
    }

    public string Item;
}
