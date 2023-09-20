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

    [SerializeField] private GameObject beeVision;
    [SerializeField] private GameObject spotToCarry;

    void Start()
    {
        gm = GameManager.GetComponent<GameManager>();
        pc = gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        //Breaks objects
        if (gm.BaseHead && breakableTriggered && pc.Interact)
        {
            print("breaking");
            Destroy(breakableObject);
        }

        //Bee Vision
        if(!gm.BaseHead)
        {
            beeVision.SetActive(true);
        }
        if (gm.BaseHead)
        {
            beeVision.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //triggeres a breakable object
        if(collision.gameObject.CompareTag("Breakable"))
        {
            breakableTriggered = true;
            breakableObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //moved away from breakable object
        if (collision.gameObject.CompareTag("Breakable"))
        {
            breakableTriggered = false;
            breakableObject = null;
        }
    }
}
