using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DTInventory
{
    public class ItemEditor : EditorWindow
    {
        [MenuItem("DT Inventory/Create Item")]
        static void Init()
        {
            ItemEditor _editor = (ItemEditor)GetWindow(typeof(ItemEditor));
            _editor.Show();
        }

        GameObject obj;
        GameObject finalObj;
        
        int index = 0;
        bool generateCollider;
        bool addRigidbody;
        private void OnGUI()
        {
            if (index == 0)
            {
                GUILayout.TextArea("New item", EditorStyles.boldLabel);
                GUILayout.BeginVertical("HelpBox");
                obj = (GameObject)EditorGUILayout.ObjectField("Item model", obj, typeof(GameObject), true);
                EditorGUILayout.HelpBox("Collider will generated only if object has MeshFilter component! If it is not, you can create and edit collider manually (use primitives)", MessageType.Info);
                generateCollider = EditorGUILayout.Toggle("Generate collider?", generateCollider);
                addRigidbody = EditorGUILayout.Toggle("Add rigidbody?", addRigidbody);

                if (GUILayout.Button("Next"))
                {
                    finalObj = Instantiate(obj);
                    finalObj.AddComponent<Item>();
                    finalObj.tag = "Item";

                    if(generateCollider)
                    {
                        finalObj.AddComponent<MeshCollider>().convex = true;
                    }
                    if(addRigidbody)
                    {
                        var rb = finalObj.AddComponent<Rigidbody>();
                        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        rb.interpolation = RigidbodyInterpolation.Interpolate;
                    }

                    finalObj.name = obj.name + " - Item";

                    Selection.activeGameObject = finalObj;
                    SceneView.lastActiveSceneView.FrameSelected();

                    index++;
                }
            }
            else if(index == 1)
            {
                GUILayout.TextArea("New item", EditorStyles.boldLabel);
                GUILayout.BeginVertical("HelpBox");
                

                if (finalObj != null)
                {
                    Editor editor = Editor.CreateEditor(finalObj.GetComponent<Item>());
                    editor.OnInspectorGUI();
                }
                else
                {
                    EditorGUILayout.HelpBox("Can't find item. Maybe reference is missed. Try again from first step", MessageType.Warning, true);
                }

                GUILayout.EndVertical();

                EditorGUILayout.HelpBox("Don't forget to save your item settings and prefab itself! Drag prefab to the items folder.", MessageType.Warning);
            }
            

            //shotAnimation = (Game)EditorGUILayout.ObjectField("Shot animation", shotAnimation, typeof(AnimationClip), false);
        }
    }
}