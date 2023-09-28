/**********************************************************************************

// File Name :         GameManager.cs
// Author :            Marissa Moser
// Creation Date :     September 18, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool BaseHead;
    public bool BaseLeg;

    public List<GameObject> BeeVisionObjects = new List<GameObject>();

    void Start()
    {
        BaseHead = true;
        BaseLeg = true; 
    }
}
