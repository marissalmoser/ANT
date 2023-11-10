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

        while(Vector2.Distance(transform.position, MousePosition) > 0.3f)
        {
            if (PlatformCanMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, platformSpeed * Time.deltaTime);
            }
            yield return null;
        }

        Rb.velocity = Vector2.zero;
        Rb.constraints = RigidbodyConstraints2D.FreezePosition;
        Rb.freezeRotation = true;
        GetComponent<BoxCollider2D>().usedByEffector = true;

        StopCoroutine(currrentCoroutine);
        currrentCoroutine = StartCoroutine(PlatformBehavior());
        PlatformCanMove = false;
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
        StopCoroutine(currrentCoroutine);
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
