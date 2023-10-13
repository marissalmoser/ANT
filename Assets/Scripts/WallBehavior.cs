using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    public static bool OnClimbableWall;
    // Start is called before the first frame update
    void Start()
    {
        OnClimbableWall = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnClimbableWall = false;
        print(OnClimbableWall);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnClimbableWall = true;
        print(OnClimbableWall);

        //make it an event instead of static bool so that 
    }
}
