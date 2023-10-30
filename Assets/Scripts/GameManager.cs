/**********************************************************************************

// File Name :         GameManager.cs
// Author :            Marissa Moser
// Creation Date :     September 18, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool BaseHead;
    public bool BaseLeg;

    public List<GameObject> WebPlatformList = new List<GameObject>();

    public static int CurrentLevel;
    public static bool GameIsPaused;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;

        BaseHead = true;
        BaseLeg = true;
    }

    public IEnumerator RestartLevel()
    {
        //print("Level Restarting");

        //stop player movement
        //forloop all bee scream animation from list of bees, set move towards pos to player?

        yield return new WaitForSeconds(2);

        BaseLeg = true;
        BaseHead = true;

        GameLost();

    }
    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1);

        BaseLeg = true;
        BaseHead = true;

        //fade to black

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //fade from black
    }
    ///called from lv 5
    public void GameWon()
    {
        SceneManager.LoadScene(6);
    }
    ///loads lose screen
    public void GameLost()
    {
        CurrentLevel = SceneManager.GetActiveScene().buildIndex;
        //print(CurrentLevel);
        SceneManager.LoadScene(7);
    }
    ///LoadsScene1
    public void StartGame()
    {
        CurrentLevel = 1;
        SceneManager.LoadScene(1);
    }
    ///loads static variable "current level"
    public void RestartCurrentLevel()
    {
        //print(CurrentLevel);
        SceneManager.LoadScene(CurrentLevel);
    }
    ///Loads Title scene
    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }
    ///Quits Application
    public void QuitGame()
    {
        Application.Quit();
    }
}
