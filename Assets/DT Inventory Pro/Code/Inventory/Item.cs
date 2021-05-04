/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.Events;

namespace DTInventory
{
    public class Item : MonoBehaviour
    {
        [System.Serializable]
        public class OnUseEvent : UnityEvent { }
        [System.Serializable]
        public class OnPickupEvent : UnityEvent { }

        public int id;
        public string title;
        public string description;
        public string type;
        public Sprite icon;

        [Range(1, 10)]
        public int width = 1, height = 1;

        [Header("Stack options")]
        public bool stackable;

        [Range(1, 100)]
        public int maxStackSize = 1;

        [Range(1, 100)]
        public int stackSize = 1;
        
        [SerializeField]
        public OnUseEvent onUseEvent;
        [SerializeField]
        public OnPickupEvent onPickupEvent;
    }
}
