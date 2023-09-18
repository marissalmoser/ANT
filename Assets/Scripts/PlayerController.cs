using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //components
    public PlayerInput MyPlayerInput;
    public Rigidbody2D Rb;

    //actions
    private InputAction move;
    private InputAction jump;

    //moving variables
    private bool playerCanMove;
    [SerializeField] private float speed;
    private float direction;
    private bool isFacingRight = true;

    //jumping variables
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool jumpStart;

    void Start()
    {
        MyPlayerInput.currentActionMap.Enable();

        move = MyPlayerInput.currentActionMap.FindAction("Move");
        jump = MyPlayerInput.currentActionMap.FindAction("Jump");

        move.started += Handle_moveStarted;
        move.canceled += Handle_moveCanceled;
        jump.started += Handle_jumpStarted;
        jump.canceled += Handle_jumpCanceled;
    }

    private void Handle_moveStarted(InputAction.CallbackContext obj)
    {
        playerCanMove = true;
    }

    private void Handle_moveCanceled(InputAction.CallbackContext obj)
    {
        playerCanMove = false;
    }

    private void Handle_jumpStarted(InputAction.CallbackContext obj)
    {
        if (jumpStart == false)
        {
            jumpStart = true;
        }
    }

    private void Handle_jumpCanceled(InputAction.CallbackContext obj)
    {
        jumpStart = false;
    }

    private void Update()
    {
        Flip();
        if (playerCanMove == true)
        {
            direction = move.ReadValue<float>();
        }
    }

    private void FixedUpdate()
    {
        //player 2D movement
        if(playerCanMove == true)
        {
            Rb.velocity = new Vector2(speed * direction, Rb.velocity.y);
        }
        else
        {
            Rb.velocity = new Vector2(0, Rb.velocity.y);
        }

        //player jump
        if (IsGrounded() && jumpStart)
        {
            print("jump");
            Rb.velocity = new Vector2(Rb.velocity.x, jumpingPower);
        }
        /*
        
        else if(jumpStart && Rb.velocity.y > 0)
        {
            //Rb.velocity = new Vector2(Rb.velocity.x, Rb.velocity.x * 0.5f);
            print("boost"); 
        }*/

    }

    private void Flip()
    {
        if (!isFacingRight && direction < 0f || isFacingRight && direction > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
