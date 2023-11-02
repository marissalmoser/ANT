/**********************************************************************************

// File Name :         UserInterfaceBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 5, 2023
//
// Brief Description : 

**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInterfaceBehvaior : MonoBehaviour
{
    [SerializeField] private TMP_Text beeVisionText;
    [SerializeField] private TMP_Text webPlatformText;
    [SerializeField] private GameObject WebPlatformUI;
    [SerializeField] private GameObject errorMessageText;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject WebPlatform1UI;
    [SerializeField] private GameObject WebPlatform2UI;
    [SerializeField] private GameObject WebPlatform3UI;

    public static Action FadeToBlack;
    [SerializeField] private GameObject blackScreen;

    private Coroutine errorCoroutineCache;

    void Awake()
    {
        PlayerController.BeeVision += SwitchHeadUI;
        PlayerController.WebShooterUI += SwitchLegUI;
        PlayerController.ErrorMessage += StartErrorCoroutine;
        PlayerController.PlatformCountUI += ChangePlatformCountUI;
        PlayerController.GamePaused += Pause;
        FadeToBlack += Fade;
    }

    private void StartErrorCoroutine()
    {
        if(errorCoroutineCache == null)
        {
            errorCoroutineCache = StartCoroutine(ErrorMesageUI());
        }
        else
        {
            StopCoroutine(errorCoroutineCache);
            errorCoroutineCache = StartCoroutine(ErrorMesageUI());
        }
    }

    public void SwitchHeadUI()
    {
        // on UI
        if(!GameManager.Instance.BaseHead)
        {
            beeVisionText.text = "Bee Vision: On";
        }
        // off UI
        else
        {
            beeVisionText.text = "Bee Vision Off";
        }
    }

    public void SwitchLegUI()
    {
        //print("web ui");
        if (!GameManager.Instance.BaseLeg)
        {
            WebPlatformUI.SetActive(true);
            //webPlatformText.text = "Web Platforms: On";
        }
        else
        {
            WebPlatformUI.SetActive(false);
            //webPlatformText.text = "Web Platforms: Off";
        }
    }

    public void ChangePlatformCountUI()
    {
        if(GameManager.Instance.WebPlatformList.Count == 0)
        {
            WebPlatform1UI.SetActive(true);
            WebPlatform2UI.SetActive(true);
            WebPlatform3UI.SetActive(true);
        }
        if (GameManager.Instance.WebPlatformList.Count == 1)
        {
            WebPlatform1UI.SetActive(true);
            WebPlatform2UI.SetActive(true);
            WebPlatform3UI.SetActive(false);
        }
        if (GameManager.Instance.WebPlatformList.Count == 2)
        {
            WebPlatform1UI.SetActive(true);
            WebPlatform2UI.SetActive(false);
            WebPlatform3UI.SetActive(false);
        }
        if (GameManager.Instance.WebPlatformList.Count == 3)
        {
            WebPlatform1UI.SetActive(false);
            WebPlatform2UI.SetActive(false);
            WebPlatform3UI.SetActive(false);
        }
    }

    public IEnumerator ErrorMesageUI()
    {
        errorMessageText.SetActive(true);

        yield return new WaitForSeconds(1);

        errorMessageText.SetActive(false);
        errorCoroutineCache = null;
    }

    private void Fade()
    {
        blackScreen.GetComponent<Animator>().SetBool("FadeToBlack", true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ReturnToTitle()
    {
        UnPause();
        GameManager.Instance.ReturnToTitle();
    }
    public void RetryLevel()
    {
        UnPause();
        GameManager.Instance.RestartCurrentLevel();
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        GameManager.GameIsPaused = true;
        GameManager.CurrentLevel = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 0;
    }
    public void UnPause()
    {
        //print("unpause");
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        GameManager.GameIsPaused = false;
    }

    private void OnDestroy()
    {
        PlayerController.BeeVision -= SwitchHeadUI;
        PlayerController.WebShooterUI -= SwitchLegUI;
        PlayerController.ErrorMessage -= StartErrorCoroutine;
        PlayerController.PlatformCountUI -= ChangePlatformCountUI;
        PlayerController.GamePaused -= Pause;
        FadeToBlack -= Fade;
    }
}
