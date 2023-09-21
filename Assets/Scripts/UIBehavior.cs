/**********************************************************************************

// File Name :         GameManager.cs
// Author :            William Dietert
// Creation Date :     September 21, 2023
//
// Brief Description : The UI to see if Bee Vision is Off or On while playing the 
game.
**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIBehavior : MonoBehaviour
{
    public Text Beevision; 

    //making a variable for Game Manager ( this only recognizes names of scripts)
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        //finding the script Game Manager 
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //setting the UI to off be defualt
        Beevision.text = "Off"; 
    } 

    // Update is called once per frame
    void Update()
    {
        //checking if the head is on
      if(gm.BaseHead)
        {
            Beevision.text = "OFF";
        }
      else
        {
            Beevision.text = "ON";
        }
    }
}
