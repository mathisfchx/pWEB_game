using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 

public class ProjectileMovement : NetworkBehaviour
{
    public float speed;
    public float TimeToLive;
    public Vector3 mouse_position;

    void Awake()
    {
        speed = 1   ;
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
        mouse_position.z = Camera.main.nearClipPlane;
        mouse_position = Camera.main.ScreenToWorldPoint(mouse_position);
        print(mouse_position);
    }


    void Update()
    {
        transform.Translate(mouse_position * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
