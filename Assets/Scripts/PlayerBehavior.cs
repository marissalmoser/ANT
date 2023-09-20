/**********************************************************************************

// File Name :         PlayerBehavior.cs
// Author :            Marissa Moser
// Creation Date :     September 19, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameManager GameManager;
    private GameManager gm;
    private PlayerController pc;

    private bool breakableTriggered;
    private GameObject breakableObject;

    void Start()
    {
        gm = GameManager.GetComponent<GameManager>();
        pc = gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (gm.BaseHead && breakableTriggered && pc.Interact)
        {
            print("breaking");
            Destroy(breakableObject);

            print(gm.BaseHead);
            print(breakableTriggered);
            print(pc.Interact);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if base head is true, and is hive to break
        if(collision.gameObject.CompareTag("Breakable"))
        {
            breakableTriggered = true;
            breakableObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if base head is true, and is hive to break
        if (collision.gameObject.CompareTag("Breakable"))
        {
            breakableTriggered = false;
            breakableObject = null;
        }
    }
}
