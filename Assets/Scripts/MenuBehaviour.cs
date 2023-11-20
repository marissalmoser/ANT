/**********************************************************************************

// File Name :         UserInterfaceBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 22, 2023
//
// Brief Description : This script contains the behaviors for the buttons on the
menu screens. 

**********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject FadeImage;

    public void StartGame()
    {
        AudioManager.Instance.Play("ButtonClicks");
        StartCoroutine(StartingGame());
    }

    public void QuitGame()
    {
        AudioManager.Instance.Play("ButtonClicks");
        Application.Quit();
    }

    public void ReturnToTitle()
    {
        AudioManager.Instance.Play("ButtonClicks");
        GameManager.Instance.ReturnToTitle();
    }

    public void RetryLevel()
    {
        AudioManager.Instance.Play("ButtonClicks");
        Time.timeScale = 1;
        GameManager.Instance.RestartCurrentLevel();
    }

    public void ClickSound()
    {
        AudioManager.Instance.Play("ButtonClicks");
    }

    IEnumerator StartingGame()
    {
        Time.timeScale = 1;

        FadeImage.SetActive(true);
        FadeImage.GetComponent<Animator>().SetBool("FadeToBlack", true);

        yield return new WaitForSeconds(1);

        GameManager.Instance.StartGame();
    }
}
