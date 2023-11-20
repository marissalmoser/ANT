/**********************************************************************************

// File Name :         BeeStates.cs
// Author :            Marissa Moser
// Creation Date :     September 24, 2023
//
// Brief Description : This script manages the bee vision error sparks.

**********************************************************************************/
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
