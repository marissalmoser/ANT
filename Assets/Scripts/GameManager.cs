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

        yield return new WaitForSeconds(1);
        WebPlatformList.Clear();

        BaseLeg = true;
        BaseHead = true;

        GameLost();

    }
    public IEnumerator NextLevel()
    {
        UserInterfaceBehvaior.FadeToBlack?.Invoke();

        yield return new WaitForSeconds(1);

        BaseLeg = true;
        BaseHead = true;
        WebPlatformList.Clear();

        StartCoroutine(BackgroundMusicManager.Instance.FadeMusic(false));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        StartCoroutine(BackgroundMusicManager.Instance.FadeMusic(true));
        SceneManager.LoadScene(7);
    }
    ///LoadsScene1
    public void StartGame()
    {
        BaseLeg = true;
        BaseHead = true;
        WebPlatformList.Clear();

        CurrentLevel = 1;
        StartCoroutine(BackgroundMusicManager.Instance.FadeMusic(false));
        SceneManager.LoadScene(1);
    }
    ///loads static variable "current level", called from pause menu reset level and loss screen reset level
    public void RestartCurrentLevel()
    {
        //print(CurrentLevel);
        GameManager.Instance.WebPlatformList.Clear();
        BaseLeg = true;
        BaseHead = true;
        Time.timeScale = 1;

        StartCoroutine(BackgroundMusicManager.Instance.FadeMusic(true)); //cut
        SceneManager.LoadScene(CurrentLevel);
    }
    public void RestartCurrentLevelFromPause()
    {
        //print(CurrentLevel);
        GameManager.Instance.WebPlatformList.Clear();
        BaseLeg = true;
        BaseHead = true;
        Time.timeScale = 1;

        SceneManager.LoadScene(CurrentLevel);
    }
    ///Loads Title scene
    public void ReturnToTitle()
    {
        GameManager.Instance.WebPlatformList.Clear();
        StartCoroutine(BackgroundMusicManager.Instance.FadeMusic(true));
        SceneManager.LoadScene(0);
    }
    ///Quits Application
    public void QuitGame()
    {
        Application.Quit();
    }
}
