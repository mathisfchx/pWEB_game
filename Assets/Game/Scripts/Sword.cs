using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Sword : NetworkBehaviour, IInventoryItem
{
    public string Name
    {
        get
        {
            return "Sword";
        }
    }
    
    public Sprite _Image = null;

    public Sprite Image
    {
        get
        {
            return _Image;
        }
    }

    public void OnPickup()
    {
        //Todo -- logic when grabbing object
        gameObject.SetActive(false);
    }
}
