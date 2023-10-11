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
using System;

public class PlayerBehavior : MonoBehaviour
{
    //components and GOs
    private PlayerController pc;

    //breaking object vars
    [Header("Breaking Objects")]
    private bool breakableTriggered;
    private GameObject breakableObject;

    //bee vision vars
    //[Header("Bee Vision")]

    //carrying object vars
    [Header("Carrying Objects")]
    [SerializeField] private Transform spotToCarry;
    private GameObject pickedUpObject;
    private bool pickUpTriggered;
    private bool isCarrying;
    private Vector3 crawlCarryOffset = new Vector3(0, -0.12f, 0);
    private Vector3 walkCarryOffset = new Vector3(0, 0.35f, 0);
    public static Action ObjectDropped;

    [Header("Web Platforms")]
    [HideInInspector] public GameObject WebPlatform;
    [SerializeField] private GameObject WebPlatformPrefab;

    void Start()
    {
        pc = gameObject.GetComponent<PlayerController>();
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        //triggeres a pickup-able object
        if (collision.gameObject.CompareTag("PickUp-able"))
        {
            pickUpTriggered = true;
            //print(pickUpTriggered);

            if (!isCarrying)
            {
                pickedUpObject = collision.gameObject;
            }
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
            //print(pickUpTriggered);
        }
    }

    public void PickUpObject()
    {
        //picking up
        if (pickUpTriggered && !isCarrying)
        {
            //print("pick up");
            isCarrying = true;
            StartCoroutine(MovePickedUpObjeect());
        }
        //dropping
        else if (pickedUpObject != null)
        {
            print("drop");
            isCarrying = false;
            ObjectDropped?.Invoke();
        }
    }

    public void BreakObject()
    {
        if (GameManager.Instance.BaseHead && breakableTriggered)
        {
            //print("breaking");
            Destroy(breakableObject);
        }
    }

    IEnumerator MovePickedUpObjeect()
    {
        while(isCarrying)
        {
            if(pc.CrawlMapEnabled && pickedUpObject != null)
            {
                pickedUpObject.transform.position = transform.position + crawlCarryOffset;
                pickedUpObject.transform.rotation = transform.rotation;
            }
            else if(pickedUpObject != null)
            {
                pickedUpObject.transform.position = transform.position + walkCarryOffset;
            }
            yield return null;
        }

        pickedUpObject = null;
        StopCoroutine(MovePickedUpObjeect());
    }

    public void SpawnWebPlatform()
    {
        if (GameManager.Instance.WebPlatformList.Count < 3 && !GameManager.Instance.BaseLeg)
        {
            WebPlatform = Instantiate(WebPlatformPrefab, spotToCarry.position, transform.rotation);
            GameManager.Instance.WebPlatformList.Add(WebPlatform);
            //print(gm.WebPlatformList.Count); 
        }
    }
}
