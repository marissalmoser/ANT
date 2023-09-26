using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehavior : MonoBehaviour
{
    [SerializeField] private GameObject Bee;
    private GameObject hiveObject;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp-able"))
        {
            hiveObject = collision.gameObject;
            StartCoroutine(LightBlocked());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp-able"))
        {
            StopAllCoroutines();
            hiveObject = null;
        }
    }

    IEnumerator LightBlocked()
    {
        while (true)
        {
            if (hiveObject.transform.parent == null) 
            {
                Bee.GetComponent<BeeStates>().FSM(3);
                Destroy(gameObject);
                Destroy(hiveObject);
            }
            yield return null;
        }
    }
}
