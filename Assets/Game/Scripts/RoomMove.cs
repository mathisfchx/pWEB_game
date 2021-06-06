using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game{
    public class RoomMove : MonoBehaviour
    {
        public Vector2 cameraChange;
        public Vector3 playerChange;

        void Start()
        {

        }

        void Update()
        {
            
        }

        public void OnTriggerEnter2D(Collider2D other)
        {   
                other.gameObject.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition += cameraChange; 
                other.gameObject.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition += cameraChange;
                other.transform.position += playerChange;
        }
    }
}

