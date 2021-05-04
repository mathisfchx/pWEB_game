// Creates a simple wizard that lets you create a Light GameObject
// or if the user clicks in "Apply", it will set the color of the currently
// object selected to red

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DTInventory
{
    public class InventoryWizard : EditorWindow
    {
        #region variables

        //Inventory

        private int SizeX, SizeY;
        private int CellRectSize;
        private int CellSpacing;

        private Sprite CellImage;
        private Sprite InventoryBackground;

        private bool HasEquipmentSlots = false;
        private int EquipmentSlotsNumber;

        private Color normalCellColor = Color.white, hoveredCellColor = Color.grey, blockedCellCover = Color.red;
        private Color inventoryBackgroundColor = Color.white;

        private Color stackTextColor = Color.gray;
        private Font stackTextFont;

        //Loot window

        private Vector2 inventoryRectSize;
        private int LootSizeX, LootSizeY;

        //Equipment panels

        private int equipmentPanelsCount;
        int[] equipmentPanelX;
        int[] equipmentPanelY;
        string[] equipmentPanelType;

        public int currentEquipmentPanelsCount;

        #endregion

        public int tabIndex = 0;
        public string[] tabHeaders = new string[] { "Inventory", "Loot window", "Equipment panels" };

        public Transform inventoryTransform;
        
        DTInventory inventory;

        [MenuItem("DT Inventory/New Inventory")]
        static void Init()
        {
            InventoryWizard _editor = (InventoryWizard)GetWindow(typeof(InventoryWizard));

            _editor.Show();
        }

        void OnGUI()
        {
            tabIndex = GUILayout.Toolbar(tabIndex, tabHeaders);

            if (tabIndex == 0)
            {
                EditorGUILayout.HelpBox("Welcome to Inventory Wizard. With this tool you can create your own inventory. Watch video and follow steps", MessageType.Info);

                EditorGUILayout.LabelField("Inventory & Cell size", EditorStyles.centeredGreyMiniLabel);

                GUILayout.BeginVertical("HelpBox"); GUILayout.BeginVertical("GroupBox");
                
                SizeX = EditorGUILayout.IntSlider("Inventory Horizontal size", SizeX, 1, 100);
                SizeY = EditorGUILayout.IntSlider("Inventory Vertical size", SizeY, 1, 100);

                normalCellColor = EditorGUILayout.ColorField("Normal cell color", normalCellColor);
                hoveredCellColor = EditorGUILayout.ColorField("Hovered cell color", hoveredCellColor);
                blockedCellCover = EditorGUILayout.ColorField("Blocked cell color", blockedCellCover);

                stackTextFont = (Font)EditorGUILayout.ObjectField("Stack text font", stackTextFont, typeof(Font), false);
                stackTextColor = EditorGUILayout.ColorField("Stack text color", stackTextColor);

                EditorGUILayout.LabelField("");

                CellRectSize = EditorGUILayout.IntSlider("Cell size", CellRectSize, 1, 200);

                //CellSpacing = EditorGUILayout.IntSlider("Space between cells", CellSpacing, 0, 20);

                CellImage = (Sprite)EditorGUILayout.ObjectField("Cell image", CellImage, typeof(Sprite), false);

                inventoryBackgroundColor = EditorGUILayout.ColorField("Background color", inventoryBackgroundColor);
                InventoryBackground = (Sprite)EditorGUILayout.ObjectField("Inventory background", InventoryBackground, typeof(Sprite), false);

                if (GUILayout.Button("Build inventory"))
                {
                    var eventSystem = Instantiate(new GameObject());
                    eventSystem.AddComponent<EventSystem>();
                    eventSystem.AddComponent<StandaloneInputModule>();
                    eventSystem.AddComponent<BaseInput>();
                    eventSystem.name = "Event System";

                    CleanUp();

                    if (CellImage == null)
                    {
                        EditorUtility.DisplayDialog("Setup uncompleted", " Please attach sprite to cell image field. Otherwise, inventory cells will be invisible", "Continue");
                        return;
                    }

                    // Drawing Canvas
                    var obj = Instantiate(new GameObject());
                    obj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    obj.AddComponent<CanvasScaler>();
                    obj.AddComponent<GraphicRaycaster>();
                    obj.AddComponent<InventoryManager>();

                    eventSystem.gameObject.transform.parent = obj.transform;

                    obj.name = "Inventory Canvas";


                    // Drawing Inventory (cell holder in context)

                    var cellHolder = Instantiate(new GameObject());
                    cellHolder.transform.SetParent(obj.transform);
                    cellHolder.name = "Inventory";

                    inventoryTransform = obj.transform;

                    var cellHolderImage = cellHolder.AddComponent<Image>();
                    cellHolderImage.rectTransform.sizeDelta = new Vector2((CellRectSize + CellSpacing) * SizeX, (CellRectSize + CellSpacing) * SizeY);
                    cellHolderImage.rectTransform.anchoredPosition = Vector2.zero;
                    cellHolderImage.color = inventoryBackgroundColor;

                    inventoryRectSize = cellHolderImage.rectTransform.sizeDelta;

                    var inventory = cellHolder.AddComponent<DTInventory>();
                    inventory.cellSize = CellRectSize;
                    inventory.padding = CellSpacing;
                    inventory.column = SizeX;
                    inventory.row = SizeY;

                    inventory.normalCellColor = normalCellColor;
                    inventory.blockedCellColor = blockedCellCover;
                    inventory.hoveredCellColor = hoveredCellColor;

                    // Making inventory grid cell object

                    var imgObj = Instantiate(new GameObject());
                    imgObj.AddComponent<Image>().sprite = CellImage;
                    imgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(CellRectSize, CellRectSize);
                    imgObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                    imgObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                    imgObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                    imgObj.transform.SetParent(obj.transform);

                    imgObj.GetComponent<Image>().type = Image.Type.Sliced;
                    imgObj.GetComponent<Image>().color = Color.white;

                    imgObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 3000);
                    imgObj.AddComponent<GridSlot>();
                    
                    imgObj.name = "Utility object (Don't delete!)";

                    var stackText = Instantiate(new GameObject());
                    stackText.transform.SetParent(imgObj.transform);

                    var stackTextComponent = stackText.AddComponent<Text>();
                    stackTextComponent.alignment = TextAnchor.LowerLeft;
                    stackTextComponent.raycastTarget = false;
                    stackTextComponent.rectTransform.anchorMin = new Vector2(0, 0);
                    stackTextComponent.rectTransform.anchorMax = new Vector2(0, 0);
                    stackTextComponent.rectTransform.anchoredPosition = new Vector2(53, 53);
                    stackTextComponent.color = stackTextColor;
                    if (stackTextFont != null)
                        stackTextComponent.font = stackTextFont;

                    // Drawing Inventory view

                    inventory.cell = imgObj.GetComponent<Image>();
                    inventory.DrawPreview();

                    CleanUp(); CleanUp(); CleanUp(); CleanUp();

                    //this.Close();
                }

                GUILayout.EndVertical();

                GUILayout.EndVertical();
            }

            if (tabIndex == 1)
            {
                inventory = FindObjectOfType<DTInventory>();

                if(inventory == null)
                {
                    EditorGUILayout.HelpBox("No inventory found in the scene. You must create inventory first, in order to create loot window", MessageType.Error);
                }else
                {
                    GUILayout.BeginVertical("GroupBox");

                    LootSizeX = EditorGUILayout.IntField("Loot Horizontal size", LootSizeX);
                    LootSizeY = EditorGUILayout.IntField("Loot Vertical size", LootSizeY);

                    if (GUILayout.Button("Build Loot Window"))
                    {
                        var cellHolder = Instantiate(new GameObject());
                        cellHolder.transform.SetParent(inventoryTransform);
                        cellHolder.name = "Loot window";

                        var cellHolderImage = cellHolder.AddComponent<Image>();
                        cellHolderImage.rectTransform.sizeDelta = new Vector2((CellRectSize + CellSpacing) * LootSizeX, (CellRectSize + CellSpacing) * LootSizeY);
                        cellHolderImage.rectTransform.anchoredPosition = Vector2.zero;
                        cellHolderImage.color = inventoryBackgroundColor;

                        cellHolderImage.rectTransform.anchoredPosition = new Vector2(inventoryRectSize.x, 0);

                        inventory.lootPanel = cellHolder.GetComponent<RectTransform>();
                        inventory.lootRow = LootSizeX;
                        inventory.lootColumn = LootSizeY;

                        //DestroyImmediate(GameObject.Find("Inventory Canvas"));
                        inventory.DrawPreview();
                        CleanUp();
                    }
                }
            }

            if (tabIndex == 2)
            {
                if (inventory == null)
                {
                    EditorGUILayout.HelpBox("No inventory found in the scene. You must create inventory first, in order to create equipment panels", MessageType.Error);
                }
                else
                {
                    equipmentPanelsCount = EditorGUILayout.IntSlider("Equipment panels count", equipmentPanelsCount, 1, 20);

                    if (currentEquipmentPanelsCount != equipmentPanelsCount)
                    {
                        equipmentPanelX = new int[equipmentPanelsCount];
                        equipmentPanelY = new int[equipmentPanelsCount];
                        equipmentPanelType = new string[equipmentPanelsCount];

                        currentEquipmentPanelsCount = equipmentPanelsCount;
                    }

                    for (int i = 0; i < equipmentPanelsCount; i++)
                    {
                        GUILayout.BeginVertical("HelpBox");
                        GUILayout.Label("Equipment panel " + (i + 1) + " :" + equipmentPanelType[i], EditorStyles.boldLabel);

                        GUILayout.Label("");

                        equipmentPanelX[i] = EditorGUILayout.IntField("Equipment Horizontal size", equipmentPanelX[i]);

                        if (equipmentPanelX[i] == 0)
                            equipmentPanelX[i] = 1;

                        equipmentPanelY[i] = EditorGUILayout.IntField("Equipment Vertical size", equipmentPanelY[i]);

                        if (equipmentPanelY[i] == 0)
                            equipmentPanelY[i] = 1;

                        equipmentPanelType[i] = EditorGUILayout.TextField("Allowed item type", equipmentPanelType[i]);
                        GUILayout.EndVertical();
                    }

                    if (GUILayout.Button("Build panels"))
                    {
                        inventory.equipmentPanels = new List<EquipmentPanel>();

                        for (int i = 0; i < equipmentPanelsCount; i++)
                        {
                            var cellHolder = Instantiate(new GameObject());
                            cellHolder.transform.SetParent(inventoryTransform);
                            cellHolder.name = "Equiment panel :"+equipmentPanelType[i];
                            var equipmentPanel = cellHolder.AddComponent<EquipmentPanel>();

                            equipmentPanel.width = equipmentPanelX[i];
                            equipmentPanel.height = equipmentPanelY[i];
                            equipmentPanel.allowedItemType = equipmentPanelType[i];
                            
                            var cellHolderImage = cellHolder.AddComponent<Image>();
                            cellHolderImage.rectTransform.sizeDelta = new Vector2((CellRectSize + CellSpacing) * equipmentPanelX[i], (CellRectSize + CellSpacing) * equipmentPanelY[i]);
                            cellHolderImage.rectTransform.anchoredPosition = Vector2.zero;
                            cellHolderImage.color = inventoryBackgroundColor;
                            
                            inventory.equipmentPanels.Add(equipmentPanel);

                            //cellHolderImage.rectTransform.anchoredPosition = new Vector2(inventoryRectSize.x, 0);
                            
                            //DestroyImmediate(GameObject.Find("Inventory Canvas"));
                            
                            CleanUp();
                        }

                        inventory.DrawPreview();
                    }
                }
            }

        }

        public void CleanUp()
        {
            var garbage = GameObject.Find("New Game Object");

            DestroyImmediate(garbage);
        }

    }
}