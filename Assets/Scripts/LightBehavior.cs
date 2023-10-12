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
    private GameObject hiveObject;
    private bool hiveInPlace;
    [SerializeField] private LayerMask hiveLM;
    [SerializeField] private Vector2 detectorSize;

    private void Start()
    {
        PlayerBehavior.ObjectDropped += LightBlocked;
        StartCoroutine(ConstantDetection());
    }

    private void LightBlocked()
    {
        if (hiveInPlace)
        {
            Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            Destroy(hiveObject.transform.parent.gameObject);
            Destroy(gameObject);
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

    private void OnDestroy()
    {
        PlayerBehavior.ObjectDropped -= LightBlocked;
    }
}
