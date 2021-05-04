using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTInventory {
    
    public class RegisterPlayerInstance : MonoBehaviour
    {
        private void Start()
        {
            if (SaveData.instance == null)
            {
                SaveData.instance = gameObject;
            }
            else
            {
                print("Destroy player duplicate. Player instance already exists");
                Destroy(gameObject);
            }
    }
    }
}
