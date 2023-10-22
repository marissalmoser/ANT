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
    private PlayerBehavior pb;
    public GameObject WalkGraphics;
    public GameObject CrawlGraphics;
    private Animator walkingAnim;
    private Animator crawlingAnim;

    //actions
    private InputAction move, jump, head, leg, crawl, changeMov, interact, spawnWeb, pause, nextLevel;
    public static Action BeeVision, WebShooterUI, ErrorMessage, PlatformCountUI;

    //moving variables
    [Header("Player Movement")]
    [SerializeField] private bool playerCanMove;        //starts and stops player movement regardless of system
    private bool playerCanCrawl;                        //on start and cancel
    public bool CrawlMapEnabled;
    [SerializeField] private float speed;
    private float direction;
    private Vector2 crawlDirection;
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
    private Vector3 mousePosition;
    private Vector2 mouseWorldPosition;

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
        //gm = GameManager.Instance;

        pb = gameObject.GetComponent<PlayerBehavior>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        walkingAnim = WalkGraphics.GetComponent<Animator>();
        crawlingAnim = CrawlGraphics.GetComponent<Animator>();

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
        nextLevel = MyPlayerInput.actions.FindActionMap("PartSwitching").FindAction("NextLevelKB");

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
        pause.started += GamePaused;
        nextLevel.started += SkipToNextLevel;

        WallBehavior.WallTriggered += SwitchToWalk;
    }

    private void SkipToNextLevel(InputAction.CallbackContext obj)
    {
        StartCoroutine(GameManager.Instance.NextLevel());
    }
    private void GamePaused(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }

    private void Handle_interactStarted(InputAction.CallbackContext obj)
    {
        Interact = true;

        pb.BreakObject();

        pb.PickUpObject();
    }
    private void Handle_interactCanceled(InputAction.CallbackContext obj)
    {
        Interact = false;
    }

    private void Handle_moveStarted(InputAction.CallbackContext obj)
    {
        playerCanMove = true;

        //walking animations
        

        if (canMove == 0)
        {
            //part not enabled
            ErrorMessage?.Invoke();
        }
        else
        {
            walkingAnim.SetBool("isWalking", true);
        }
    }
    private void Handle_moveCanceled(InputAction.CallbackContext obj)
    {
        playerCanMove = false;

        walkingAnim.SetBool("isWalking", false);
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

        if (canMove == 0)
        {
            //part not enabled
            ErrorMessage?.Invoke(); 
        }
        else
        {
            crawlingAnim.SetBool("isCrawling", true);
        }
    }
    private void Handle_crawlCanceled(InputAction.CallbackContext obj)
    {
        playerCanCrawl = false;

        crawlingAnim.SetBool("isCrawling", false);

        rb.velocity = Vector2.zero;
        crawlDirection = Vector2.zero;
    }

    private void SwitchMovementSystem(InputAction.CallbackContext obj)
    {
        //switch to crawling movement system
        if (!CrawlMapEnabled && canMove == 1) // && WallBehavior.OnClimbableWall) this broke it??
        {
            if (GameManager.Instance.BaseLeg)
            {
                SwitchToCrawl();
            }
            else
            {
                //trying to crawl with web shooter enabled
                ErrorMessage?.Invoke();
            }
        }

        //switch to 2D movement system
        else if (CrawlMapEnabled && canMove == 1)
        {
            SwitchToWalk();
        }

        else if (!GameManager.Instance.BaseHead)
        {
            //trying to move with bee vision
            ErrorMessage?.Invoke();
        }
        else
        {
            print("what?");
        }
    }

    public void SwitchToWalk()
    {
        //print("switch to 2D movement system");
        CrawlMapEnabled = false;
        rb.gravityScale = 4;
        transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").Enable();
        MyPlayerInput.actions.FindActionMap("PlayerCrawlingMovement").Disable();
        CrawlGraphics.SetActive(false);
        WalkGraphics.SetActive(true);
        if(pb.pickedUpObject != null)
        {
            pb.pickedUpObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
    public void SwitchToCrawl()
    {
        //print("switch to crawling movement system");
        CrawlMapEnabled = true;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        MyPlayerInput.actions.FindActionMap("PlayerTwoDirectionMovement").Disable();
        MyPlayerInput.actions.FindActionMap("PlayerCrawlingMovement").Enable();
        CrawlGraphics.SetActive(true);
        WalkGraphics.SetActive(false);
    }

    private void SwitchHeadPart(InputAction.CallbackContext obj)
    {
        GameManager.Instance.BaseHead = !GameManager.Instance.BaseHead;

        //Bee Vision turned on
        if(!GameManager.Instance.BaseHead)
        {
            //changes the sprite
            beeMaskCrawl.SetActive(true);
            beeMaskWalk.SetActive(true);

            //vision on
            BeeVision?.Invoke();
            
            //stop movement
            canMove = 0;

            //sound
            AudioManager.Instance.Play("BeeVisionOn");
        }

        //Bee vision turned off
        if (GameManager.Instance.BaseHead)
        {
            //changes the sprite
            beeMaskCrawl.SetActive(false);
            beeMaskWalk.SetActive(false);

            //vision off
            BeeVision?.Invoke();

            //resume movement
            canMove = 1;

            //sound
            AudioManager.Instance.Play("BeeVisionOff");
        }

        
    }
    private void SwitchLegPart(InputAction.CallbackContext obj)
    {

        //trying to turn web on with crawl
        if (GameManager.Instance.BaseLeg && CrawlMapEnabled)
        {
            //part not enabled
            ErrorMessage?.Invoke();
        }
        //enables web shooter
        else if (GameManager.Instance.BaseLeg && !playerCanCrawl)
        {
            //print("web on");
            webShooterWalk.SetActive(true);
            GameManager.Instance.BaseLeg = !GameManager.Instance.BaseLeg;
            WebShooterUI?.Invoke();
            AudioManager.Instance.Play("WebShooterOn");
        }
        //disables web shooter
        else
        {
            //print("web off");
            webShooterWalk.SetActive(false);
            GameManager.Instance.BaseLeg = !GameManager.Instance.BaseLeg;
            WebShooterUI?.Invoke();
            AudioManager.Instance.Play("WebShooterOff");
        }
    }

    private void SpawnWebStarted(InputAction.CallbackContext obj)
    {
        mouseWorldPosition = Vector2.zero;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        WebPlatformBehavior.MousePosition = mouseWorldPosition;

        pb.SpawnWebPlatform();
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
        else if(IsGrounded() && jumpStart && !CrawlMapEnabled && rb.velocity.y < 0.5f)
        {
            ErrorMessage?.Invoke(); 
        }
        
        //player crawl
        if(playerCanCrawl && CrawlMapEnabled) 
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
        pause.started -= GamePaused;
        WallBehavior.WallTriggered -= SwitchToWalk;
    }
}
