/**********************************************************************************

// File Name :         LightBehavior.cs
// Author :            Marissa Moser
// Creation Date :     September 25, 2023
//
// Brief Description : 

**********************************************************************************/ 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    [SerializeField] private GameObject Bee;
    [SerializeField] private GameObject levelManager;
    private LevelManager lm;
    private GameObject hiveObject;
    private bool hiveInPlace;
    [SerializeField] private LayerMask hiveLM;
    [SerializeField] private Vector2 detectorSize;
    [SerializeField] private bool hivePlacedVertically;

    private void Start()
    {
        PlayerBehavior.ObjectDropped += LightBlocked;
        StartCoroutine(ConstantDetection());
        lm = levelManager.GetComponent<LevelManager>();
    }

    private void LightBlocked()
    {
        if (hiveInPlace)
        {
            if(!hivePlacedVertically && hiveObject.transform.rotation.z == 0 || hiveObject.transform.rotation.z == 180)
            {
                Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
                lm.BeeVisionObjects.Remove(hiveObject.transform.parent.gameObject);
                Destroy(hiveObject.transform.parent.gameObject);
                Destroy(gameObject);
            }
            //if (hivePlacedVertically && hiveObject.transform.rotation.z == 90 || hiveObject.transform.rotation.z == -90)
            //{
            //    Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            //    lm.BeeVisionObjects.Remove(hiveObject.transform.parent.gameObject);
            //    Destroy(hiveObject.transform.parent.gameObject);
            //    Destroy(gameObject);
            //}
        }
    }

    IEnumerator ConstantDetection()
    {
        while (true)
        {
            Collider2D collider = Physics2D.OverlapBox(transform.position, detectorSize, 0, hiveLM);
            if (collider != null)
            {
                //print(collider.gameObject);
                hiveInPlace = true;
                hiveObject = collider.gameObject;
                //print(hiveObject);
            }
            else
            {
                hiveInPlace = false;
                hiveObject = null;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && Bee.GetComponent<BeeStates>().StartInToPatrol)
        {
            Bee.GetComponent<BeeStates>().FSM(BeeStates.States.ToPatrol);
        }
    }

    private void OnDestroy()
    {
        PlayerBehavior.ObjectDropped -= LightBlocked;
    }
}
