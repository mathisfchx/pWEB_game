using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IInventoryItem
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
