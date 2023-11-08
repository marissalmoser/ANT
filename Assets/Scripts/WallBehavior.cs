/**********************************************************************************

// File Name :         WallBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 12, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallBehavior : MonoBehaviour
{
    public static bool OnClimbableWall;
    public static Action WallTriggered;

    // Start is called before the first frame update
    void Start()
    {
        OnClimbableWall = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            OnClimbableWall = false;
            //print(OnClimbableWall);
            WallTriggered?.Invoke();
            AudioManager.Instance.Play("CrawlToWalkFall");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnClimbableWall = true;
        //print(OnClimbableWall);
    }
}
