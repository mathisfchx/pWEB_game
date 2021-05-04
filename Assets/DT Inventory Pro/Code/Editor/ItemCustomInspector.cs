using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DTInventory
{
    [CustomEditor(typeof(Item))]
    public class ItemCustomInspector : Editor
    {
        Item item;
        Item cachedItem;

        public override void OnInspectorGUI()
        {
            item = target as Item;
            cachedItem = item;
            
            if(item)
            DrawGeneralItem();
            
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            style.fontStyle = FontStyle.Bold;

            if (GUILayout.Button("Save changes?", style))
            {
                EditorUtility.SetDirty(item);
                EditorSceneManager.MarkSceneDirty(item.gameObject.scene);
            }
        }

        public void DrawGeneralItem()
        {
            GUILayout.Label("General item settings", EditorStyles.boldLabel);
            GUILayout.BeginVertical("HelpBox");
            item.title = EditorGUILayout.TextField("Name", item.title);
            item.description = EditorGUILayout.TextField("Description", item.description);
            item.type = EditorGUILayout.TextField("Type", item.type);
            item.icon = (Sprite)EditorGUILayout.ObjectField("Item icon", item.icon, typeof(Sprite), false);
            item.id = EditorGUILayout.IntField("ID", item.id);

            if (GUILayout.Button("Generate random ID?"))
            {
                item.id = Random.Range(0, int.MaxValue);
            }

            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Item grid size", EditorStyles.boldLabel);
            item.width = EditorGUILayout.IntField("Width", item.width);
            item.height = EditorGUILayout.IntField("Height", item.height);
            GUILayout.EndVertical();
            
                item.stackable = EditorGUILayout.Toggle("Stackable", item.stackable);

                if (item.stackable)
                {
                    item.stackSize = EditorGUILayout.IntSlider("Item stack size", item.stackSize, 1, 100);
                    item.maxStackSize = EditorGUILayout.IntSlider("Max stack size", item.maxStackSize, 1, 100);
                }
            

            GUILayout.EndVertical();
        }


    }
}
