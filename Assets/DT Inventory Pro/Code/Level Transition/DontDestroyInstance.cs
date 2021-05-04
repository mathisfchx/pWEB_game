using UnityEngine;

public class DontDestroyInstance : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
