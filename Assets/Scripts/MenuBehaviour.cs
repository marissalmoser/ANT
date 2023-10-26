using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartGame();
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
    }
}
