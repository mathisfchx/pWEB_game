using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMouvement;

public class RoomMove : MonoBehaviour
{
    //public Vector2 cameraChange;
    //public Vector3 playerChange;
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
    {   /*
        if(other.CompareTag("Player")){
            other.gameObject.GetComponent<PlayerMouvement>().cam.minPosition += cameraChange;
            other.gameObject.GetComponent<PlayerMouvement>().cam.maxPosition += cameraChange;
            other.transform.position += playerChange;
        }*/
    }
}
