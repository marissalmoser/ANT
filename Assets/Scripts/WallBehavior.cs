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
        OnClimbableWall = false;
        //print(OnClimbableWall);

        if(collision.CompareTag("Player"))
        {
            //print("should switch");
            WallTriggered?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnClimbableWall = true;
        //print(OnClimbableWall);
    }
}
