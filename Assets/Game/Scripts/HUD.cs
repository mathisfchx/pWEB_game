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

    //public GameObject hud;

    void Start()
    {
        /*if(!isLocalPlayer){
            hud.SetActive(false);
        }*/
        dico.Add("_Cle(Clone)",cle);
        dico.Add("_BlueFlag(Clone)", blueFlag);
        dico.Add("_RedFlag(Clone)", redFlag);

        inventory.ItemAdded += InventoryScript_ItemAdded;

    }
    
    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {

        Debug.Log("Event d'add objet :"+e.Item);

        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach(Transform slot in inventoryPanel)
        {
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = dico[e.Item];

                break;
            }
        }
    }
}

