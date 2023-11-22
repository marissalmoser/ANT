/**********************************************************************************

// File Name :         BeeStates.cs
// Author :            Marissa Moser
// Creation Date :     September 24, 2023
//
// Brief Description : This script contains the finite state machine for the bee
enemies. The states include ToPatrol, Partol, Suspicious, Alert, and Sleep. 
These states manage the bee's movement, detection and targeting of the player, 
as well as entering sleep state when in darkness.

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
    [SerializeField] private GameObject wings;
    [SerializeField] private GameObject Zzz;

    public enum States{Patrol, Suspicious, Alert, Sleep, ToPatrol}
    private Coroutine currentState;

    [Header("Bee")]
    [SerializeField]private bool isFacingRight = true;
    [SerializeField] private bool startFacingRight = true;
    [SerializeField] private GameObject exclamation;

    [Header("Patrol")]
    [SerializeField] private Vector2 posA;
    [SerializeField] private Vector2 posB;
    private Vector2 targetPos;
    [SerializeField] private float speed;

    [Header("Detection")]
    [SerializeField] private Vector2 detectorSize;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject detectorOriginPt;
    private GameObject Player;
    private Coroutine detectionCache;

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
        switch (state)
        {
            case States.Patrol:                                              //Patrol
                StopAllCoroutines();
                currentState = StartCoroutine(PatrolState());
                detectionCache = StartCoroutine(ConstantDetection());
                break;
            case States.Suspicious:                                         //Suspicious
                if(currentState != null)
                {
                    StopCoroutine(currentState);
                }
                currentState = StartCoroutine(SusState());
                break;
            case States.Alert:                                              //Alert
                currentState = StartCoroutine(AlertState());
                break;
            case States.Sleep:                                              //Sleep
                StopAllCoroutines();
                currentState = StartCoroutine(SleepState());
                break;
            case States.ToPatrol:
                currentState = StartCoroutine(ToPatrol()); 
                break;
        }
    }

    IEnumerator PatrolState()
    {
        StopAnimations();
        anim.SetBool("Patrol", true);
        wings.GetComponent<Animator>().SetBool("WingFlap", true);

        FlipCheck();
        LevelManager.GetComponent<LevelManager>().Escaped();

        while (true)
        {
            if (Vector2.Distance(transform.position, posB) > 0.5)
            {
                targetPos = posB;
                //Flip();

                while (Vector2.Distance(transform.position, posB) > 0.5)
                {
                    if (!GameManager.GameIsPaused)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                    }
                    yield return null;
                }

            }
            if (Vector2.Distance(transform.position, posB) < 0.5)
            {
                targetPos = posA;
                //Flip();

                while (Vector2.Distance(transform.position, posA) > 0.5)
                {
                    if (!GameManager.GameIsPaused)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                    }
                    yield return null;
                }
            }

            yield return null;
        }
    }

    IEnumerator SusState()
    {
        StopAnimations();
        anim.SetBool("Suspicious", true);

        AudioManager.Instance.Play("BeeSeesPlayer");
        
        ////pauses movement
        targetPos = transform.position;     //store old target pos??
        //Flip();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        exclamation.SetActive(true);
        //targetPos = Player.transform.position;
        Flip();

        yield return new WaitForSeconds(0.5f);

        ////moves toward where it saw the player
        //while(Vector2.Distance(transform.position, targetPos) > 0.5f)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //    yield return null;
        //}

        ////pauses
        //targetPos = transform.position;
        //Flip();
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //yield return new WaitForSeconds(1);
        //checks for player
        if (Player != null)
        {
            Player = null;
            exclamation.SetActive(false);

            if (PerformDetection() && !lightObject.GetComponent<LightBehavior>().isFirstLight)
            {
                FSM(States.Alert);
            }
            else
            {
                if (detectionCache == null)
                {
                    detectionCache = StartCoroutine(ConstantDetection());
                }

                while (Vector2.Distance(transform.position, targetPos) > 0.5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                    yield return null;
                }

                FSM(States.Patrol);
            }
        }
    }

    IEnumerator AlertState()
    {
        StopAnimations();
        anim.SetBool("Alert", true);
        UserInterfaceBehvaior.FadeToBlack?.Invoke();

        LevelManager.GetComponent<LevelManager>().GotCaught();

        AudioManager.Instance.Play("BeeAlert");
        StartCoroutine(GameManager.Instance.RestartLevel());
        yield return null;
    }

    IEnumerator SleepState()
    {
        LevelManager.GetComponent<LevelManager>().Escaped();
        StopAnimations();
        anim.SetBool("Sleep", true);
        wings.GetComponent<Animator>().SetBool("WingFlap", false);
        Zzz.SetActive(true);

        //stop all movement
        targetPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        rb.gravityScale = 2;
        AudioManager.Instance.Play("BeeSnoring");

        //turn off bee vision
        if (exclamation != null)
        {
            exclamation.SetActive(false);
        }

        lm.BeeVisionObjects.Remove(detectorOriginPt);
        Destroy(detectorOriginPt);

        yield return null;
    }

    IEnumerator ToPatrol()
    {
        //hive falls
        hivePiece.GetComponent<Rigidbody2D>().gravityScale = 1;
        //light turns on                                                           change to be gradual when have art
        lightObject.GetComponent<SpriteRenderer>().enabled = true;
        lightObject.transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        StopAnimations();
        anim.SetBool("ToPatrol", true);
        AudioManager.Instance.Play("BeeWakesUp");
        if (detectionCache == null)
        {
            detectionCache = StartCoroutine(ConstantDetection());
        }
        wings.GetComponent<Animator>().SetBool("WingFlap", true);
        while (Vector2.Distance(transform.position, posA) > 0.5)
        {
            targetPos = posA;
            Flip();

            if (!GameManager.GameIsPaused && Player == null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }
            else if (currentState != null)
            {
                StopCoroutine(currentState);
                break;
            }

            yield return null;
        }
        if (currentState != null)
        {
            FSM(States.Patrol);
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }


    public bool PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox(detectorOriginPt.transform.position, detectorSize, 0, playerLayer);
        if(collider != null)
        {
            Player = collider.gameObject;
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

                FSM(States.Suspicious);
                if (detectionCache != null)
                {
                    StopCoroutine(detectionCache);
                }
                detectionCache = null;
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
        if(isFacingRight && targetPos.x < transform.position.x)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        else if(!isFacingRight && targetPos.x > transform.position.x)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    void FlipCheck()
    {
        if(isFacingRight && !startFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        if(!isFacingRight && startFacingRight)
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
