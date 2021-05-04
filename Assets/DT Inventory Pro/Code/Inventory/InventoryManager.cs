/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.Events;

namespace DTInventory
{
    [System.Serializable]
    public class OnInventoryOpen : UnityEvent { }
    [System.Serializable]
    public class OnInventoryClose : UnityEvent { }

    public class InventoryManager : MonoBehaviour
    {
        /// <summary>
        /// InventoryManager aviliable modes
        /// </summary>
        public enum ActiveMode { loot, inventory }

        /// <summary>
        /// Current InventoryManager mode
        /// </summary>
        public ActiveMode mode = ActiveMode.inventory;

        /// <summary>
        /// Inventory canvas
        /// </summary>
        Canvas canvas;

        /// <summary>
        /// Use this variable to show or close inventory! Not requires invmanager reference. Static variable
        /// </summary>
        public static bool showInventory = false;

        /// <summary>
        /// Toggle to prevent update execution each frame
        /// </summary>
        private bool isOpen = false;

        /// <summary>
        /// KeyCode to open inventory
        /// </summary>
        public KeyCode inventoryOpenKey = KeyCode.I;
        
        public bool lookCursorWhenInventoryOff = true;

        public OnInventoryOpen OnOpen;
        public OnInventoryClose OnClose;

        private DTInventory inventory;

        private PickupItem pickupItem;

        private void Start()
        {
            pickupItem = FindObjectOfType<PickupItem>();

            showInventory = false;
            isOpen = false;
            InventoryClose();
            canvas.enabled = false;

            if (pickupItem.interactionType != InteractionType.clickToPickup)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        private void OnEnable()
        {
            if(canvas == null)
                canvas = GetComponent<Canvas>();

            if (inventory == null)
                inventory = FindObjectOfType<DTInventory>();

            InventoryClose();
        }

        private void Update()
        {
            if (Input.GetKeyDown(inventoryOpenKey))
            {
                showInventory = !showInventory;
            }

            if (showInventory)
            {
                InventoryOpen();
            }
            else
            {
                mode = ActiveMode.inventory;
                InventoryClose();
            }

            if (mode == ActiveMode.inventory)
            {
                if (inventory.lootPanel != null)
                inventory.lootPanel.gameObject.SetActive(false);
            }
            else if (mode == ActiveMode.loot)
            {
                if (inventory.lootPanel != null)
                    inventory.lootPanel.gameObject.SetActive(true);
            }
        }
    
        /// <summary>
        /// Method called to open inventory. Has event callback
        /// </summary>
        private void InventoryOpen()
        {
            if (isOpen)
                return;
            else
            {

                if (pickupItem.interactionType != InteractionType.clickToPickup)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                canvas.enabled = true;
                OnOpen.Invoke();
                isOpen = true;
            }
        }

        /// <summary>
        /// Method called to close inventory. Has event callback
        /// </summary>
        private void InventoryClose()
        {
            if (!isOpen)
                return;
            else
            {
                if (lookCursorWhenInventoryOff && pickupItem.interactionType != InteractionType.clickToPickup)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                canvas.enabled = false;
                OnClose.Invoke();
                isOpen = false;
            }
        }

        /// <summary>
        /// Use this method to open - close inventory with button on mobile
        /// </summary>
        public void MobileToggle()
        {
            showInventory = !showInventory;
        }
    }
}