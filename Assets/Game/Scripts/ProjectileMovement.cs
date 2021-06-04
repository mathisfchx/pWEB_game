using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 

public class ProjectileMovement : NetworkBehaviour
{
    public float speed;
    public float TimeToLive;
    public Vector3 mouse_position;
    public int team; 

    void Awake()
    {
        speed = 15   ;
        TimeToLive = 2;
        //cam = new Camera();
        //new WaitForSeconds((float)0.2);
        //print(mouse_position);
        //print(cam.transform.position);
    }
    void Start()
    {
        Invoke("DestroyProjectile", TimeToLive);
        print(Camera.main.transform.position);
        mouse_position = Input.mousePosition;
        //mouse_position.z = Camera.main.nearClipPlane;
        mouse_position = Camera.main.ScreenToWorldPoint(mouse_position)-transform.position;
        print(mouse_position);
        print(mouse_position.normalized);

    }


    void Update()
    {

        transform.Translate(new Vector2(mouse_position.x , mouse_position.y).normalized*speed * Time.deltaTime) ;
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
