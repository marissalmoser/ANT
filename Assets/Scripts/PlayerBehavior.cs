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
    //components and GOs
    public GameManager GameManager;
    private GameManager gm;
    private PlayerController pc;

    //breaking object vars
    [Header("Breaking Objects")]
    private bool breakableTriggered;
    private GameObject breakableObject;

    //bee vision vars
    [Header("Bee Vision")]
    [SerializeField] private GameObject beeVision;

    //carrying object vars
    [Header("Carrying Objects")]
    [SerializeField] private Transform spotToCarry;
    private GameObject pickUpObject;
    private bool pickUpTriggered;
    private bool canPickUpObj = true;

    [Header("Web Platforms")]
    [HideInInspector] public GameObject WebPlatform;
    [SerializeField] private GameObject WebPlatformPrefab;
    //public int PlatformCount;

    void Start()
    {
        gm = GameManager.GetComponent<GameManager>();
        pc = gameObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        //Breaks Objects
        if (gm.BaseHead && breakableTriggered && pc.Interact)
        {
            //print("breaking");
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

        //triggeres a pickup-able object
        if (collision.gameObject.CompareTag("PickUp-able"))
        {
            pickUpTriggered = true;
            pickUpObject = collision.gameObject;
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

        //triggeres a pickup-able object
        if (collision.gameObject.CompareTag("PickUp-able"))
        {
            pickUpTriggered = false;
        }
    }

    public void PickUpObject()
    {
        //picking up
        if (pickUpTriggered && canPickUpObj)
        {
            pickUpObject.transform.position = spotToCarry.position;
            pickUpObject.transform.parent = gameObject.transform;
            canPickUpObj = false;
            pickUpTriggered = false;
        }
        //dropping
        else if (!canPickUpObj && pickUpObject != null)
        {
            pickUpObject.transform.parent = null;
            canPickUpObj = true;
            pickUpObject = null;
        }
    }

    public void SpawnWebPlatform()
    {
        if (gm.WebPlatformList.Count < 3 && !gm.BaseLeg)
        {
            WebPlatform = Instantiate(WebPlatformPrefab, spotToCarry.position, transform.rotation);
            gm.WebPlatformList.Add(WebPlatform);
            //print(gm.WebPlatformList.Count); 
        }
    }
}
