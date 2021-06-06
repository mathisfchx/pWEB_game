using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HUD : NetworkBehaviour
{
    public Inventory inventory;
    public Sprite cle;
    public Sprite blueFlag;
    public Sprite redFlag;

    public Dictionary<string, Sprite> dico = new Dictionary<string, Sprite>();

    void Start()
    {

        dico.Add("_Cle(Clone)",cle);
        dico.Add("_BlueFlag(Clone)", blueFlag);
        dico.Add("_RedFlag(Clone)", redFlag);

        inventory.ItemAdded += InventoryScript_ItemAdded;

    }
    
    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {

        //Debug.Log("Event d'add objet :"+e.Item);

        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach(Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

            if (!image.enabled)
            {
                image.name = e.Item;
                image.enabled = true;
                image.sprite = dico[e.Item];

                break;
            }
        }
    }

    public void DelItem(string nameItem){
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach(Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

            //Debug.Log(image.name);
            //Debug.Log(nameItem);

            if (image.enabled && nameItem == image.name)
            {
                //Debug.Log("DelItem");
                slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;

                break;
            }
        }
    }
}

