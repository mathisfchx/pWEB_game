using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HUD : NetworkBehaviour
{
    public Inventory inventory;

    void Start()
    {
        inventory.ItemAdded += InventoryScript_ItemAdded;
    }
    
    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        //Soit des objets dejà visibles et on update le nombre
        //Soit dictionnaire string-sprite

        Debug.Log("Event d'add objet :"+e.Item);

        /*
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach(Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = dico(e.item)

                break;
            }
        }
        */
    }
}

