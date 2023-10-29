/**********************************************************************************

// File Name :         BeeStates.cs
// Author :            Marissa Moser
// Creation Date :     September 24, 2023
//
// Brief Description : 

**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeStates : MonoBehaviour
{
    public int State;
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] GameObject LevelManager;
    private LevelManager lm;
    public bool StartInToPatrol;
    [SerializeField] private GameObject hivePiece;
    [SerializeField] private GameObject lightObject;

    public enum States{Patrol, Suspicious, Alert, Sleep, ToPatrol}

    [Header("Bee")]
    private bool isFacingRight = true;
    [SerializeField] private bool canFlip = true;
    [SerializeField] private GameObject exclamation;

    [Header("Patrol")]
    [SerializeField] private Vector2 posA;
    [SerializeField] private Vector2 posB;
    private Vector2 targetPos;
    [SerializeField] private float speed;
    private float step;

    [Header("Detection")]
    [SerializeField] private Vector2 detectorSize;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject detectorOriginPt;
    private GameObject Player;

    [Header("Gizmos")]
    public Color gizmoIdle = Color.green;
    public Color gizmoDetected = Color.red;
    public bool ShowGizmos = true;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        lm = LevelManager.GetComponent<LevelManager>();
        anim = gameObject.GetComponent<Animator>();

        if(!lightObject.GetComponent<LightBehavior>().isFirstLight)
        {
            lm.BeeVisionObjects.Add(detectorOriginPt);
        }
        
        lm.Bees.Add(gameObject);

        step = speed * Time.deltaTime;


        if(!StartInToPatrol)
        {
            FSM(States.Patrol);
        }
        else
        {
            StopAnimations();
            anim.SetBool("Sleep", true);
        }
    }

    public void FSM(States state)
    {
        //State = newState;
        switch (state)
        {
            case States.Patrol:                                              //Patrol
                
                StopAllCoroutines();
                StartCoroutine(PatrolState());
                StartCoroutine(ConstantDetection());
                break;
            case States.Suspicious:                                         //Suspicious
                StopAllCoroutines();
                StartCoroutine(SusState());
                break;
            case States.Alert:                                              //Alert
                StopAllCoroutines();
                StartCoroutine(AlertState());
                break;
            case States.Sleep:                                              //Sleep
                StopAllCoroutines();
                StartCoroutine(SleepState());
                break;
            case States.ToPatrol:
                StopAllCoroutines();
                StartCoroutine(ToPatrol());
                break;
        }
    }

    IEnumerator PatrolState()
    {
        StopAnimations();
        anim.SetBool("Patrol", true);

        while (true)
        {
            if (Vector2.Distance(transform.position, posA) < 0.5)
            {
                targetPos = posB;
                Flip();

            }
            if (Vector2.Distance(transform.position, posB) < 0.5)
            {
                targetPos = posA;
                Flip();
            }
            
            if(!GameManager.GameIsPaused)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            }

            yield return null;
        }
    }

    IEnumerator SusState()
    {
        StopAnimations();
        anim.SetBool("Suspicious", true);

        AudioManager.Instance.Play("BeeSeesPlayer");
        
        //pauses movement
        targetPos = transform.position;
        Flip();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        exclamation.SetActive(true);
        targetPos = Player.transform.position;
        Flip();

        yield return new WaitForSeconds(1);

        //moves toward where it saw the player
        while(Vector2.Distance(transform.position, targetPos) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }

        //pauses
        targetPos = transform.position;
        Flip();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        yield return new WaitForSeconds(1);

        //checks for player
        if (Player != null)
        {
            Player = null;
            exclamation.SetActive(false);

            if (PerformDetection())
            {
                FSM(States.Alert);
            }
            else
            {
                //print("back to patrol");
                targetPos = posA;
                Flip();
                AudioManager.Instance.Play("BeeBuzzing");
                FSM(States.Patrol);
            }
        }
    }

    IEnumerator AlertState()
    {
        StopAnimations();
        anim.SetBool("Alert", true);

        print("BEE MORE SNEAKY BZZZZZZ");

        AudioManager.Instance.Play("BeeAlert");
        //all bees move toward player
        StartCoroutine(GameManager.Instance.RestartLevel());
        yield return null;
    }

    IEnumerator SleepState()
    {
        StopAnimations();
        anim.SetBool("Sleep", true);

        //stop all movement
        targetPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        rb.gravityScale = 2;
        AudioManager.Instance.Play("BeeSnoring");

        //turn off bee vision
        if (exclamation != null)
        {
            exclamation.SetActive(false);
        }

        lm.BeeVisionObjects.Remove(detectorOriginPt);
        Destroy(detectorOriginPt); //just turn off the sprite renderer is its off

        //bzzzz sound
        yield return null;
    }

    IEnumerator ToPatrol()
    {
        //hive falls
        hivePiece.GetComponent<Rigidbody2D>().gravityScale = 1;
        //light turns on ->                                                           change to be gradual when have art
        lightObject.GetComponent<SpriteRenderer>().enabled = true;
        lightObject.transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        StopAnimations();
        anim.SetBool("ToPatrol", true);

        //wing animation
        while (StartInToPatrol)
        {
            targetPos = posA;
            Flip();

            if (!GameManager.GameIsPaused)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            }

            if(Vector2.Distance(transform.position, posA) < 0.5)
            {
                //hivePiece.GetComponent<Rigidbody2D>().gravityScale = 0;                   ??
                FSM(States.Patrol);
                gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            }

            yield return null;
        }
    }


    public bool PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox(detectorOriginPt.transform.position, detectorSize, 0, playerLayer);
        if(collider != null)
        {
            Player = collider.gameObject;
            //print("player detected");
            return (true);
        }
        else
        {
            return (false);
        }
    }

    IEnumerator ConstantDetection()
    {
        while(true)
        {
            Collider2D collider = Physics2D.OverlapBox(detectorOriginPt.transform.position, detectorSize, 0, playerLayer);
            if (collider != null && !GameManager.GameIsPaused)
            {
                Player = collider.gameObject;
                FSM(States.Suspicious);                         //goes into suspicious state
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnDrawGizmos()
    {
        if(ShowGizmos && detectorOriginPt != null)
        {
            Gizmos.color = gizmoIdle;
            if (Player != null)
                Gizmos.color = gizmoDetected;
            Gizmos.DrawCube(detectorOriginPt.transform.position, detectorSize);
        }
    }

    private void Flip()
    {
        if(canFlip && isFacingRight && targetPos.x < transform.position.x || !isFacingRight && targetPos.x > transform.position.x)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void StopAnimations()
    {
        anim.SetBool("ToPatrol", false);
        anim.SetBool("Patrol", false);
        anim.SetBool("Suspicious", false);
        anim.SetBool("Alert", false);
        anim.SetBool("Sleep", false);
    }
}
