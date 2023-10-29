/**********************************************************************************

// File Name :         UserInterfaceBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 22, 2023
//
// Brief Description : 

**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartGame();
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToTitle()
    {
        GameManager.Instance.ReturnToTitle();
    }

    public void RetryLevel()
    {
        GameManager.Instance.RestartCurrentLevel();
        Time.timeScale = 1;
    }
}
