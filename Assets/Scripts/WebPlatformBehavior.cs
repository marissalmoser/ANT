using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebPlatformBehavior : MonoBehaviour
{
    public bool PlatformCanMove = true;
    [SerializeField] private float platformSpeed;
    [SerializeField] private Rigidbody2D Rb;
    public float Direction;
    [SerializeField] private GameObject Player;

    private GameManager gm;


    void Start()
    {
        StartCoroutine(PlatformMoving());
        Rb.freezeRotation = true;
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    IEnumerator PlatformMoving()
    {
        while(true)
        {
            if (PlatformCanMove)
            {
                //print(PlatformCanMove);
                Rb.velocity = new Vector2(platformSpeed * Direction, 0);
            }
            else
            {
                //print("else");
                Rb.velocity = Vector2.zero;
                Rb.constraints = RigidbodyConstraints2D.FreezePosition;
                Rb.freezeRotation = true;
                StopCoroutine(PlatformMoving());
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

    IEnumerator DestroyWebPlatform()
    {
        yield return new WaitForSeconds(5);
        gm.WebPlatformList.Remove(gameObject);
        Destroy(gameObject);
    }
}
