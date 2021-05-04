using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTInventory
{
    public class LevelTransition : MonoBehaviour
    {
        public int sceneId;

        private void OnTriggerEnter(Collider other)
        {
            FindObjectOfType<SaveData>().SaveLevelPeristence();
            SceneManager.LoadScene(sceneId);
        }
    }
}