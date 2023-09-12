using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public bool BaseWalking;
    public bool BaseHead;           //
    public bool BaseMid;            //
    public bool BaseLeg;            // true for climbing, false for jumping

    private float horizontal;
    private float speed = 8f;
    private bool isFacingRight = true;
    //private float jumpingPower = 16f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();

        BaseWalking = true;
        BaseHead = true;
        BaseMid = true;
        BaseLeg = true;
    }

    void Update()
    {
         horizontal = Input.GetAxisRaw("Horizontal");
         Flip();
    }

    private void FixedUpdate()
    {
        if(BaseWalking)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private void Flip()
    {
        if(!isFacingRight && horizontal < 0f || isFacingRight && horizontal > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
}
