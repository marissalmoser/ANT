/**********************************************************************************

// File Name :         WebPlatformBehavior.cs
// Author :            Marissa Moser
// Creation Date :     September 21, 2023
//
// Brief Description : This script manages the behavior of the web platforms. It
contains a coroutine to move the platform to its target location, wait for a
period of time, and then destory itself after that time or once the player 
collides with it.

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebPlatformBehavior : MonoBehaviour
{
    public bool PlatformCanMove = true;
    [SerializeField] private float platformSpeed;
    [SerializeField] private Rigidbody2D Rb;
    public static Vector3 MousePosition;
    private Coroutine currrentCoroutine;
    private Animator anim;

    void Start()
    {
        currrentCoroutine = StartCoroutine(PlatformMoving());

        anim = gameObject.GetComponent<Animator>();

        AudioManager.Instance.Play("ShootPlatform");
    }

    IEnumerator PlatformMoving()
    {
        Vector3 targetPos = MousePosition;

        for(int i = 0; i < 75; i++)
        {
            print("in loop");
            transform.position = Vector3.MoveTowards(transform.position, targetPos, platformSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, MousePosition) < 0.4f)
            {
                break;
            }
            yield return null;
        }

        print("inplace");
        PlatformCanMove = false;

        Rb.velocity = Vector2.zero;
        Rb.constraints = RigidbodyConstraints2D.FreezePosition;
        Rb.freezeRotation = true;
        GetComponent<BoxCollider2D>().usedByEffector = true;
        GetComponent<BoxCollider2D>().enabled = true;

        currrentCoroutine = StartCoroutine(PlatformBehavior());
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
        yield return new WaitForSeconds(7);
        currrentCoroutine = StartCoroutine(DestroyWebPlatform());
    }

    IEnumerator DestroyWebPlatform()
    {
        if(currrentCoroutine != null)
        {
            StopCoroutine(currrentCoroutine);
        }
        anim.SetBool("WebBreaking", true);

        yield return new WaitForSeconds(3);

        GameManager.Instance.WebPlatformList.Remove(gameObject);
        PlayerController.PlatformCountUI?.Invoke();
        anim.SetBool("WebFalling", true);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        AudioManager.Instance.Play("PlatformBreak");

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
