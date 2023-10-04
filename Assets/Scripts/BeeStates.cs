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
    private GameManager gm;

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

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        gm.BeeVisionObjects.Add(detectorOriginPt);
        //print(gm.BeeVisionObjects.Count);

        step = speed * Time.deltaTime;

        FSM(0);
    }

    /// <summary>
    /// Call this function with the State's parameter to change states :)
    /// </summary>
    public void FSM(int newState)
    {
        State = newState;
        switch (State)
        {
            case (0):                                              //Patrol
                
                StopAllCoroutines();
                StartCoroutine(PatrolState());
                StartCoroutine(ConstantDetection());
                //print("start detecting");
                break;
            case (1):                                              //Suspicious
                StopAllCoroutines();
                StartCoroutine(SusState());
                //print("stop detecting");
                break;
            case (2):                                              //Alert
                StopAllCoroutines();
                StartCoroutine(AlertState());
                break;
            case (3):                                              //Sleep
                StopAllCoroutines();
                StartCoroutine(SleepState());
                break;
        }
    }

    IEnumerator PatrolState()
    {
        while(true)
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
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            yield return null;
        }
    }

    IEnumerator SusState()
    {
        //print("sus");
        //pauses movement
        targetPos = transform.position;
        Flip();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        exclamation.SetActive(true);
        targetPos = Player.transform.position;
        Flip();

        yield return new WaitForSeconds(2);

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
        yield return new WaitForSeconds(2);

        //checks for player
        if (Player != null)
        {
            Player = null;
            exclamation.SetActive(false);

            if (PerformDetection())
            {
                FSM(2);
            }
            else
            {
                //print("back to patrol");
                targetPos = posA;
                Flip();
                FSM(0);
            }
        }
    }

    IEnumerator AlertState()
    {
        print("BEE MORE SNEAKY BZZZZZZ");
        StartCoroutine(gm.EndLevel());
        yield return null;
    }

    IEnumerator SleepState()
    {
        //stop all movement
        targetPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        rb.gravityScale = 2;
        //turn off bee vision
        if(exclamation != null)
        {
            exclamation.SetActive(false);
        }
        
        gm.BeeVisionObjects.Remove(detectorOriginPt);
        Destroy(detectorOriginPt);

        //bzzzz sound
        yield return null;
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
            if (collider != null)
            {
                Player = collider.gameObject;
                FSM(1);                         //goes into suspicious state
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
}
