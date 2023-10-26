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
        print("Level Restarting");
        //stop player movement
        //forloop all bee scream animation from list of bees, set move towards pos to player
        //clear bee list

        yield return new WaitForSeconds(2);

        BaseLeg = true;
        BaseHead = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1);

        BaseLeg = true;
        BaseHead = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public IEnumerator GameWon()
    {
        SceneManager.LoadScene(0);
        yield return null;
    }
}
