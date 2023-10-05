/**********************************************************************************

// File Name :         PlayerController.cs
// Author :            Marissa Moser
// Creation Date :     September 13, 2023
//
// Brief Description : 

**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //components and GOs
    public PlayerInput MyPlayerInput;
    private Rigidbody2D rb;
    //public GameManager GameManager;
    private GameManager gm;
    private PlayerBehavior pb;
    public GameObject WalkGraphics;
    public GameObject CrawlGraphics;

    //actions
    private InputAction move, jump, head, leg, crawl, changeMov, interact, spawnWeb, pause;

    //moving variables
    [Header("Player Movement")]
    [SerializeField] private bool playerCanMove;        //starts and stops player movement regardless of system
    private bool playerCanCrawl;                        //on start and cancel
    public bool CrawlMapEnabled;
    [SerializeField] private float speed;
    private float direction;
    //private float rotationSpeed = 3;
    private Vector2 crawlDirection;
    //private Vector2 crawlRotation;
    private bool isFacingRight = true;
    [SerializeField] private LayerMask climbableWalls;
    private float angle;
    private int canMove = 1;

    //jumping variables
    [Header("Jumping")]
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool jumpStart;

    //interacting
    [HideInInspector] public bool Interact;

    [Header("Bug Parts")]
    [SerializeField] private GameObject beeMaskWalk;
    [SerializeField] private GameObject beeMaskCrawl;
    [SerializeField] private GameObject webShooterWalk;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    private Vector3 camOffset = new Vector3(0, 1, -10);
    private Vector3 camPos;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        pb = gameObject.GetComponent<PlayerBehavior>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").Enable();
        MyPlayerInput.actions.FindActionMap("PartSwitching").Enable();

        move = MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").FindAction("Move");
        jump = MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").FindAction("Jump");
        crawl = MyPlayerInput.actions.FindActionMap("PlayerCrawlingMovement").FindAction("Crawl");
        head = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("Head");
        leg = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("Leg");
        changeMov = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("SwitchMovementSystem");
        interact = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("Interact");
        spawnWeb = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("SpawnWebPlatform");
        pause = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("Pause");

        move.started += Handle_moveStarted;
        move.canceled += Handle_moveCanceled;
        jump.started += Handle_jumpStarted;
        jump.canceled += Handle_jumpCanceled;
        head.started += SwitchHeadPart;
        leg.started += SwitchLegPart;
        crawl.started += Handle_crawlStarted;
        crawl.canceled += Handle_crawlCanceled;
        changeMov.started += SwitchMovementSystem;
        interact.started += Handle_interactStarted;
        interact.canceled += Handle_interactCanceled;
        spawnWeb.started += SpawnWebStarted;
        spawnWeb.canceled += SpawnWebCanceled;
        pause.started += GamePaused;
    }

    private void GamePaused(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }

    private void Handle_interactStarted(InputAction.CallbackContext obj)
    {
        Interact = true;

        pb.PickUpObject();
    }
    private void Handle_interactCanceled(InputAction.CallbackContext obj)
    {
        Interact = false;
    }

    private void Handle_moveStarted(InputAction.CallbackContext obj)
    {
        playerCanMove = true;
        if (canMove == 0)
        {
            //part not enabled
        }
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

    private void Handle_crawlStarted(InputAction.CallbackContext obj)
    {
        playerCanCrawl = true;
        if(canMove == 0)
        {
            //part not enabled
        }
    }
    private void Handle_crawlCanceled(InputAction.CallbackContext obj)
    {
        playerCanCrawl = false;
        rb.velocity = Vector2.zero;
        crawlDirection = Vector2.zero;
    }

    private void SwitchMovementSystem(InputAction.CallbackContext obj)
    {
        //switch to crawling movement system
        if (!CrawlMapEnabled && gm.BaseLeg && canMove == 1)
        {
            //print("switch to crawling movement system");
            CrawlMapEnabled = true;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            //spotToCarry.transform.position = crawlCarryOffset + transform.position;
            MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").Disable();
            MyPlayerInput.actions.FindActionMap("PlayerCrawlingMovement").Enable();
            CrawlGraphics.SetActive(true);
            WalkGraphics.SetActive(false);
        }
        //switch to 2D movement system
        else if (canMove == 1)
        {
            //print("switch to 2D movement system");
            CrawlMapEnabled = false;
            rb.gravityScale = 4;
            //spotToCarry.transform.position = walkCarryOffset + transform.position;
            transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").Enable();
            MyPlayerInput.actions.FindActionMap("PlayerCrawlingMovement").Disable();
            CrawlGraphics.SetActive(false);
            WalkGraphics.SetActive(true);
        }
        else
        {
            //part not enabled
        }
    }

    private void SwitchHeadPart(InputAction.CallbackContext obj)
    {
        gm.BaseHead = !gm.BaseHead;

        //Bee Vision turned on
        if(!gm.BaseHead)
        {
            //changes the sprite
            beeMaskCrawl.SetActive(true);
            beeMaskWalk.SetActive(true);

            //vision on
            foreach (var vision in gm.BeeVisionObjects)
            {
                vision.GetComponent<SpriteRenderer>().enabled = true;
            }

            //stop movement
            canMove = 0;
        }

        //Bee vision turned off
        if (gm.BaseHead)
        {
            //changes the sprite
            beeMaskCrawl.SetActive(false);
            beeMaskWalk.SetActive(false);

            //vision off
            foreach (var vision in gm.BeeVisionObjects)
            {
                vision.GetComponent<SpriteRenderer>().enabled = false;
            }

            //resume movement
            canMove = 1;
        }

        
    }
    private void SwitchLegPart(InputAction.CallbackContext obj)
    {
        //enables web shooter
        if (gm.BaseLeg && !playerCanCrawl)
        {
            print("web on");
            webShooterWalk.SetActive(true);
            gm.BaseLeg = !gm.BaseLeg;
        }
        //disables web shooter
        else
        {
            print("web off");
            webShooterWalk.SetActive(false);
            gm.BaseLeg = !gm.BaseLeg;
        }
    }

    private void SpawnWebStarted(InputAction.CallbackContext obj)
    {
        pb.SpawnWebPlatform();
        if(pb.WebPlatform != null)
        {
            pb.WebPlatform.GetComponent<WebPlatformBehavior>().Direction = direction;
        }
    }

    private void SpawnWebCanceled(InputAction.CallbackContext obj)
    {
        //print(pb.WebPlatform);
        if(pb.WebPlatform != null)
        {
            pb.WebPlatform.GetComponent<WebPlatformBehavior>().PlatformCanMove = false;
            pb.WebPlatform = null;
        }
    }

    private void Update()
    {
        Flip();
        if (playerCanMove == true)
        {
            direction = move.ReadValue<float>();
        }

        if(playerCanCrawl == true)
        {
            crawlDirection = crawl.ReadValue<Vector2>();
        }

        //rotation of the player during movement. Only if in crawl map and bee vision off
        if (crawlDirection != Vector2.zero && CrawlMapEnabled && canMove == 1)
        {
            angle = Mathf.Atan2(crawlDirection.y, crawlDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        }
    }

    private void FixedUpdate()
    {
        //player 2D movement
        if(playerCanMove == true)
        {
            rb.velocity = new Vector2(speed * direction * canMove, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        //player jump
        if (IsGrounded() && jumpStart && !CrawlMapEnabled && rb.velocity.y < 0.5f && canMove == 1)
        {
            //print("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        //else "part not enabled"
        
        //player crawl
        if(playerCanCrawl && CrawlMapEnabled)          // && CanClimb()    ?
        {
            rb.velocity = new Vector2(crawlDirection.x, crawlDirection.y) * speed * canMove;
            //print(CanClimb());
        }

        //camera movement
        camPos = transform.position + camOffset;
        cam.transform.position = camPos;

    }

    private void Flip()
    {
        if (isFacingRight && direction < 0f || !isFacingRight && direction > 0 && !CrawlMapEnabled)
        {
            if(canMove == 1)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool CanClimbWall()
    {
        return Physics2D.OverlapCircle(transform.position, 0.2f, ~climbableWalls);
    }

    public void OnDestroy()
    {
        move.started -= Handle_moveStarted;
        move.canceled -= Handle_moveCanceled;
        jump.started -= Handle_jumpStarted;
        jump.canceled -= Handle_jumpCanceled;
        head.started -= SwitchHeadPart;
        leg.started -= SwitchLegPart;
        crawl.started -= Handle_crawlStarted;
        crawl.canceled -= Handle_crawlCanceled;
        changeMov.started -= SwitchMovementSystem;
        interact.started -= Handle_interactStarted;
        interact.canceled -= Handle_interactCanceled;
        spawnWeb.started -= SpawnWebStarted;
        spawnWeb.canceled -= SpawnWebCanceled;
        pause.started -= GamePaused;
    }
}
