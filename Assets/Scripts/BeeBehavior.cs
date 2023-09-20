/*****************************************************************************
// File Name :         BeeBehavior.cs
// Author :            Raymond Knapp
// Creation Date :     September 20th, 2023
//
// Behavior script for the enemy Bee GameObjects. This code essentially tells
// the bees to travel between the set positions until it finds the player. And once it finds 
// the player, it will catch the player. Otherwise, the bee will patrol between the set positions.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeBehavior : MonoBehaviour
{
    private Vector3 localScale;
    private float dirX;
    public float speed;
    private Rigidbody2D rb2d;
    private bool facingRight = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        localScale = transform.localScale;
        dirX = -1f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PatrolPoint")
        {
            dirX *= -1f;
            print("turn");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(dirX * speed, rb2d.velocity.y);
    }

    private void LateUpdate()
    {
        CheckDirection();
    }

    void CheckDirection()
    {
        if (dirX > 0)
        {
            facingRight = false;
        }
        else if (dirX < 0)
        {
            facingRight = true;
        }

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
        {
            localScale.x *= -1;
        }

        transform.localScale = localScale;
    }
}
