/**********************************************************************************

// File Name :         CameraBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 3, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector2 cameraOffset;
    void FixedUpdate()
    {
        //transform.position = (player.transform.position.x, player.transform.position.y + 2f, 0f);
    }
}
