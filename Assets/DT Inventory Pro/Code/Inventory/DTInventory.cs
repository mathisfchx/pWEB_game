using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DTInventory
{
    [System.Serializable]
    /// <summary>
    /// Event to provide callback for item add case
    /// </summary>
    public class OnInventoryItemAdd : UnityEvent { }

    [System.Serializable]
    /// <summary>
    /// Event to provide callback for item drop case
    /// </summary>
    public class OnInventoryItemDrop : UnityEvent { }

    [System.Serializable]
    /// <summary>
    /// Event to provide callback for item remove case
    /// </summary>
    public class OnInventoryItemRemove : UnityEvent { }
    
    /// <summary>
    /// Main core class of inventory system
    /// </summary>
    public class DTInventory : MonoBehaviour
    {
        //Unity events to provide callback in specified cases
        public OnInventoryItemAdd OnInventoryItemAdd;
        public OnInventoryItemDrop OnInventoryItemDrop;
        public OnInventoryItemRemove OnInventoryItemRemove;

        /// <summary>
        /// A list that contains all items in inventory.
        /// </summary>
        public List<InventoryItem> inventoryItems = new List<InventoryItem>();

        /// <summary>
        /// Equipment panels which stores items that havs being equiped on player
        /// </summary>
        public List<EquipmentPanel> equipmentPanels;
        
        /// <summary>
        /// Should item be dropped by dragging it outside of inventory bounds?
        /// </summary>
        public bool dragItemOutsideToDrop = false;
        
        /// <summary>
        /// Player's transform. We drop items at player position point + player forward
        /// </summary>
        public Transform player;

        /// <summary>
        /// It's a transform to store scene root to drop items from DontDestroyOnLoad to active scene
        /// </summary>
        [HideInInspector]
        public Transform levelPoint;

        public float distanceAwayFromPlayerToDrop = 1f;

        /// <summary>
        /// Should we equip picked items if we have free slot for such type of equipment?
        /// </summary>
        public bool autoEquipItems = true;

        #region utility

        /// <summary>
        /// An image that used for cell representation (used by InventoryWizard. Don't set manually)
        /// </summary>
        [HideInInspector] public Image cell;

        /// <summary>
        /// Grid cell rect size (used by InventoryWizard. Don't set manually)
        /// </summary>
        [HideInInspector] public int cellSize = 70;

        /// <summary>
        /// Controls a space between cells (used by InventoryWizard. Don't set manually)
        /// </summary>
        [HideInInspector] public int padding;

        /// <summary>
        /// Number of columns in inventory grid (used by InventoryWizard. Don't set manually)
        /// </summary>
        [HideInInspector] public int column = 5;

        /// <summary>
        /// Number of rows in inventroy grid
        /// </summary>
        [HideInInspector] public int row = 4;

        /// <summary>
        /// Number of columns in loot invetory grid (used by InventoryWizard. Don't set manually)
        /// </summary>
        [HideInInspector] public int lootColumn = 4;

        /// <summary>
        /// Number of rows in loot inventory grid (used by InventoryWizrd. Don't set manually)
        /// </summary>
        [HideInInspector] public int lootRow = 4;

        /// <summary>
        /// All inventory slots which used for grid functionality
        /// </summary>
        [HideInInspector] public List<GridSlot> slots;

        /// <summary>
        /// Cell color when in normal state
        /// </summary>
        [HideInInspector] public Color normalCellColor;
        /// <summary>
        /// Cell color when hovered by item
        /// </summary>
        [HideInInspector] public Color hoveredCellColor;
        /// <summary>
        /// Cell color when it's blocked for hovered item
        /// </summary>
        [HideInInspector] public Color blockedCellColor;

        /// <summary>
        /// RectTransform of loot panel
        /// </summary>
        [HideInInspector] public RectTransform lootPanel;

        /// <summary>
        /// Temp reference for LootBox
        /// </summary>
        [HideInInspector] public LootBox activeLootBox;

        /// <summary>
        /// Reference to a InventoryManager component
        /// </summary>
        [HideInInspector] InventoryManager inventoryManager;

        /// <summary>
        /// Internal method being used by InventoryWizard. Do not use it
        /// </summary>
        public void DrawPreview()
        {
            ClearPreview();

            //Inventory grid preview initialization
            for (int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    var _cell = Instantiate(cell);
                    _cell.rectTransform.SetParent(transform);
                    _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                    _cell.rectTransform.anchoredPosition = new Vector2(((cellSize * i) + padding), ((-cellSize * j) + padding));
                    _cell.rectTransform.localScale = new Vector2(1, 1);

                    _cell.name = i + "," + j;

                    _cell.color = normalCellColor;
                }
            }

            //Loot inventory grid preview initialization
            if (lootPanel != null)
            {
                for (int i = 0; i < lootRow; i++)
                {
                    for (int j = 0; j < lootColumn; j++)
                    {
                        var _cell = Instantiate(cell);
                        _cell.rectTransform.SetParent(lootPanel);
                        _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                        _cell.rectTransform.anchoredPosition = new Vector2(((cellSize * i) + padding), ((-cellSize * j) + padding));
                        _cell.rectTransform.localScale = new Vector2(1, 1);

                        _cell.name = i + "," + j;

                        _cell.color = normalCellColor;
                    }
                }
            }

            //Equipment grid preview initialization
            if (equipmentPanels != null)
            {
                for (int k = 0; k < equipmentPanels.Count; k++)
                {
                    for (int i = 0; i < equipmentPanels[k].width; i++)
                    {
                        for (int j = 0; j < equipmentPanels[k].height; j++)
                        {

                            var _cell = Instantiate(cell);
                            _cell.rectTransform.SetParent(equipmentPanels[k].transform);
                            _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                            _cell.rectTransform.anchoredPosition = new Vector2(((cellSize * i) + padding), ((-cellSize * j) + padding));
                            _cell.rectTransform.localScale = new Vector2(1, 1);

                            _cell.name = i + (k + 1) * 100 + "," + j + (k + 1) * 100;

                            _cell.color = normalCellColor;
                        }
                    }
                }
            }

            //Move gameobject transform to end. Required for UI elements overlap proper
            transform.SetAsLastSibling();
        }

        #endregion

        void Awake()
        {
            //We need to keep canvas enabled on start. It will turn off after initialization with InventoryManager
            GetComponentInParent<Canvas>().enabled = true;

            //Initialization of inventory grid 
            Initialize();
        }

        /// <summary>
        /// Destroy each slot before making of new end configuration
        /// </summary>
        public void ClearPreview()
        {
            //Slots which you can see on the screen in edit mode are just previews. We need to delete them and create real instances of the cells.
            if (GetComponents<GridSlot>() != null)
            {
                foreach (var previewSlot in FindObjectsOfType<GridSlot>())
                {
                    if(previewSlot.gameObject.name != "Utility object (Don't delete!)")
                        DestroyImmediate(previewSlot.gameObject);
                }
            }
        }

        /// <summary>
        /// Method that initializes inventory cells and referenced components such as loot inventory and equipment panels
        /// </summary>
        public void Initialize()
        {
            if(SaveData.instance == null)
            ClearPreview();

            inventoryManager = FindObjectOfType<InventoryManager>();

            //Inventory grid initialization
            for (int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    var _cell = Instantiate(cell);
                    _cell.rectTransform.SetParent(transform);
                    _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                    _cell.rectTransform.anchoredPosition = new Vector2(((cellSize * i) + padding), ((-cellSize * j) + padding));
                    _cell.rectTransform.localScale = new Vector2(1, 1);

                    _cell.name = i + "," + j;

                    var slot = _cell.GetComponent<GridSlot>();
                    slot.free = true;
                    slot.image.color = normalCellColor;
                    slot.x = i; slot.y = j;
                    slots.Add(slot);
                }
            }

            //Loot inventory grid initialization
            if (lootPanel != null)
            {
                for (int i = 0; i < lootRow; i++)
                {
                    for (int j = 0; j < lootColumn; j++)
                    {
                        var _cell = Instantiate(cell);
                        _cell.rectTransform.SetParent(lootPanel);
                        _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                        _cell.rectTransform.anchoredPosition = new Vector2(((cellSize * i) + padding), ((-cellSize * j) + padding));
                        _cell.rectTransform.localScale = new Vector2(1, 1);

                        _cell.name = i + "," + j;

                        var slot = _cell.GetComponent<GridSlot>();
                        slot.free = true;
                        slot.image.color = normalCellColor;
                        slot.x = i + 9000; slot.y = j + 9000;
                        slot.isLoot = true;
                        slots.Add(slot);
                    }
                }
            }

            //Equipment grid initialization
            if (equipmentPanels != null)
            {
                for (int k = 0; k < equipmentPanels.Count; k++)
                {
                    for (int i = 0; i < equipmentPanels[k].width; i++)
                    {
                        for (int j = 0; j < equipmentPanels[k].height; j++)
                        {

                            var _cell = Instantiate(cell);
                            _cell.rectTransform.SetParent(equipmentPanels[k].transform);
                            _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                            _cell.rectTransform.anchoredPosition = new Vector2(((cellSize * i) + padding), ((-cellSize * j) + padding));
                            _cell.rectTransform.localScale = new Vector2(1, 1);

                            _cell.name = i + (k + 1) * 100 + "," + j + (k + 1) * 100;

                            var slot = _cell.GetComponent<GridSlot>();
                            slot.free = true;
                            slot.image.color = normalCellColor;
                            slot.equipmentPanel = equipmentPanels[k];
                            slot.x = i + (k + 1) * 100; slot.y = j + (k + 1) * 100;
                            slots.Add(slot);

                            if (i == 0 && j == 0)
                            {
                                equipmentPanels[k].mainSlot = slot;
                            }
                        }
                    }
                }
            }

            //Move gameobject transform to end. Required for UI elements overlap proper
            transform.SetAsLastSibling();
        }

        private void Update()
        {
            if (!InventoryManager.showInventory && activeLootBox != null)
            {
                activeLootBox.lootBoxItems = new List<Item>();

                if (FindItemsLeftInLoot() != null)
                {
                    foreach (var item in FindItemsLeftInLoot())
                    {
                        activeLootBox.lootBoxItems.Add(item.item);
                        RemoveInventoryItem(item);
                    }
                }
                activeLootBox = null;
            }
        }

        /// <summary>
        /// Use this method to add item to inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            if (CheckFreeSpaceForAllSlots(item.width, item.height))
            {
                var _slot = CheckFreeSpaceForAllSlots(item.width, item.height);
                var _InventoryItem = Instantiate(cell).gameObject.AddComponent<InventoryItem>();
                var _InventoryItemImage = _InventoryItem.GetComponent<Image>();

                _InventoryItemImage.rectTransform.SetParent(transform);
                _InventoryItemImage.rectTransform.sizeDelta = new Vector2(cellSize * item.width - padding, cellSize * item.height - padding);
                _InventoryItemImage.rectTransform.anchoredPosition = _slot.GetComponent<RectTransform>().anchoredPosition;
                _InventoryItemImage.color = Color.white;
                _InventoryItemImage.sprite = item.icon;

                _InventoryItem.item = item;

                _InventoryItem.finalPosition = _slot.GetComponent<RectTransform>().anchoredPosition;

                _InventoryItem.x = _slot.x;
                _InventoryItem.y = _slot.y;
                _InventoryItem.width = item.width;
                _InventoryItem.height = item.height;
                _InventoryItem.inventory = this;

                MarkSlots(_slot.x, _slot.y, item.width, item.height, false);

                Destroy(_InventoryItem.GetComponent<GridSlot>());

                inventoryItems.Add(_InventoryItem);

                item.transform.parent = this.transform;

                item.gameObject.SetActive(false);
                
                if (autoEquipItems)
                {
                    foreach (var equipmentPanel in equipmentPanels)
                    {
                        if (equipmentPanel.allowedItemType == _InventoryItem.item.type && equipmentPanel.equipedItem == null)
                        {
                            EquipItem(equipmentPanel, _InventoryItem);
                            MarkSlots(_slot.x, _slot.y, item.width, item.height, true);
                        }
                    }
                }

                OnInventoryItemAdd.Invoke();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to add item to special grid slot.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="x">X grid position</param>
        /// <param name="y">Y grid position</param>
        /// <returns></returns>
        public bool AddItem(Item item, int x, int y)
        {
            if (CheckFreeSpaceForAllSlots(item.width, item.height))
            {
                var _InventoryItem = Instantiate(cell).gameObject.AddComponent<InventoryItem>();
                var _InventoryItemImage = _InventoryItem.GetComponent<Image>();

                _InventoryItemImage.rectTransform.SetParent(FindSlotByIndex(x, y).GetComponent<RectTransform>().transform.parent);
                _InventoryItemImage.rectTransform.sizeDelta = new Vector2(cellSize * item.width - padding, cellSize * item.height - padding);
                _InventoryItemImage.rectTransform.anchoredPosition = FindSlotByIndex(x, y).GetComponent<RectTransform>().anchoredPosition;

                _InventoryItem.item = item;

                _InventoryItemImage.sprite = item.icon;
                _InventoryItem.x = x;
                _InventoryItem.y = y;
                _InventoryItem.width = item.width;
                _InventoryItem.height = item.height;
                _InventoryItem.inventory = this;

                _InventoryItem.finalPosition = FindSlotByIndex(x, y).GetComponent<RectTransform>().anchoredPosition;

                MarkSlots(x, y, item.width, item.height, false);

                Destroy(_InventoryItem.GetComponent<GridSlot>());

                inventoryItems.Add(_InventoryItem);

                item.transform.parent = this.transform;

                item.gameObject.SetActive(false);

                if (autoEquipItems)
                {
                    foreach (var equipmentPanel in equipmentPanels)
                    {
                        if (equipmentPanel.allowedItemType == _InventoryItem.item.type && equipmentPanel.equipedItem == null)
                        {
                            EquipItem(equipmentPanel, _InventoryItem);
                            MarkSlots(x, y, item.width, item.height, true);
                        }
                    }
                }

                OnInventoryItemAdd.Invoke();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Method returns true if item with specified title exists in the inventory
        /// </summary>
        /// <param name="itemTitle"></param>
        /// <returns></returns>
        public bool CheckIfItemExist(string itemTitle)
        {
            foreach (var item in inventoryItems)
            {
                if (item.item.title == itemTitle)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Use this method to drop items from inventory. This method is not destroys items
        /// </summary>
        /// <param name="InventoryItem">Item handler to remove</param>
        public void DropItem(InventoryItem InventoryItem)
        {
            if (InventoryItem == null)
                return;

            int _temp_x, _temp_y, _temp_width, _temp_height;

            _temp_x = InventoryItem.x;
            _temp_y = InventoryItem.y;
            _temp_width = InventoryItem.width;
            _temp_height = InventoryItem.height;

            InventoryItem.item.gameObject.SetActive(true);
            InventoryItem.item.transform.position = player.transform.position + player.transform.forward * distanceAwayFromPlayerToDrop;

            if (levelPoint != null)
            {
                InventoryItem.item.transform.parent = levelPoint.parent;
            }

            InventoryItem.item.gameObject.transform.parent = null;

            inventoryItems.Remove(InventoryItem);

            Destroy(InventoryItem.gameObject);

            OnInventoryItemDrop.Invoke();

            MarkSlots(_temp_x, _temp_y, _temp_width, _temp_height, true);
        }

        /// <summary>
        /// This method is for auto-equip only.Don't use it anywhere else
        /// </summary>
        /// <param name="panel">Equipment panel to move item on</param>
        /// <param name="item">Item we want to equip</param>
        public void EquipItem(EquipmentPanel panel, InventoryItem item)
        {
            item.transform.SetParent(panel.mainSlot.transform.parent);

            item.finalPosition = panel.mainSlot.GetComponent<RectTransform>().anchoredPosition;

            item.x = panel.mainSlot.x;
            item.y = panel.mainSlot.y;

            MarkSlots(panel.mainSlot.x, panel.mainSlot.y, item.width, item.height, false);
            panel.equipedItem = item.item;
        }

        /// <summary>
        /// Method to remove and destroy an item from a scene
        /// </summary>
        /// <param name="InventoryItem">Item to remove and destroy</param>
        public void RemoveItem(InventoryItem InventoryItem)
        {
            int temp_x, temp_y, temp_width, temp_height;

            temp_x = InventoryItem.x;
            temp_y = InventoryItem.y;
            temp_width = InventoryItem.width;
            temp_height = InventoryItem.height;

            inventoryItems.Remove(InventoryItem);

            Destroy(InventoryItem.gameObject);
            Destroy(InventoryItem.item.gameObject);

            OnInventoryItemRemove.Invoke();

            MarkSlots(temp_x, temp_y, temp_width, temp_height, true);
        }

        /// <summary>
        /// We use this method to mark inventory slots free or used by some item.
        /// Used for visual marking of slots with their current state (used, free, blocked)
        /// </summary>
        /// <param name="startSlot_x">Slot x coordinate</param>
        /// <param name="startSlot_y">Slot y corrdinate</param>
        /// <param name="width">Item width in grid space</param>
        /// <param name="height">Item height in grid space</param>
        /// <param name="isFree">State with which we are mark slots. Free or not</param>
        public void MarkSlots(int startSlot_x, int startSlot_y, int width, int height, bool isFree)
        {
            for (int i = startSlot_x; i < startSlot_x + width; i++)
            {
                for (int j = startSlot_y; j < startSlot_y + height; j++)
                {
                    if (FindSlotByIndex(i, j))
                    {
                        var slot = FindSlotByIndex(i, j);
                        slot.free = isFree;

                        if (slot.free) slot.image.color = normalCellColor; else slot.image.color = hoveredCellColor;
                    }
                }
            }
        }

        /// <summary>
        /// Method to check if we can put an item with some width & height to a specified slot 
        /// </summary>
        /// <param name="cell_x">Slot x coordinate</param>
        /// <param name="cell_y">Slot y coordinate</param>
        /// <param name="width">Item width in grid space</param>
        /// <param name="height">Item height in grid space</param>
        /// <returns></returns>
        public GridSlot CheckFreeSpaceAtSlot(int cell_x, int cell_y, int width, int height)
        {
            for (int i = cell_x; i < cell_x + width; i++)
            {
                for (int j = cell_y; j < cell_y + height; j++)
                {
                    if (FindSlotByIndex(i, j) == null || FindSlotByIndex(i, j).free == false)
                    {
                        return null;
                    }
                }
            }

            return FindSlotByIndex(cell_x, cell_y);
        }

        /// <summary>
        /// Method for marking slots as free or not. Sometimes we need to redraw inventory state in some cases
        /// </summary>
        public void DrawRegularSlotsColors()
        {
            foreach (var slot in slots)
            {
                if (slot.free)
                {
                    slot.image.color = normalCellColor;
                }
                else
                {
                    slot.image.color = hoveredCellColor;
                }
            }
        }

        /// <summary>
        /// When we dragging item in inventory grid we call this method to draw current slots state at realtime
        /// </summary>
        /// <param name="startSlot_x">Slot x where pointer over</param>
        /// <param name="startSlot_y">Slot y where pointer over</param>
        /// <param name="width">Item width we want to put to this slot</param>
        /// <param name="height">Item height we want to put to this slot</param>
        public void DrawColorsForHoveredSlots(int startSlot_x, int startSlot_y, int width, int height)
        {
            for (int i = startSlot_x; i < startSlot_x + width; i++)
            {
                for (int j = startSlot_y; j < startSlot_y + height; j++)
                {
                    var slot = FindSlotByIndex(i, j);

                    if (slot != null)
                    {
                        if (slot.free)
                            slot.image.color = hoveredCellColor;
                        else 
                            slot.image.color = blockedCellColor;
                    }
                }
            }
        }

        /// <summary>
        /// Same as DrawColorsForHoveredSlots but for stacking items. With default draw method blocked color will be apeared on attempt to stack items
        /// </summary>
        /// <param name="startSlot_x">Slot x where pointer over</param>
        /// <param name="startSlot_y">Slot y where pointer over</param>
        /// <param name="width">Item width we want to put to this slot</param>
        /// <param name="height">Item height we want to put to this slot</param>
        public void DrawColorForStackableHoveredSlots(int startSlot_x, int startSlot_y, int width, int height)
        {
            for (int i = startSlot_x; i < startSlot_x + width; i++)
            {
                for (int j = startSlot_y; j < startSlot_y + height; j++)
                {
                    var slot = FindSlotByIndex(i, j);

                    if (slot != null)
                    {
                         slot.image.color = hoveredCellColor;
                    }
                }
            }
        }
        
        /// <summary>
        /// Method to check if we have enough space to pickup something
        /// </summary>
        /// <param name="width">Required width to check</param>
        /// <param name="height">Required height to check</param>
        /// <returns></returns>
        public GridSlot CheckFreeSpaceForAllSlots(int width, int height)
        {
            foreach (var slot in slots)
            {
                if (CheckFreeSpaceAtSlot(slot.x, slot.y, width, height) && slot.equipmentPanel == null)
                    return CheckFreeSpaceAtSlot(slot.x, slot.y, width, height);
            }

            return null;
        }

        /// <summary>
        /// Method to find slot with coordinates. Return slot if found or null when is none
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GridSlot FindSlotByIndex(int x, int y)
        {
            foreach (var slot in slots)
            {
                if (slot.x == x && slot.y == y)
                    return slot;
            }

            return null;
        }

        /// <summary>
        /// Remove left items back to lootbox on inspection end
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> FindItemsLeftInLoot()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            if (inventoryItems != null)
            {
                foreach (var item in inventoryItems)
                {
                    if (FindSlotByIndex(item.x, item.y).isLoot && !items.Contains(item))
                    {
                        items.Add(item);
                    }
                }

                return items;
            }
            else
                return null;

        }

        /// <summary>
        /// Remove 
        /// </summary>
        /// <param name="InventoryItem"></param>
        public void RemoveInventoryItem(InventoryItem InventoryItem)
        {
            int temp_x, temp_y, temp_width, temp_height;

            temp_x = InventoryItem.x;
            temp_y = InventoryItem.y;
            temp_width = InventoryItem.width;
            temp_height = InventoryItem.height;

            inventoryItems.Remove(InventoryItem);
            
            Destroy(InventoryItem.gameObject);

            MarkSlots(temp_x, temp_y, temp_width, temp_height, true);
        }

        /// <summary>
        /// Complex method to inspect lootboxes
        /// </summary>
        /// <param name="lootBox"></param>
        public void SearchLootBox(LootBox lootBox)
        {
            //Changing inventory state
            inventoryManager.mode = InventoryManager.ActiveMode.loot;
            InventoryManager.showInventory = true;

            //Making a reference to current lootbox in order to restore left items on inspection end
            activeLootBox = lootBox;

            //Adding an items to loot window in order to interact with them
            AddItemsToLoot(activeLootBox.lootBoxItems);
        }

        /// <summary>
        /// Add items to loot window. There is also check for enough space to place an item
        /// </summary>
        /// <param name="items"></param>
        public void AddItemsToLoot(List<Item> items)
        {
            if (items == null)
                return;

            foreach (var _item in items)
            {
                if (CheckFreeSpaceForAllSlotsLoot(_item.width, _item.height))
                {
                    var slotToEquip = CheckFreeSpaceForAllSlotsLoot(_item.width, _item.height);

                    AddItem(_item, slotToEquip.x, slotToEquip.y);
                }
            }
        }

        /// <summary>
        /// Check for enough space in loot window
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public GridSlot CheckFreeSpaceForAllSlotsLoot(int width, int height)
        {
            foreach (var slot in slots)
            {
                if (CheckFreeSpaceAtSlot(slot.x, slot.y, width, height) && slot.equipmentPanel == null && slot.isLoot == true)
                    return CheckFreeSpaceAtSlot(slot.x, slot.y, width, height);
            }

            return null;
        }

        /// <summary>
        /// Method which allows us to substract stack on a need
        /// </summary>
        /// <param name="InventoryItem"></param>
        public void SubstractStack(InventoryItem InventoryItem)
        {
            // If check free space == false -> exit
            if (InventoryItem.item.stackSize < 2)
                return;

            if (InventoryItem.item.stackable)
            {
                if (InventoryItem.item.stackSize % 2 == 0)
                {
                    if (CheckFreeSpaceForAllSlots(InventoryItem.width, InventoryItem.height) == null)
                    {
                        return;
                    }

                    InventoryItem.item.gameObject.SetActive(true);

                    var second_item_go = Instantiate(InventoryItem.item.gameObject);
                    var second_item = second_item_go.GetComponent<Item>();

                    InventoryItem.item.stackSize /= 2;
                    second_item.stackSize = InventoryItem.item.stackSize;

                    AddItem(second_item);

                    second_item.name = InventoryItem.item.name;

                    InventoryItem.item.gameObject.SetActive(false);
                }
                else if (InventoryItem.item.stackSize % 2 == 1)
                {
                    if (CheckFreeSpaceForAllSlots(InventoryItem.width, InventoryItem.height) == null)
                    {
                        return;
                    }

                    InventoryItem.item.gameObject.SetActive(true);

                    var second_item_go = Instantiate(InventoryItem.item.gameObject);
                    var second_item = second_item_go.GetComponent<Item>();

                    InventoryItem.item.stackSize = (InventoryItem.item.stackSize - 1) / 2;
                    second_item.stackSize = InventoryItem.item.stackSize + 1;

                    AddItem(second_item);

                    second_item.name = InventoryItem.item.name;

                    InventoryItem.item.gameObject.SetActive(false);
                }
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// Method for items auto stacking. Not used currently and not tested.
        /// </summary>
        /// <param name="InventoryItem"></param>
        public void AutoStack(InventoryItem InventoryItem)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            foreach (var i in inventoryItems)
            {
                if (i.item.id == InventoryItem.item.id)
                {
                    items.Add(i);
                }
            }

            foreach (var i in items)
            {
                if (InventoryItem.item.stackSize + i.item.stackSize <= InventoryItem.item.maxStackSize)
                {
                    InventoryItem.item.stackSize += InventoryItem.item.stackSize;
                    RemoveItem(i);
                }

                if (InventoryItem.item.stackSize == InventoryItem.item.maxStackSize)
                    break;
            }
        }

        /// <summary>
        /// We can use consumable items with this method. Stacked items will be decreased by one. Single item will be removed after use
        /// </summary>
        /// <param name="InventoryItem">Item to use</param>
        /// <param name="closeInventory">Should inventory be closed after use?</param>
        public void UseItem(InventoryItem InventoryItem, bool closeInventory)
        {
                // If not stackable
                if (!InventoryItem.item.stackable || InventoryItem.item.stackSize <= 1)
                {
                    InventoryItem.item.onUseEvent.Invoke();
                    RemoveItem(InventoryItem);
                }
                // If stackable
                else
                {
                    InventoryItem.item.onUseEvent.Invoke();
                    InventoryItem.item.stackSize -= 1;
                }
            

            if (closeInventory)
                InventoryManager.showInventory = false;
        }
    }
}

