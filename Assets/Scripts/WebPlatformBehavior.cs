/**********************************************************************************

// File Name :         WebPlatformBehavior.cs
// Author :            Marissa Moser
// Creation Date :     September 21, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebPlatformBehavior : MonoBehaviour
{
    public bool PlatformCanMove = true;
    [SerializeField] private float platformSpeed;
    [SerializeField] private Rigidbody2D Rb;
    private float step;
    public static Vector3 MousePosition;
    private Coroutine currrentCoroutine;

    void Start()
    {
        currrentCoroutine = StartCoroutine(PlatformMoving());
        Rb.freezeRotation = true;

        step = platformSpeed * Time.deltaTime;
    }

    IEnumerator PlatformMoving()
    {
        while(true)
        {
            if (PlatformCanMove)
            {
                if(Vector2.Distance(transform.position, MousePosition) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, MousePosition, step);
                    //transform.Translate(targetPos * step);
                    //Rb.AddForce(targetPos);
                }
                else
                {
                    PlatformCanMove = false;
                }
            }
            else
            {
                //print("else");
                Rb.velocity = Vector2.zero;
                Rb.constraints = RigidbodyConstraints2D.FreezePosition;
                Rb.freezeRotation = true;
                StopCoroutine(currrentCoroutine);
                currrentCoroutine = StartCoroutine(PlatformBehavior());
            }

            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlatformCanMove)
        {
            StartCoroutine(DestroyWebPlatform());
        }
    }

    IEnumerator PlatformBehavior()
    {
        //print("wait 5");
        yield return new WaitForSeconds(5);
        currrentCoroutine = StartCoroutine(DestroyWebPlatform());
    }

    IEnumerator DestroyWebPlatform()
    {
        //print("destroy in 5");
        StopCoroutine(currrentCoroutine);
        //play destroy animantion;
        yield return new WaitForSeconds(5);
        GameManager.Instance.WebPlatformList.Remove(gameObject);
        PlayerController.PlatformCountUI?.Invoke();
        Destroy(gameObject);
    }
}
