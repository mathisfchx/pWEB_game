using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DTInventory
{
    [CustomEditor(typeof(DTInventory))]
    public class InventoryExtended : Editor
    {
        DTInventory inventory;

        private void OnEnable()
        {
            inventory = FindObjectOfType<DTInventory>();
        }

        public override void OnInspectorGUI()
        {
            Editor editor = Editor.CreateEditor(inventory);
            editor.DrawDefaultInspector();

            if (GUILayout.Button("Clear scene persistent data"))
            {
                FindObjectOfType<SaveData>().ClearScenePersistence();
            }
        }
    }
}