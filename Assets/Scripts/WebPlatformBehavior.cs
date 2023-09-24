using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebPlatformBehavior : MonoBehaviour
{
    public bool PlatformCanMove = true;
    [SerializeField] private float platformSpeed = 15f;
    [SerializeField] private Rigidbody2D Rb;

    void Update()
    {
        
        if(PlatformCanMove)
        {
            print(PlatformCanMove);
            Rb.velocity = new Vector2(platformSpeed, Rb.velocity.y);
        }
        else
        {
            Rb.velocity = Vector2.zero;

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlatformCanMove)
        {
            StartCoroutine(DestroyWebPlatform());
        }
    }

    IEnumerator DestroyWebPlatform()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
