/**********************************************************************************

// File Name :         UserInterfaceBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 18, 2023
//
// Brief Description : 

**********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenBeeBehavior : MonoBehaviour
{
    public enum States { Patrol, Suspicious, Alert, Sleep}
    private Rigidbody2D rb;
    [SerializeField] GameObject LevelManager;
    private LevelManager lm;
    int lightBlocked = 0;

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
    private bool isFlipping;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        lm = LevelManager.GetComponent<LevelManager>();

        lm.BeeVisionObjects.Add(detectorOriginPt);

        step = speed * Time.deltaTime;

        FSM(States.Patrol);
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
                StartCoroutine(SuspiciousState());
                break;
            case States.Alert:                                              //Alert
                StopAllCoroutines();
                StartCoroutine(AlertState());
                break;
            case States.Sleep:                                              //Sleep
                StopAllCoroutines();
                StartCoroutine(SleepState());
                break;
        }
    }

    IEnumerator PatrolState()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, posA) < 0.5)
            {
                targetPos = posB;
                StartCoroutine(Flip());
                yield return new WaitForSeconds(1);

            }
            if (Vector2.Distance(transform.position, posB) < 0.5)
            {
                targetPos = posA;
                StartCoroutine(Flip());
                yield return new WaitForSeconds(1);
            }

            if (!GameManager.GameIsPaused)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            }

            yield return null;
        }
    }

    IEnumerator SuspiciousState()
    {
        //exclamation.SetActive(true);
        AudioManager.Instance.Play("BeeSeesPlayer");

        yield return new WaitForSeconds(1);

        //checks for player
        if (Player != null)
        {
            Player = null;
            //exclamation.SetActive(false);

            if (PerformDetection())
            {
                FSM(States.Alert);
            }
            else
            {
                //print("back to patrol");
                //targetPos = posA;
                FSM(States.Patrol);
            }
        }
    }

    IEnumerator AlertState()
    {
        //print("BEE MORE SNEAKY BZZZZZZ");
        AudioManager.Instance.Play("BeeAlert");
        StartCoroutine(GameManager.Instance.RestartLevel()); 
        yield return null;
    }

    IEnumerator SleepState()
    {
        //gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        rb.gravityScale = 2;

        //turn off bee vision
        lm.BeeVisionObjects.Remove(detectorOriginPt);
        Destroy(detectorOriginPt);

        print("YOU BEAT THE BEE!");
        yield return new WaitForSeconds(2);
        GameManager.Instance.GameWon();
    }

    public bool PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapBox(detectorOriginPt.transform.position, detectorSize, 0, playerLayer);
        if (collider != null)
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
        while (!isFlipping)
        {
            Collider2D collider = Physics2D.OverlapBox(detectorOriginPt.transform.position, detectorSize, 0, playerLayer);
            if (collider != null && !GameManager.GameIsPaused)
            {
                Player = collider.gameObject;
                //print("detected");
                FSM(States.Suspicious);  
            }
    
            yield return null;
        }
    }

    IEnumerator Flip()
    {
        //front animation true
        isFlipping = true;
        detectorOriginPt.SetActive(false);

        yield return new WaitForSeconds(1);

        //front animation false
        isFlipping = false;
        StartCoroutine(ConstantDetection());
        detectorOriginPt.SetActive(true);

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void LightShutOff()
    {
        lightBlocked++;
        //print(lightBlocked);
        if(lightBlocked >=3)
        {
            FSM(States.Sleep);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(detectorOriginPt.transform.position, detectorSize);
    }
}
