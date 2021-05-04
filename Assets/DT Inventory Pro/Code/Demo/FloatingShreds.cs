using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingShreds : MonoBehaviour
{
    public float min = 2f;
    public float max = 3f;
    // Use this for initialization
    void Start()
    {

        min = transform.position.x;
        max = transform.position.x + Random.Range(0.1f, 2f);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * 0.4f, max - min) + min, transform.position.z);
    }
}
