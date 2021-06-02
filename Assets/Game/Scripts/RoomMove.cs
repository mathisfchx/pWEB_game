using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static PlayerMouvement;

namespace Game{
    public class RoomMove : MonoBehaviour
    {
        public Vector2 cameraChange;
        public Vector3 playerChange;
        //public CameraMovement cam;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OnTriggerEnter2D(Collider2D other)
        {   
                other.gameObject.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().minPosition += cameraChange; //il y a une erreur ici, peut etre due à l'utilisation du getcomponent sans typeof
                other.gameObject.GetComponent<PlayerMouvement>().cam.GetComponent<CameraMovement>().maxPosition += cameraChange;
                other.transform.position += playerChange;
        }
    }
}

