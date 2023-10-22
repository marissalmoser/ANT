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
    [SerializeField] private bool queensLight;
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
            print("dropped");

            if (queensLight)
            {
                Bee.GetComponent<QueenBeeBehavior>().LightShutOff();
            }
            else
            {
                Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            }

            lm.BeeVisionObjects.Remove(hiveObject.transform.parent.gameObject);
            Destroy(hiveObject.transform.parent.gameObject);
            Destroy(gameObject);

            //if (!hivePlacedVertically && hiveObject.transform.parent.transform.rotation.z == 0)
            //{
            //    print("horiz 0");
            //    if (queensLight)
            //    {
            //        Bee.GetComponent<QueenBeeBehavior>().LightShutOff();
            //    }
            //    else
            //    {
            //        Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            //    }

            //    lm.BeeVisionObjects.Remove(hiveObject);
            //    Destroy(hiveObject.transform.parent.gameObject);
            //    Destroy(gameObject);
            //}
            //else if (!hivePlacedVertically/* && hiveObject.transform.parent.transform.rotation.z == 180*/)
            //{
            //    print("horiz 180");
            //    if (queensLight)
            //    {
            //        Bee.GetComponent<QueenBeeBehavior>().LightShutOff();
            //    }
            //    else
            //    {
            //        Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            //    }

            //    lm.BeeVisionObjects.Remove(hiveObject);
            //    Destroy(hiveObject.transform.parent.gameObject);
            //    Destroy(gameObject);
            //}

            //if (hivePlacedVertically)
            //{
            //    print("vert");
            //    if(hiveObject.transform.parent.transform.rotation.z == 90 || hiveObject.transform.parent.transform.rotation.z == -90)
            //    {
            //        print("vert 90");
            //        if (queensLight)
            //        {
            //            Bee.GetComponent<QueenBeeBehavior>().LightShutOff();
            //        }
            //        else
            //        {
            //            Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            //        }

            //        lm.BeeVisionObjects.Remove(hiveObject.transform.parent.gameObject);
            //        Destroy(hiveObject.transform.parent.gameObject);
            //        Destroy(gameObject);
            //    }
                
            //}
            //else if (hivePlacedVertically && hiveObject.transform.parent.transform.rotation.z == -90)
            //{
            //    print("vert -90");
            //    if (queensLight)
            //    {
            //        Bee.GetComponent<QueenBeeBehavior>().LightShutOff();
            //    }
            //    else
            //    {
            //        Bee.GetComponent<BeeStates>().FSM(BeeStates.States.Sleep);
            //    }

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
                hiveInPlace = true;
                hiveObject = collider.gameObject;   //bee vision object
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
        if(!queensLight && collision.CompareTag("Player") && Bee.GetComponent<BeeStates>().StartInToPatrol)
        {
            Bee.GetComponent<BeeStates>().FSM(BeeStates.States.ToPatrol);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, detectorSize);
    }

    private void OnDestroy()
    {
        PlayerBehavior.ObjectDropped -= LightBlocked;
    }
}
