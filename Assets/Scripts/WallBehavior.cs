/**********************************************************************************

// File Name :         WallBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 12, 2023
//
// Brief Description : This script checks if the player is on a climbable wall
when climbing, and then switches the player to walking movement system.

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallBehavior : MonoBehaviour
{
    public static bool OnClimbableWall;
    public static Action WallTriggered;

    void Start()
    {
        OnClimbableWall = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            OnClimbableWall = false;
            
            if (PlayerController.PlayerCanCrawl)
            {
                WallTriggered?.Invoke();
                AudioManager.Instance.Play("CrawlToWalkFall");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnClimbableWall = true;
    }
}
