using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DTInventory
{
    /// <summary>
    /// Class for representation inventory grid items
    /// Contains all functionality for drag and drop, click etc.
    /// </summary>
    public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //A slot where item is
        [HideInInspector]
        public int x, y, width, height;

        /// <summary>
        /// Inventory item image component
        /// </summary>
        Image image;

        /// <summary>
        /// Reference to an item. Item and InventoryItem are not the same. InventoryItem it's a special component which allow us to draw item in inventory
        /// </summary>
        public Item item;

        /// <summary>
        /// Reference to a rect transform of InventoryItem image. Used for position control
        /// </summary>
        RectTransform m_rect;

        /// <summary>
        /// Reference to an inventory
        /// </summary>
        internal DTInventory inventory;

        /// <summary>
        /// Variable to store event data for some purposes
        /// </summary>
        PointerEventData dragEventData;

        /// <summary>
        /// Variable for drag state tracking
        /// </summary>
        bool drag;

        /// <summary>
        /// Store previos position of item to restore if placement on a new position was failed
        /// </summary>
        Vector2 lastPosition;

        /// <summary>
        /// Here we store actual transform of InventoryItem and interpolate value with this variable
        /// </summary>
        [HideInInspector]
        public Vector2 finalPosition;

        /// <summary>
        /// Reference to a text component to draw stack value if object is stackable
        /// </summary>
        public Text stackText;

        /// <summary>
        /// Stores gridslot reference when we dragging item and hover over it to draw blocked effect
        /// </summary>
        GridSlot hoveredSlot;

        private void OnEnable()
        {
            if (m_rect == null) m_rect = GetComponent<RectTransform>();
            if (inventory == null) inventory = FindObjectOfType<DTInventory>();
            if (image == null) image = GetComponent<Image>();
            if (stackText == null) stackText = GetComponentInChildren<Text>();
        }

        void Update()
        {
            if (drag)
            {
                m_rect.pivot =  new Vector2(Mathf.Lerp(m_rect.pivot.x, 0.5f, Time.deltaTime * 30), Mathf.Lerp(m_rect.pivot.y, 0.5f, Time.deltaTime * 30));
                m_rect.position = Vector2.Lerp(m_rect.position, dragEventData.position, Time.deltaTime * 30);
            }
            else
            {
                m_rect.pivot = new Vector2(Mathf.Lerp(m_rect.pivot.x, 0f, Time.deltaTime * 10), Mathf.Lerp(m_rect.pivot.y, 1f, Time.deltaTime * 10));
                m_rect.anchoredPosition = Vector2.Lerp(m_rect.anchoredPosition, finalPosition, Time.deltaTime * 10);
            }

            //Draw stack info if item stackable
            if (item != null && stackText != null && item.stackable)
            {
                if (item.stackSize > 1)
                    stackText.text = item.stackSize.ToString();
                else
                    stackText.text = string.Empty;
            }
        }

        //Here we processing event when click on item
        public void OnPointerClick(PointerEventData eventData)
        {
            //Right click to drop
            if (eventData.clickCount > 1 && eventData.button == PointerEventData.InputButton.Right)
            {
                inventory.DropItem(this);
                return;
            }

            //Substract stack event
            if (eventData.clickCount > 1 && eventData.button == PointerEventData.InputButton.Left && item.stackSize > 1)
            {
                if (inventory.CheckFreeSpaceForAllSlots(item.width, item.height))
                    inventory.SubstractStack(this);
            }
        }

        //Here we processing the moment when we start dragging an item
        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(inventory.transform);
            
            //Turn of raycastTarget to prevent unexpected raycasts
            image.raycastTarget = false;

            //Saving last item position to restore if something goes wrong
            lastPosition = m_rect.anchoredPosition;

            //Find occupied slot and setting it free
            var slot = inventory.FindSlotByIndex(x, y);

            //If InventoryItem in equipment slot, removing refernce for it on equip component
            if (slot.equipmentPanel != null)
            {
                if (slot.equipmentPanel.equipedItem != null)
                    slot.equipmentPanel.equipedItem = null;
            }

            //Marking slots as free
            inventory.MarkSlots(x, y, width, height, true);

            //inventory.DrawColorsForHoveredSlots(x, y, width, height);
            
            //To render our image above other UI elements, we put it transform to the end of our transforms list
            transform.SetAsLastSibling();

            //Make item image semi-transparent
            image.color = new Color(1, 1, 1, 0.5f);
        }
        
        // Here is the event callback of an item drag
        public void OnDrag(PointerEventData eventData)
        {
            //Hide cursor when dragging
            Cursor.visible = false;
            
            dragEventData = eventData;
            drag = true;

            //Getting gameobject that we hover on
            var hoveredGameObject = eventData.pointerCurrentRaycast.gameObject;

            if(hoveredGameObject != null)
            {
                //If we have not an gridslot hit and got inventoryitem component
                if(hoveredGameObject.GetComponent<InventoryItem>() != null && hoveredGameObject.GetComponent<GridSlot>() == null)
                {
                    inventory.DrawRegularSlotsColors();

                    var inv_item = hoveredGameObject.GetComponent<InventoryItem>();

                    if (inv_item.item.id != item.id)
                    {
                        //Look here we check if we should hovered item blocked fully or not
                        //If 

                        if(inv_item.x > x || inv_item.y > y)
                            inventory.DrawColorsForHoveredSlots(inv_item.x, inv_item.y, inv_item.width, inv_item.height);
                        else if(inv_item.x == x && inv_item.y == y)
                            inventory.DrawColorsForHoveredSlots(inv_item.x, inv_item.y, width, height);
                        else
                            inventory.DrawColorsForHoveredSlots(inv_item.x, inv_item.y, width, height);
                    }
                    else
                    {
                        inventory.DrawColorForStackableHoveredSlots(inv_item.x, inv_item.y, inv_item.width, inv_item.height);
                    }
                    
                }
                else if(hoveredGameObject.GetComponent<InventoryItem>() == null && hoveredGameObject.GetComponent<GridSlot>() != null)
                {
                    if (hoveredSlot != hoveredGameObject.GetComponent<GridSlot>())
                    {
                        hoveredSlot = hoveredGameObject.GetComponent<GridSlot>();
                    }
                    else if (hoveredSlot == hoveredGameObject.GetComponent<GridSlot>() && hoveredGameObject.GetComponent<GridSlot>().equipmentPanel != null)
                    {
                        inventory.DrawRegularSlotsColors();
                        inventory.DrawColorsForHoveredSlots(CalculateShiftedAxes().x, CalculateShiftedAxes().y, width, height);
                    }
                    else if (hoveredGameObject.GetComponent<GridSlot>().equipmentPanel == null)
                    {
                        inventory.DrawRegularSlotsColors();
                        inventory.DrawColorsForHoveredSlots(CalculateShiftedAxes().x, CalculateShiftedAxes().y, width, height);
                    }
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            image.color = Color.white;

            Cursor.visible = true;

            image.raycastTarget = true;
            
            drag = false;
            
            inventory.DrawRegularSlotsColors();

            var eventDataRaycast = eventData.pointerCurrentRaycast;
            
            if (eventDataRaycast.gameObject == null && inventory.dragItemOutsideToDrop)
            {
                inventory.DropItem(this);
                return;
            }

            if (eventDataRaycast.gameObject != null 
                && item != null
                && item.stackable == true
                && eventDataRaycast.gameObject.GetComponent<InventoryItem>() != null
                && eventDataRaycast.gameObject.GetComponent<InventoryItem>().item.id == item.id
                && eventDataRaycast.gameObject.GetComponent<InventoryItem>().item.stackSize + item.stackSize <= item.maxStackSize)
            {
                eventDataRaycast.gameObject.GetComponent<InventoryItem>().item.stackSize += item.stackSize;
                inventory.RemoveItem(this);
                Destroy(gameObject);
                return;
            }

            if (eventDataRaycast.gameObject != null && eventDataRaycast.gameObject.GetComponent<GridSlot>())
            {
                var slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<GridSlot>();

                if (inventory.CheckFreeSpaceAtSlot(CalculateShiftedAxes().x, CalculateShiftedAxes().y, width, height) && slot.free && slot.equipmentPanel == null)
                {
                    transform.SetParent(slot.transform.parent);

                    finalPosition = inventory.FindSlotByIndex(CalculateShiftedAxes().x, CalculateShiftedAxes().y).GetComponent<RectTransform>().anchoredPosition;

                    x = CalculateShiftedAxes().x;
                    y = CalculateShiftedAxes().y;

                    inventory.MarkSlots(CalculateShiftedAxes().x, CalculateShiftedAxes().y, width, height, false);
                }
                else if (inventory.CheckFreeSpaceAtSlot(CalculateShiftedAxes().x, CalculateShiftedAxes().y, width, height) && slot.free && slot.equipmentPanel != null && slot.equipmentPanel.allowedItemType == item.type)
                {
                    transform.SetParent(slot.transform.parent);

                    finalPosition = inventory.FindSlotByIndex(CalculateShiftedAxes().x, CalculateShiftedAxes().y).GetComponent<RectTransform>().anchoredPosition;

                    x = CalculateShiftedAxes().x;
                    y = CalculateShiftedAxes().y;

                    slot.equipmentPanel.equipedItem = item;

                    inventory.MarkSlots(CalculateShiftedAxes().x, CalculateShiftedAxes().y, width, height, false);
                }
                else
                {
                    finalPosition = lastPosition;
                    inventory.MarkSlots(x, y, width, height, false);
                }
            }
            else
            {
                finalPosition = lastPosition;
                inventory.MarkSlots(x, y, width, height, false);
            }
        }
        
        public Vector2Int CalculateShiftedAxes()
        {
            int mod_x = 0, mod_y = 0;

            if (width == 1) mod_x = 1;
            if (width / width == 0) mod_x = hoveredSlot.x - (width / 2);
            else if (width / width != 0) mod_x = hoveredSlot.x - ((width - 1) / 2);

            if (height == 1) mod_y = 1;
            if (height / height == 0) mod_y = hoveredSlot.y - (height / 2);
            else if (height / height != 0) mod_y = hoveredSlot.y - ((height - 1) / 2);

            return new Vector2Int(mod_x, mod_y);
        }
        
    }
}
