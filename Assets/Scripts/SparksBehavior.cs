using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparksBehavior : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyGO", 1);
    }

    void DestroyGO()
    {
        Destroy(gameObject);
    }
}
